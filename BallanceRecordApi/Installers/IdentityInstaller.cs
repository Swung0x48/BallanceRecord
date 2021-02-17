using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordOptions = BallanceRecordApi.Options.PasswordOptions;

namespace BallanceRecordApi.Installers
{
    public class IdentityInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var passwordOptions = new PasswordOptions();
            configuration.Bind(nameof(passwordOptions), passwordOptions);
            services.AddSingleton(passwordOptions);
            
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = passwordOptions.RequireDigit;
                options.Password.RequireLowercase = passwordOptions.RequireLowercase;
                options.Password.RequireNonAlphanumeric = passwordOptions.RequireNonAlphanumeric;
                options.Password.RequireUppercase = passwordOptions.RequireUppercase;
                options.Password.RequiredLength = passwordOptions.RequiredLength;
                options.Password.RequiredUniqueChars = passwordOptions.RequiredUniqueChars;
            });
        }
    }
}