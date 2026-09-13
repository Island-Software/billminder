using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Paybills.API.Infrastructure.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenKey = string.Empty;

            tokenKey = Environment.GetEnvironmentVariable("TOKEN_KEY");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    if (tokenKey != null)
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                        };
                    }

                    opt.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // 1. Check if the token is expired and attach a flag to the context items
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.HttpContext.Items["IsTokenExpired"] = true;
                            }

                            return Task.CompletedTask;
                        },
                        OnChallenge = async context =>
                        {
                            // 2. Skip the default logic to avoid duplicate headers/responses
                            context.HandleResponse();

                            // 3. Check if our custom expiration flag exists
                            bool isExpired = context.HttpContext.Items.ContainsKey("IsTokenExpired");

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            // 4. Define your custom payload structure
                            var responsePayload = new
                            {
                                StatusCode = StatusCodes.Status401Unauthorized,
                                Message = isExpired ? "Token has expired" : "Unauthorized access",
                                Code = isExpired ? "TOKEN_EXPIRED" : "INVALID_TOKEN"
                            };

                            // 5. Serialize and write the response
                            var json = JsonSerializer.Serialize(responsePayload);
                            await context.Response.WriteAsync(json);
                        }
                    };
                });

            return services;
        }
    }
}