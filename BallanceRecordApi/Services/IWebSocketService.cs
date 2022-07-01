using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BallanceRecordApi.Services;

public interface IWebSocketService
{
    Task Process(WebSocket ws, RouteData route);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="route"></param>
    /// <param name="predicate">
    /// </param>
    /// <returns></returns>
    Task Broadcast(string message, Func<RouteData, bool> predicate);
}