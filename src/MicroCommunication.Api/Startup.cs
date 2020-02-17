using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MicroCommunication.Api.Authentication;
using MicroCommunication.Api.Hubs;
using MicroCommunication.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RandomNameGeneratorLibrary;
using Swashbuckle.AspNetCore.Swagger;

namespace MicroCommunication.Api
{
    public class Startup
    {
        readonly bool useApiKey;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            useApiKey = !string.IsNullOrEmpty(configuration["ApiKey"]);
            Console.WriteLine("Using API Key: " + useApiKey);
        }

        public IConfiguration Configuration { get; }

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

            services.AddSingleton(new HistoryService(Configuration["MongoDbConnectionString"]));

            // Create random name for testing session affinity
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstName();
            Configuration["RandomName"] = name;
            Console.WriteLine("My name is " + Configuration["RandomName"]);

            // Enforce lowercase routes
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("1.0", new Info
                {
                    Title = "Random API ",
                    Version = "1.0",
                    Description = $"An API for generating random numbers.\nMy name is {Configuration["RandomName"]}."
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (useApiKey)
                {
                    c.AddSecurityDefinition("API Key", new ApiKeyScheme
                    {
                        Description = "Add the key to access this API to the HTTP header of your requests.",
                        Name = "api-key",
                        In = "header",
                        Type = "apiKey"
                    });
                    c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "API Key", new string[] {} },
                });
                }
            });

            // CORS
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    //.WithOrigins("http://localhost:4200")
                    .WithMethods("GET", "POST")
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthentication();
            if (useApiKey)
            {
                app.UseApiKey(c =>
                {
                    c.ApiKeyHeaderName = "api-key";
                    c.ApiKey = Configuration["ApiKey"];
                });
            }

            app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<EchoHub>("/echo");
            });
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/1.0/swagger.json", "Version 1.0");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
