using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using MicroCommunication.Random.Abstractions;
using MicroCommunication.Random.Hubs;
using MicroCommunication.Random.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using RandomNameGeneratorLibrary;

namespace MicroCommunication.Random
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
                    builder.ConfigureResource((resource) =>
                    {
                        resource.AddService("Random", "MicroCommunication", Assembly.GetExecutingAssembly().GetName().Version!.ToString(), false, name);
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

            // Enforce lowercase routes
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers();

            services.AddHealthChecks();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("1.0", new OpenApiInfo
                {
                    Title = "Micro Communication Random API",
                    Version = "1.0",
                    Description = "An API for generating random numbers.\n\n" +
                        $"Instance name: {Configuration["RandomName"]}\n\n" +
                        $"Environment: {environment}"
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/1.0/swagger.json", "Version 1.0");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapControllers();
            });
        }
    }
}
