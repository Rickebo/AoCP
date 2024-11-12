using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Problems;
using Backend.Problems.Updates;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ProblemController(ProblemService problemService) : ControllerBase
{
    private static JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    private async Task Transmit(WebSocket socket, ProblemUpdate update)
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

    private async Task<string> Receive(WebSocket webSocket)
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

    private async Task<T?> Receive<T>(WebSocket webSocket) =>
        JsonSerializer.Deserialize<T>(await Receive(webSocket));

    [Route("solve/{year}/{setName}/{problemName}")]
    public async Task Solve(
        [FromRoute] string year,
        [FromRoute] string setName,
        [FromRoute] string problemName
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
            SetName = setName,
            ProblemName = problemName
        };

        var problem = problemService.GetProblem(id);

        if (problem == null)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            return;
        }

        await Transmit(socket, new StartProblemUpdate
        {
            Id = id
        });
        
        try
        {
            await foreach (
                var update in problem.Solve(input).WithCancellation(CancellationToken.None)
            )
            {
                update.Id = id;
                await Transmit(socket, update);
            }
        }
        catch (Exception ex)
        {
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

    [Route("ws")]
    public async Task OpenWebsocket(ProblemId problem)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
        }
    }
}