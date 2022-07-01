using BallanceRecordApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BallanceRecordApi.Installers;

public class LogicInstaller: IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRecordService, RecordService>();
        services.AddScoped<IRoomService, RoomService>();
    }
}