    using System.Net.WebSockets;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    namespace RaceTrack {
        public class Startup {
            public Startup (IConfiguration configuration) {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices (IServiceCollection services) { }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure (IApplicationBuilder app, IHostingEnvironment env, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory) {
                loggerFactory.AddConsole (LogLevel.Debug);
                loggerFactory.AddDebug (LogLevel.Debug);
                if (env.IsDevelopment ()) {
                    app.UseDeveloperExceptionPage ();
                }
                app.UseWebSockets ();

                app.Use (async (context, next) => {
                    if (context.Request.Path == "/ws") {
                        if (context.WebSockets.IsWebSocketRequest) {
                            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync ();
                            await Echo (context, webSocket);
                        } else {
                            context.Response.StatusCode = 400;
                        }
                    } else {
                        await next ();
                    }

                });
            }
            private async Task Echo (HttpContext context, WebSocket webSocket) {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync (new ArraySegment<byte> (buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue) {
                    await webSocket.SendAsync (new ArraySegment<byte> (buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                    result = await webSocket.ReceiveAsync (new ArraySegment<byte> (buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync (result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }
    }