using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Logging;

namespace HTTPServer
{
    public class WebServer
    {
        public int basePort = 3000;
        public IWebHost host;

        public WebServer(){
			var configuration = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.Build();

			if (!ushort.TryParse(configuration["BASE_PORT"], NumberStyles.None, CultureInfo.InvariantCulture, out var basePort))
			{
				basePort = 5000;
			}

			host = new WebHostBuilder()
				//.ConfigureLogging((_, factory) =>
				//{
				//	factory.AddConsole();
				//})
				.UseKestrel(options =>
				{
					// Run callbacks on the transport thread
					options.ApplicationSchedulingMode = SchedulingMode.Inline;

					options.Listen(IPAddress.Loopback, basePort, listenOptions =>
					{
						// Uncomment the following to enable Nagle's algorithm for this endpoint.
						//listenOptions.NoDelay = false;

						listenOptions.UseConnectionLogging();
					});

					options.Listen(IPAddress.Loopback, basePort + 1, listenOptions =>
					{
						//listenOptions.UseHttps("testCert.pfx", "testPassword");
						listenOptions.UseConnectionLogging();
					});

					options.UseSystemd();

					// The following section should be used to demo sockets
					//options.ListenUnixSocket("/tmp/kestrel-test.sock");
				})
				.UseLibuv(options =>
				{
					// Uncomment the following line to change the default number of libuv threads for all endpoints.
					// options.ThreadCount = 4;
				})
				.UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
				.Build();
        }
    }
}
