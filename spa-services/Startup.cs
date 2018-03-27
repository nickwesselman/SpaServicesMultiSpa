using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace spa_services
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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // need to be able to spin up multiple SPAs
            // https://github.com/aspnet/JavaScriptServices/issues/1491
            // Running late could be issue for plugins until Bristol solves: https://github.com/aspnet/JavaScriptServices/blob/c8b337ebaa67c888901cbed4b3fd574a9113df15/src/Microsoft.AspNetCore.SpaServices.Extensions/SpaApplicationBuilderExtensions.cs#L21
            // More on middleware: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/index?tabs=aspnetcore2x#use-run-and-map
            /*            
                spa.UseSpaPrerendering(options =>
                {
                    options.BootModulePath = $"{spa.Options.SourcePath}/dist-server/main.bundle.js";
                    options.BootModuleBuilder = env.IsDevelopment()
                        ? new AngularCliBuilder(npmScript: "build:ssr")
                        : null;
                    options.ExcludeUrls = new[] { "/sockjs-node" };
                });
            */

            app.Map("/app1", app1 => {
                var fileOptions = new StaticFileOptions();
                if (!env.IsDevelopment()) {
                    // this will error if ng prod build has not been run
                    fileOptions.FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "ClientApp/dist"));
                    app1.UseSpaStaticFiles(options: fileOptions);
                }

                app1.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";
                    spa.Options.DefaultPage = "/index.html";

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start:hosted");
                    }
                    else
                    {
                        spa.Options.DefaultPageStaticFileOptions = fileOptions;
                    }
                });
            });

            app.Map("/app2", app2 => {
                var fileOptions = new StaticFileOptions();
                if (!env.IsDevelopment()) {
                    // this will error if ng prod build has not been run
                    fileOptions.FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "ClientApp2/dist"));
                    app2.UseSpaStaticFiles(options: fileOptions);
                }

                app2.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp2";
                    spa.Options.DefaultPage = "/index.html";

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start:hosted");
                    }
                    else
                    {
                        spa.Options.DefaultPageStaticFileOptions = fileOptions;
                    }
                });
            });
        }
    }
}
