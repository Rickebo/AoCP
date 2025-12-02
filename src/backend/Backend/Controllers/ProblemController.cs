using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Backend.Services;
using Common;
using Common.Updates;
using Lib.Parsing;
using Lib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ProblemController(
    ILogger<ProblemController> logger,
    ProblemService problemService
) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static async Task Transmit(WebSocket socket, ProblemUpdate update)
    {
        var json = JsonSerializer.Serialize(update, JsonOptions);
        var jsonBytes = Encoding.UTF8.GetBytes(json);
        await socket.SendAsync(
            jsonBytes,
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None
        );
    }

    private static async Task<string> Receive(WebSocket webSocket)
    {
        var buffers = new List<byte[]>();
        while (true)
        {
            var buffer = new byte[0x1000];

            var result = await webSocket.ReceiveAsync(
                buffer,
                CancellationToken.None
            );

            var readBytes = result.Count != buffer.Length
                ? buffer[..result.Count]
                : buffer;

            buffers.Add(readBytes);

            if (result.EndOfMessage)
                break;
        }

        var bytes = buffers.SelectMany(b => b).ToArray();
        return Encoding.UTF8.GetString(bytes);
    }

    private static async Task<T?> Receive<T>(WebSocket webSocket) =>
        JsonSerializer.Deserialize<T>(await Receive(webSocket));

    [Route("solve/{source}/{year}/{author}/{setName}/{problemName}")]
    public async Task Solve(
        [FromRoute] string source,
        [FromRoute] string year,
        [FromRoute] string author,
        [FromRoute]
        string setName,
        [FromRoute]
        string problemName
    )
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        var input = await Receive(socket);

        var id = new ProblemId
        {
            Year = int.Parse(year),
            Source = source,
            Author = author,
            SetName = setName,
            ProblemName = problemName
        };

        var problem = problemService.GetProblem(id);

        if (problem == null)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        await Transmit(
            socket,
            new StartProblemUpdate
            {
                Id = id
            }
        );

        try
        {
            var reporter = new Reporter();
            var cts = new CancellationTokenSource();
            var problemTask = Task.Run(
                async () =>
                {
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        await problem.Solve(input, reporter);
                        return sw.Elapsed;
                    }
                    finally
                    {
                        cts.Cancel();
                    }
                }
            );

            var postponed = new Queue<ProblemUpdate>();

            try
            {
                await foreach (
                    var update in reporter.ReadAll().WithCancellation(cts.Token)
                )
                {
                    if (update is FinishedProblemUpdate finished)
                        postponed.Enqueue(finished);
                    else
                    {
                        update.Id = id;
                        await Transmit(socket, update);                        
                    }
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
            {
            }

            var elapsed = await problemTask;

            foreach (var update in postponed.Concat(reporter.ReadAllCurrent()))
            {
                if (update is FinishedProblemUpdate finished)
                    finished.ElapsedNanoseconds = elapsed.Ticks * 100;
                
                update.Id = id;
                await Transmit(socket, update);
            }
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError("{Message}", $"An exception occurred while solving problem: {setName} -> {problemName}");

            PrintException(ex);

            await Transmit(
                socket,
                new FinishedProblemUpdate
                {
                    Error = ex.Message,
                    Solution = null,
                    Successful = false
                }
            );
        }
    }

    private static void PrintException(Exception ex)
    {
        // Split exception content
        string[] lines = ex.ToString().SplitLines();

        // Print type and message
        if (lines.Length > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var part in lines[0].SplitBy([": "]))
                Console.WriteLine(new string(' ', 6) + part);
            Console.ResetColor();
        }

        // Format stack trace
        if (lines.Length > 1)
        {
            foreach (var line in lines[1..])
            {
                // Retrieve relevant lines
                var index = line.IndexOf("src\\backend");
                if (index >= 0)
                {
                    string[] parts = line[index..].SplitBy([":"]);
                    if (parts.Length > 1)
                    {
                        Console.Write(new string(' ', 6) + parts[0] + " - ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(parts[1]);
                        Console.ResetColor();
                    }
                }
            }
        }
    }

    [Route("ws")]
    public async Task OpenWebsocket(ProblemId problem)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
        }
    }
}
