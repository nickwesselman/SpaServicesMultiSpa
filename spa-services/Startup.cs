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

                    spa.UseSpaPrerendering(options =>
                    {
                        options.BootModulePath = $"{spa.Options.SourcePath}/dist-server/main.bundle.js";
                        options.BootModuleBuilder = env.IsDevelopment()
                            ? new AngularCliBuilder(npmScript: "build:ssr")
                            : null;
                        options.ExcludeUrls = new[] { "/app1/sockjs-node" };
                    });

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

                    spa.UseSpaPrerendering(options =>
                    {
                        options.BootModulePath = $"{spa.Options.SourcePath}/dist-server/main.bundle.js";
                        options.BootModuleBuilder = env.IsDevelopment()
                            ? new AngularCliBuilder(npmScript: "build:ssr")
                            : null;
                        options.ExcludeUrls = new[] { "/app2/sockjs-node" };
                    });

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
