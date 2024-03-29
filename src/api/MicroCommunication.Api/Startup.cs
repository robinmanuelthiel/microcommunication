using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MicroCommunication.Api.Abstractions;
using MicroCommunication.Api.Authentication;
using MicroCommunication.Api.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RandomNameGeneratorLibrary;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Refit;

namespace MicroCommunication.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        readonly bool useApiKey;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            useApiKey = !string.IsNullOrEmpty(configuration["ApiKey"]);
            Console.WriteLine("Using API Key: " + useApiKey);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (useApiKey)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                }).AddApiKeySupport(options =>
                {
                    options.ApiKeyHeaderName = "api-key";
                    options.ApiKey = "";
                });
            }

            // Register Random API
            services
                .AddRefitClient<IRandomApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["RandomApiUrl"]));

            // Create random name for testing session affinity
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstName();
            Configuration["RandomName"] = name;
            Console.WriteLine("My instance name is: " + Configuration["RandomName"]);

            // Environment
            var environment = string.IsNullOrEmpty(Configuration["EnvironmentName"]) ? "Default" : Configuration["EnvironmentName"];
            Console.WriteLine("My environment is: " + environment);

            // Monitoring
            services
                .AddOpenTelemetry()
                .WithMetrics(builder =>
                {
                    builder.AddRuntimeInstrumentation();
                    builder.AddHttpClientInstrumentation();
                    builder.AddAspNetCoreInstrumentation();
                    builder.AddPrometheusExporter();
                })
                .WithTracing(builder =>
                {
                    builder.AddAspNetCoreInstrumentation();
                    builder.AddEntityFrameworkCoreInstrumentation();
                    builder.AddSource(Observability.DefaultActivities.Name);
                    builder.ConfigureResource((resource) =>
                    {
                        resource.AddService("API", "MicroCommunication", Assembly.GetExecutingAssembly().GetName().Version!.ToString(), false, name);
                    });
                });

            if (!string.IsNullOrEmpty(Configuration["ApplicationInsightsConnectionString"]))
            {
                services.AddOpenTelemetry().UseAzureMonitor(options =>
                {
                    options.ConnectionString = Configuration["ApplicationInsightsConnectionString"];
                });
                Console.WriteLine("Using Azure Application Insights");
            }

            // CORS
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                var cors = string.IsNullOrEmpty(Configuration["Cors"])
                    ? "http://localhost:5000"
                    : Configuration["Cors"];
                Console.WriteLine("CORS: " + Configuration["Cors"]);

                builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins(Configuration["Cors"])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            // Enforce lowercase routes
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers();

            services.AddHealthChecks();

            // SignalR
            var signalR = services.AddSignalR();
            if (!string.IsNullOrEmpty(Configuration["RedisCacheConnectionString"]))
                signalR.AddStackExchangeRedis(Configuration["RedisCacheConnectionString"]);

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("1.0", new OpenApiInfo
                {
                    Title = "Micro Communication API Gateway",
                    Version = "1.0",
                    Description = "Public Micro Communication API.\n\n" +
                        $"Instance name: {Configuration["RandomName"]}\n\n" +
                        $"Environment: {environment}"
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                if (useApiKey)
                {
                    c.AddSecurityDefinition("API Key", new OpenApiSecurityScheme
                    {
                        Description = "Add the key to access this API to the HTTP header of your requests.",
                        Name = "api-key",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "API Key",
                                    Type = ReferenceType.SecurityScheme
                                }
                            }, new List<string>()
                        }
                    });
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (useApiKey)
            {
                app.UseApiKey(c =>
                {
                    c.ApiKeyHeaderName = "api-key";
                    c.ApiKey = Configuration["ApiKey"];
                });
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/1.0/swagger.json", "Version 1.0");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseOpenTelemetryPrometheusScrapingEndpoint();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapHub<ChatHub>("/chat");
            });




        }
    }
}
