using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Consul;

namespace ConsulTest.Library.Consul.Registration
{
    public static class ServiceRegistrationBuilderExtensions
    {
        public static IConsulServiceRegistrationBuilder AddHttpHealthCheckEndpoint(this IConsulServiceRegistrationBuilder registrationBuilder, int healthCheckPort)
        {
            var responseBody = Encoding.UTF8.GetBytes("OK");
            var address = $"http://*:{healthCheckPort}/";

            var listener = new HttpListener
            {
                Prefixes = { address }
            };

            listener.Start();

            Task.Run(() =>
            {
                while (true)
                {
                    var context = listener.GetContext();
                    var response = context.Response;

                    response.OutputStream.Write(responseBody, 0, responseBody.Length);
                    response.OutputStream.Close();
                }
            });

            return registrationBuilder;
        }



        public static IConsulServiceRegistrationBuilder AddDefaultHttpCheck(this IConsulServiceRegistrationBuilder registrationBuilder, string relativePath = "")
        {
            return registrationBuilder.AddHttpCheck(
                relativePath: relativePath,
                interval: TimeSpan.FromSeconds(10),
                deregisterFailedAfter: TimeSpan.FromMinutes(2)
            );
        }

        public static IConsulServiceRegistrationBuilder AddHttpCheck(this IConsulServiceRegistrationBuilder registrationBuilder, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            return registrationBuilder.AddHttpCheck(registrationBuilder.Port, interval, deregisterFailedAfter);
        }

        public static IConsulServiceRegistrationBuilder AddHttpCheck(this IConsulServiceRegistrationBuilder registrationBuilder, string relativePath, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            return registrationBuilder.AddHttpCheck(registrationBuilder.Port, relativePath, interval, deregisterFailedAfter);
        }

        public static IConsulServiceRegistrationBuilder AddHttpCheck(this IConsulServiceRegistrationBuilder registrationBuilder, int port, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            return registrationBuilder.AddHttpCheck(port, string.Empty, interval, deregisterFailedAfter);
        }

        public static IConsulServiceRegistrationBuilder AddHttpCheck(this IConsulServiceRegistrationBuilder registrationBuilder, int port, string relativePath, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            var url = $"{registrationBuilder.Address}:{port}{relativePath}";
            var httpCheck = new AgentCheckRegistration
            {
                DockerContainerID = Environment.MachineName,
                DeregisterCriticalServiceAfter = deregisterFailedAfter,
                Interval = interval,
                HTTP = url,
                Name = $"HTTP GET {url} check"
            };

            registrationBuilder.WithHealthCheck(httpCheck);

            return registrationBuilder;
        }


        public static IConsulServiceRegistrationBuilder AddDefaultTTLCheck(this IConsulServiceRegistrationBuilder registrationBuilder)
        {
            return registrationBuilder.AddTTLCheck(
                ttl: TimeSpan.FromSeconds(30),
                interval: TimeSpan.FromSeconds(10),
                deregisterFailedAfter: TimeSpan.FromMinutes(2)
            );
        }

        public static IConsulServiceRegistrationBuilder AddTTLCheck(this IConsulServiceRegistrationBuilder registrationBuilder, TimeSpan ttl, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            var ttlCheck = new AgentCheckRegistration
            {
                DockerContainerID = Environment.MachineName,
                DeregisterCriticalServiceAfter = deregisterFailedAfter,
                TTL = ttl,
                Name = "TTL check"
            };

            registrationBuilder.WithTTLUpdatingPeriod(interval);
            registrationBuilder.WithHealthCheck(ttlCheck);

            return registrationBuilder;
        }
    }
}
