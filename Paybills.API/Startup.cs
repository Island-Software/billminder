using Paybills.API.Extensions;
using Paybills.API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Http;
using Paybills.API.Infrastructure.Extensions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

namespace Paybills.API
{
    public class Startup
    {
        public IConfiguration _config { get; }
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddAuthentication(
                CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate();
            services.AddApplicationServices(_config);
            services.AddControllers();
            services.AddIdentityServices(_config);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                    var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                };
            });

            services.AddExceptionHandler<ProblemExceptionHandler>();
            services.AddExceptionHandler<GeneralExceptionHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler();
            // Exception handling is done via IExceptionHandler implementations

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

            // if (env.EnvironmentName == "Development")
            // {
            //     app.UseDeveloperExceptionPage();
            // } 
            // else
            // {PP
            //     app.UseHttpsRedirection();                
            // }

            app.UseRouting();

            app.UseStaticFiles();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(
                "https://localhost:4200",                 
                "http://localhost:4200",
                "https://billminder.com.br"));

            app.UseAuthentication();
            app.UseAuthorization();


            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("Content-Security-Policy", 
                    "default-src 'self'; font-src 'self' https://fonts.googleapis.com/ https://fonts.gstatic.com/; img-src 'self'; object-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com/");
                await next();
            }
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseHealthChecks("/api/health");
        }
    }
}
