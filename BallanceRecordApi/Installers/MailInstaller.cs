using BallanceRecordApi.Options;
using BallanceRecordApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BallanceRecordApi.Installers
{
    public class MailInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var emailOptions = new EmailOptions();
            configuration.Bind(nameof(emailOptions), emailOptions);
            services.AddSingleton(emailOptions);
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}