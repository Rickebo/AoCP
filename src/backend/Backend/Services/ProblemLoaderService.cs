using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Backend.Problems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp;
using Basic.Reference.Assemblies;
using Microsoft.Extensions.Hosting;
using static System.Threading.Tasks.Task;

namespace Backend.Services;

public class ProblemLoaderService : IHostedService
{
    private readonly IEnumerable<ProblemCollection> _problemCollections;
    private readonly DirectoryInfo _monitorRoot;
    private IReadOnlyCollection<MetadataReference> _references;
    private Task? _watchTask;
    private Task? _loadTask;
    private List<DirectoryInfo> _rootDirectories;

    public unsafe ProblemLoaderService(IEnumerable<ProblemCollection> problemCollections)
    {
        _problemCollections = problemCollections;
        var srcs = new List<Assembly>
        {
            typeof(Common.Reporter).Assembly,
            typeof(Lib.StringExtensions).Assembly,
            typeof(ProblemLoaderService).Assembly
        };
        
        srcs.AddRange(AppDomain.CurrentDomain.GetAssemblies());

        var references = new List<MetadataReference>(ReferenceAssemblies.Net80);

        foreach (var a in srcs)
        {
            if (!a.TryGetRawMetadata(out var bytes, out var length))
                continue;

            var moduleMetadata = ModuleMetadata.CreateFromMetadata((nint)bytes, length);
            var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
            var reference = assemblyMetadata.GetReference();

            references.Add(reference);
        }

        _references = references;
        _monitorRoot = new DirectoryInfo(
            Path.Combine(Environment.CurrentDirectory, "Problems")
        );

        _rootDirectories = _monitorRoot
            .GetDirectories("Year*")
            .SelectMany(yearDir => yearDir.GetDirectories())
            .ToList();
    }

    private async Task WatchTree(string source)
    {
        var watcher = new FileSystemWatcher();
        watcher.Path = source;
        watcher.Filter = "*.cs";
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.EnableRaisingEvents = true;

        watcher.Created += (_, e) => Run(() => UpdateFile(e.FullPath));
        watcher.Changed += (_, e) => Run(() => UpdateFile(e.FullPath));

        watcher.BeginInit();
    }

    private async Task LoadAll(DirectoryInfo root)
    {
        foreach (var file in root.GetFiles("*.cs"))
            await UpdateFile(file.FullName);

        foreach (var dir in root.GetDirectories())
            await LoadAll(dir);
    }

    private async Task UpdateFile(string path)
    {
        var fileInfo = new FileInfo(path);
        if (!_rootDirectories.Any(dir => fileInfo.FullName.StartsWith(dir.FullName)))
            return;

        var bytes = await File.ReadAllBytesAsync(fileInfo.FullName);
        var codeString = SourceText.From(bytes, bytes.Length);
        var options =
            CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12);

        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

        var compilation = CSharpCompilation.Create(
            "aocp.Problem",
            new[] { parsedSyntaxTree },
            references: _references,
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default,
                allowUnsafe: true,
                platform: Platform.AnyCpu
            )
        );

        var ms = new MemoryStream();
        var emitResult = compilation.Emit(ms);

        if (!emitResult.Success)
        {
            var msg = new StringBuilder();
            foreach (var diagnostic in emitResult.Diagnostics)
                msg.AppendLine(diagnostic.ToString());

            throw new Exception(msg.ToString());
        }

        var assembly = Assembly.Load(ms.ToArray());

        foreach (var collection in _problemCollections)
            collection.FindProblems(assembly: assembly);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _watchTask = Run(() => WatchTree(_monitorRoot.FullName), cancellationToken);
        _loadTask = Run(() => LoadAll(_monitorRoot), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return CompletedTask;
    }
}