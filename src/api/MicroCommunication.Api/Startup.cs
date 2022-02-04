using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MicroCommunication.Api.Abstractions;
using MicroCommunication.Api.Authentication;
using MicroCommunication.Api.Hubs;
using MicroCommunication.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using RandomNameGeneratorLibrary;

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

            switch (Configuration["Database"])
            {
                case "CosmosDB":
                    services.AddSingleton<IHistoryService>(new CosmosDbHistoryService(Configuration["CosmosDbConnectionString"]));
                    break;
                case "MongoDB":
                    services.AddSingleton<IHistoryService>(new MongoDbHistoryService(Configuration["MongoDbConnectionString"]));
                    break;
                default:
                    throw new Exception("Unknown database type: " + Configuration["Database"]);
            }

            // Logging
            if (!string.IsNullOrEmpty(Configuration["ApplicationInsightsInstrumentationKey"]))
            {
                services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsightsInstrumentationKey"]);
                Console.WriteLine("Application Insights configured.");
            }

            // Create random name for testing session affinity
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstName();
            Configuration["RandomName"] = name;
            Console.WriteLine("My instance name is: " + Configuration["RandomName"]);

            // Environment
            var environment = string.IsNullOrEmpty(Configuration["EnvironmentName"]) ? "Default" : Configuration["EnvironmentName"];
            Console.WriteLine("My environment is: " + environment);

            // Enforce lowercase routes
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers();

            // SignalR
            var signalR = services.AddSignalR();
            if (!string.IsNullOrEmpty(Configuration["RedisCacheConnectionString"]))
                signalR.AddStackExchangeRedis(Configuration["RedisCacheConnectionString"]);

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("1.0", new OpenApiInfo
                {
                    Title = "Random API ",
                    Version = "1.0",
                    Description = "An API for generating random numbers.\n\n" +
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

            // CORS
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();

                if (!string.IsNullOrEmpty(Configuration["Cors"]))
                    builder.WithOrigins(Configuration["Cors"]);
            }));
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

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Add Prometheus server
                endpoints.MapMetrics();

                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/1.0/swagger.json", "Version 1.0");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
