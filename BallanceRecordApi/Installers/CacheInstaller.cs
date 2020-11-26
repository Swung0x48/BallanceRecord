using BallanceRecordApi.Cache;
using BallanceRecordApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BallanceRecordApi.Installers
{
    public class CacheInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisCacheOptions = new RedisCacheOptions();
            configuration.GetSection(nameof(RedisCacheOptions)).Bind(redisCacheOptions);
            services.AddSingleton(redisCacheOptions);

            if (!redisCacheOptions.Enabled)
            {
                return;
            }

            services.AddStackExchangeRedisCache(options => options.Configuration = redisCacheOptions.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}