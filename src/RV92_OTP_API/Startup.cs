using System;
using System.IO;
using System.Reflection;
using Autofac;// Configure Dependency Injection: factory pattern
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;// Configure Dependency Injection
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RV92.Otp.WebApi.Infrastructure.Installers;

namespace RV92.Otp.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //Configure Swagger Middleware: Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "OTP API",
                    Description = "This is a One Time Pin (OTP) ASP.NET Core Web API. <br/>This enables you to do 2 factor authentication by validating your secret (Message Identifier) and the other parties secret (One Time Pin).",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Rishal Vallabh",
                        Email = "rishal92@gmail.com",
                        Url = new Uri("https://twitter.com/rishal92"),
                    }
                });

                //Configure Swagger Middleware: Set the comments path for the Swagger JSON and UI - This is needed if you want to document your functions in the controller.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //Configure Sessions: We created a session storage that keeps the OTP values for a day. You can change this to suit your needs
            services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromDays(1);
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Configure Sessions
            app.UseSession();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Configure Swagger Middleware:  Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            //Configure Swagger Middleware:  Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            //Configure Swagger Middleware:  specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "OTP API v1");
            });
        }

        // Configure Dependency Injection
        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            RegisterContainer.RegisterModules(builder);
        }
    }
}
