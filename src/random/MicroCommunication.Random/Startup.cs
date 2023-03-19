using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MicroCommunication.Random.Abstractions;
using MicroCommunication.Random.Authentication;
using MicroCommunication.Random.Hubs;
using MicroCommunication.Random.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
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

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Add Prometheus server
                endpoints.MapMetrics();

                endpoints.MapControllers();
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
