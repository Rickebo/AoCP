using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Backend.Problems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Hosting;
using static System.Threading.Tasks.Task;

namespace Backend.Services;

public class ProblemLoaderService : IHostedService
{
    private readonly IEnumerable<ProblemCollection> _problemCollections;
    private readonly DirectoryInfo _monitorRoot;
    private readonly FileSystemWatcher _fileSystemWatcher;
    private readonly HashAlgorithm _hashAlgorithm = SHA256.Create();
    private readonly IReadOnlyCollection<MetadataReference> _references;
    private readonly List<DirectoryInfo> _rootDirectories;
    private readonly ConcurrentDictionary<string, string> _fileHash = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

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

        _rootDirectories = [.. _monitorRoot
            .GetDirectories("Year*")
            .SelectMany(yearDir => yearDir.GetDirectories())];


        _fileSystemWatcher = new FileSystemWatcher
        {
            Path = _monitorRoot.FullName,
            NotifyFilter = NotifyFilters.LastWrite,

            IncludeSubdirectories = true
        };

        _fileSystemWatcher.Created += (_, e) => Run(() => UpdateFile(e.FullPath));
        _fileSystemWatcher.Changed += (_, e) => Run(() => UpdateFile(e.FullPath));
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
        if (Directory.Exists(path) ||
            !_rootDirectories.Any(dir => fileInfo.FullName.StartsWith(dir.FullName)))
            return;

        var bytes = await File.ReadAllBytesAsync(fileInfo.FullName);
        var hash = BitConverter.ToString(_hashAlgorithm.ComputeHash(bytes));

        if (_fileHash.TryGetValue(path, out var existingHash) && hash == existingHash)
            return;

        _fileHash[path] = hash;

        var pdbPath = Path.ChangeExtension(path, "pdb");
        var codeString = SourceText.From(bytes, bytes.Length);
        var options = CSharpParseOptions.Default.WithLanguageVersion(
            LanguageVersion.CSharp12
        );

        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(
            codeString,
            options,
            path: path
        );

        var compilation = CSharpCompilation.Create(
            "aocp.Problem",
            [parsedSyntaxTree],
            references: _references,
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Debug,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default,
                allowUnsafe: true,
                platform: Platform.AnyCpu
            )
        );

        using var ms = new MemoryStream();
        using var pdbStream = new MemoryStream();
        var emitOptions = new EmitOptions(
            debugInformationFormat: DebugInformationFormat.PortablePdb,
            pdbFilePath: pdbPath
        );
        
        var emitResult = compilation.Emit(peStream: ms, pdbStream: pdbStream, options: emitOptions);

        if (!emitResult.Success)
        {
            var msg = new StringBuilder();
            foreach (var diagnostic in emitResult.Diagnostics)
                msg.AppendLine(diagnostic.ToString());

            throw new Exception(msg.ToString());
        }

        ms.Seek(0, SeekOrigin.Begin);
        pdbStream.Seek(0, SeekOrigin.Begin);
        
        var assembly = AssemblyLoadContext.Default.LoadFromStream(ms, pdbStream);

        foreach (var collection in _problemCollections)
            collection.FindProblems(assembly: assembly);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _fileSystemWatcher.EnableRaisingEvents = true;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return CompletedTask;
    }
}