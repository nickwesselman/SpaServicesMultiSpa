using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using spa_services;

namespace Microsoft.AspNetCore.Builder
{

    public static class SpaPluginExtensions {
        public static void PluginSpa(this IApplicationBuilder app, IHostingEnvironment env, Action<SpaPluginOptions> configuration) {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            var options = new SpaPluginOptions();
            configuration.Invoke(options);

            // branch middleware to a app-specific path
            app.Map(options.MapPath, app1 => {
                // we only need SPA static files in prod mode
                // don't use AddSpaStaticFiles -- it puts a Singleton file provider in the container
                var fileOptions = new StaticFileOptions();
                if (!env.IsDevelopment()) {
                    // path should be dist folder of the SPA
                    // this will error if ng prod build has not been run
                    var staticPath = Path.Combine(Directory.GetCurrentDirectory(), $"{options.SourcePath}{options.DistPath}");
                    fileOptions.FileProvider = new PhysicalFileProvider(staticPath);

                    // this will root in the MapPath since we are branched
                    app1.UseSpaStaticFiles(options: fileOptions);
                }

                // create the SPA within the branch path
                app1.UseSpa(spa =>
                {
                    spa.Options.SourcePath = options.SourcePath;
                    spa.Options.DefaultPage = options.DefaultPage;

                    spa.UseSpaPrerendering(renderingOptions =>
                    {
                        renderingOptions.BootModulePath = $"{spa.Options.SourcePath}{options.ServerRenderBundlePath}";
                        renderingOptions.BootModuleBuilder = env.IsDevelopment()
                            ? new AngularCliBuilder(npmScript: options.ServerRenderBuildScript)
                            : null;
                        // proxied socksjs-node will be under the branched path
                        renderingOptions.ExcludeUrls = new[] { $"{options.MapPath}/sockjs-node" };
                    });

                    if (env.IsDevelopment())
                    {
                        // this defaults to start:hosted, which has some ng serve options for multi-spa
                        spa.UseAngularCliServer(npmScript: options.DevServerScript);
                    }
                    else
                    {
                        // ensure the DefaultPage is found within the app
                        spa.Options.DefaultPageStaticFileOptions = fileOptions;
                    }
                });
            });
        }
    }

}