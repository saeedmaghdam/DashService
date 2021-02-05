using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DashService.WebApi.WebSocket
{
    public static class Socket
    {
        private static bool _isInitialized = false;
        private static System.Net.WebSockets.WebSocket _webSocket;

        public static void Initialize(IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(20),
                ReceiveBufferSize = 6 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/hub")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        _webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        _isInitialized = true;

                        await ListenToClients(context, _webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        public static async Task CallClientMethod(string command)
        {
            if (!_isInitialized)
                return;

            var buffer = Encoding.UTF8.GetBytes(command);
            await _webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static async Task ListenToClients(HttpContext context,  System.Net.WebSockets.WebSocket socket)
        {
            var buffer = new byte[6 * 1024];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var content = Encoding.UTF8.GetString(buffer).Substring(0, result.Count);

                try
                {
                    CallServerMethod(content);
                }
                catch { }

                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private static void CallServerMethod(string command)
        {
            // Command is like => TestClass.Test(\"092651ef-16fe-4214-bb59-1220fce90a7a\")
            if (Regex.IsMatch(command, @"^\w*\.\w*\(.*?\)$"))
            {
                var match = Regex.Match(command, @"^(\w*)\.(\w*)\(.*?\)$");
                var className = match.Groups[1].Value;
                var methodName = match.Groups[2].Value;

                match = Regex.Match(command, @"^\w*\.\w*\((.*?)\)$");
                var rawParameters = match.Groups[1].Value.Replace("\"", "").Split(",");

                var testClass = Type.GetType($"{MethodInfo.GetCurrentMethod().ReflectedType.Namespace}.{className}");
                var methodInfo = testClass.GetMethod(methodName);
                if (methodInfo != null)
                {
                    var parameters = new List<object>();
                    var methodInfoParameters = methodInfo.GetParameters();
                    if (methodInfoParameters.Length != rawParameters.Length)
                        throw new Exception("Parameter(s) are not correct");

                    for (int i = 0; i < methodInfoParameters.Length; i++)
                    {
                        var methodInfoParameterType = methodInfoParameters[i].ParameterType;
                        var type = Type.GetType(methodInfoParameterType.FullName);
                        var typeParseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
                        var val = typeParseMethod.Invoke(null, new object[] { rawParameters[i] });
                        parameters.Add(val);
                    }

                    methodInfo.Invoke(null, parameters.ToArray());
                }
            }
        }
    }
}
