using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BallanceRecordApi.Installers
{
    public class MvcInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(x => x.SwaggerDoc(
                "v1",
                new Microsoft.OpenApi.Models.OpenApiInfo {Title = "Ballance Record API", Version = "v1"}
            ));
        }
    }
}