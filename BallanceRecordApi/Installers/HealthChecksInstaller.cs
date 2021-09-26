using BallanceRecordApi.Data;
using BallanceRecordApi.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.AspNetCore.HealthChecks;

namespace BallanceRecordApi.Installers
{
    public class HealthChecksInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<DataContext>()
                .AddCheck<RedisHealthCheck>("Redis")
                .AddMinio(sp => sp.GetRequiredService<MinioClient>());
        }
    }
}