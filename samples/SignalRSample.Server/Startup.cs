// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SignalRSample.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseIISPlatformHandler();
            app.UseFileServer();

            app.UseWebSockets();
            app.UseSignalR();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseServer("Microsoft.AspNetCore.Server.Kestrel")
                .UseUrls("http://localhost:5001")
                .UseDefaultConfiguration(args)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
