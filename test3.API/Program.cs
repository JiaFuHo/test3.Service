using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace test3.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new Microsoft.OpenApi.Models.OpenApiComponents();
                    document.Components.SecuritySchemes.Add("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    });
                    document.SecurityRequirements ??= new List<Microsoft.OpenApi.Models.OpenApiSecurityRequirement>();
                    document.SecurityRequirements.Add(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        [new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>()
                    });

                    return Task.CompletedTask;
                });
            });

            var jwt = builder.Configuration.GetSection("JwtSettings");
            var SK = jwt["SK"] ?? "JFH^260608#test3.Service/JFH^260608#test3.Service/JFH^260608#test3.Service/";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt["Issuer"] ?? "LaJiaGer",
                    ValidAudience = jwt["Audience"] ?? "Guest",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SK))
                };
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}