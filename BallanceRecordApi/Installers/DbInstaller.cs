using BallanceRecordApi.Data;
using BallanceRecordApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BallanceRecordApi.Installers
{
    public class DbInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>();
            
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
            
            services.AddScoped<IRecordService, RecordService>();
        }
    }
}