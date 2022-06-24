using BallanceRecordApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio.AspNetCore;
using MinioOptions = BallanceRecordApi.ObjectStorage.MinioOptions;

namespace BallanceRecordApi.Installers
{
    public class ObjectStorageInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var minioOptions = new MinioOptions();
            configuration.GetSection(nameof(MinioOptions))
                .Bind(minioOptions);
            services.AddSingleton(minioOptions);

            services.AddMinio(options =>
            {
                options.Endpoint = minioOptions.Endpoint;
                options.AccessKey = minioOptions.AccessKey;
                options.SecretKey = minioOptions.SecretKey;
                // options.ConfigureClient = client =>
                // {
                //     if (minioOptions.Ssl)
                //         client.WithSSL();
                // };
            });
            services.AddScoped<IObjectStorageService, ObjectStorageService>();
        }
    }
}