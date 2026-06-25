using test3.API.Providers.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using System.Text;
using test3.Common;

namespace test3.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("test3_CORS", policy =>
                {
                    policy.AllowAnyOrigin()
                              // .WithOrigins("URL")
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                });
            });
            #endregion

            #region Cache
            builder.Services.AddMemoryCache();
            #endregion

            #region Serilog
            var logPath = builder.Configuration["LogPath"] ?? "C:\\JiaFuHo - GF66\\Programs\\Others\\test3\\test3.Logs";

            Log.Logger = new LoggerConfiguration()
                                   .WriteTo.Console(
                                       outputTemplate: "{Timestamp:HH:mm} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                                       restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
                                   )
                                   .WriteTo.Async(x => x.File(
                                       outputTemplate: "{Timestamp:HH:mm} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                                       path: logPath,
                                       retainedFileCountLimit: null,
                                       rollingInterval: RollingInterval.Day,
                                       shared: true
                                   ))
                                   .MinimumLevel.Verbose()
                                   .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                                   .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                                   .CreateLogger();

            builder.Host.UseSerilog();

            _loggerX.Decorator = new LoggerConfiguration()
                                                   .WriteTo.Console(
                                                        outputTemplate: "{Message:lj}{NewLine}",
                                                        restrictedToMinimumLevel: LogEventLevel.Information
                                                    )
                                                  .WriteTo.File(
                                                       outputTemplate: "{Message:lj}{NewLine}",
                                                       path: logPath,
                                                       retainedFileCountLimit: null,
                                                       rollingInterval: RollingInterval.Day,
                                                       shared: true
                                                   )
                                                  .MinimumLevel.Verbose()
                                                  .CreateLogger();
            #endregion

            #region Providers
            builder.Services.AddScoped<IAuthP, AuthP>();
            #endregion

            #region BLL
            builder.Services.AddScoped<BLL.test3L>();
            #endregion

            #region DAL
            //var HIS3 = builder.Configuration.GetConnectionString("HIS3") ?? throw new Exception("System Para Error: HIS3 ConnStr");

            // builder.Services.ConnDB(HIS3);
            #endregion

            #region JWT
            var JWT = builder.Configuration.GetSection("JWT");

            var SK = JWT["SK"] ?? throw new Exception("System Para Error: JWT.SK");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = JWT["Issuer"] ?? throw new Exception("System Para Error: JWT.Issuer"),
                    ValidAudience = JWT["Audience"] ?? throw new Exception("System Para Error: JWT.Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SK))
                };
            });
            #endregion

            #region API
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

                    if (document.Paths != null)
                    {
                        foreach (var path in document.Paths.Values)
                        {
                            if (path.Operations == null) { continue; }

                            foreach (var operation in path.Operations.Values)
                            {
                                operation.Security ??= new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();
                                operation.Security.Add(new Microsoft.OpenApi.OpenApiSecurityRequirement
                                {
                                    [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                                });
                            }
                        }
                    }

                    return Task.CompletedTask;
                });
            });
            #endregion

            var app = builder.Build();

            #region Middleware
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseCors("test3_CORS");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}