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

            #region DAL

            #endregion

            #region API
            builder.Services.AddScoped<BLL.test3L>();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new Microsoft.OpenApi.OpenApiComponents();

                    document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes.Add("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
                    {
                        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    });

                    return Task.CompletedTask;
                });
            });
            #endregion

            #region JWT
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
            #endregion

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