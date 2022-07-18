using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BallanceRecordApi.Services;

public class WebSocketService: IWebSocketService
{
    private class Connection
    {
        public readonly WebSocket Socket;
        public readonly RouteData Route;

        public Connection(WebSocket socket, RouteData route)
        {
            Socket = socket;
            Route = route;
        }
    }
    private readonly ConcurrentDictionary<int, Connection> _aliveConnections = new();

    public async Task Process(WebSocket ws, RouteData route)
    {
        var connection = new Connection(ws, route);
        // Console.WriteLine($"accepting {ws.GetHashCode()}");
        _aliveConnections.TryAdd(connection.GetHashCode(), connection);
        var buffer = new byte[5 * 1024];
        var res = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var username = $"Unidentified{ws.GetHashCode()}";
        while (!res.CloseStatus.HasValue) {
            var content = Encoding.UTF8.GetString(buffer, 0, res.Count);
            if (!string.IsNullOrEmpty(content)) {
                Console.WriteLine($"Received from {ws.GetHashCode()}: {content}");
                if (content == "/close")
                    break;
            }
            res = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await ws.CloseAsync(res.CloseStatus ?? WebSocketCloseStatus.InternalServerError, res.CloseStatusDescription, CancellationToken.None);
        _aliveConnections.TryRemove(connection.GetHashCode(), out _);
    }

    public async Task Broadcast(string message, Func<RouteData, bool> predicate)
    {
        var buff = Encoding.UTF8.GetBytes(message);
        var data = new ArraySegment<byte>(buff, 0, buff.Length);
        foreach (var (id, connection) in _aliveConnections)
        {
            if (connection.Socket.State == WebSocketState.Open && predicate(connection.Route))
            {
                await connection.Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
    
    
}