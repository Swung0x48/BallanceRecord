using BallanceRecordApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BallanceRecordApi.Installers;

public class WebSocketInstaller: IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IWebSocketService, WebSocketService>();
    }
}