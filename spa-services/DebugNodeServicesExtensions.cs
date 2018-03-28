using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.NodeServices;

namespace Microsoft.AspNetCore.Builder
{
    public static class DebugNodeServicesExtensions
    {
        public static void AddDebugNodeServices(this IServiceCollection services, int startPort = 9229)
        {
            var port = startPort;
            services.AddTransient<INodeServices>(serviceProvider => {
                var options = new NodeServicesOptions(serviceProvider);
                options.LaunchWithDebugging = true;
                options.DebuggingPort = port;
                port++;
                return NodeServicesFactory.CreateNodeServices(options);
            });
        }
    }
}