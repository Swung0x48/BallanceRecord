using System;
using System.Collections.Generic;
using System.Text;
using BallanceRecordApi.Filters;
using BallanceRecordApi.Options;
using BallanceRecordApi.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BallanceRecordApi.Installers
{
    public class MvcInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(mvcConfiguration => mvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            
            #region JWT Authentication

            try
            {
                var jwtOptions = new JwtOptions();
                configuration.Bind(nameof(jwtOptions), jwtOptions);
                services.AddSingleton(jwtOptions);

                services.AddScoped<IIdentityService, IdentityService>();

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
                services.AddSingleton(tokenValidationParameters);

                services.AddAuthentication(x =>
                    {
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(x =>
                    {
                        x.SaveToken = true;
                        x.TokenValidationParameters = tokenValidationParameters;
                    });
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"Cannot retrieve parameter {e.ParamName}. Maybe forgot to create an appsettings.json?");
                Environment.Exit(1);
            }

            #endregion

            services.AddAuthorization();

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(
                    "v1",
                    new OpenApiInfo {Title = "Ballance Record API", Version = "v1"}
                );
                
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme { Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }}, new List<string>()}
                });
            });

            services.AddSingleton<IUriService>(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext?.Request;
                var absoluteUri = $"{request?.Scheme}://{request?.Host.ToUriComponent()}/";
                return new UriService(absoluteUri);
            });
        }
    }
}