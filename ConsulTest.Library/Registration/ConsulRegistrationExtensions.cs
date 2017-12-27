using Consul;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsulTest.Library.Registration
{
    public static class ConsulRegistrationExtensions
    {
        public static IConsulRegistration AddHttpHealthCheckEndpoint(this IConsulRegistration registration, int healthCheckPort)
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

            return registration;
        }



        public static IConsulRegistration AddHttpCheck(this IConsulRegistration registration, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            return registration.AddHttpCheck(registration.Port, interval, deregisterFailedAfter);
        }

        public static IConsulRegistration AddHttpCheck(this IConsulRegistration registration, string relativePath, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            return registration.AddHttpCheck(registration.Port, relativePath, interval, deregisterFailedAfter);
        }

        public static IConsulRegistration AddHttpCheck(this IConsulRegistration registration, int port, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            return registration.AddHttpCheck(port, string.Empty, interval, deregisterFailedAfter);
        }

        public static IConsulRegistration AddHttpCheck(this IConsulRegistration registration, int port, string relativePath, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            var url = $"{registration.Address}:{port}{relativePath}";
            var httpCheck = new AgentCheckRegistration
            {
                DockerContainerID = Environment.MachineName,
                DeregisterCriticalServiceAfter = deregisterFailedAfter,
                Interval = interval,
                HTTP = url,
                Name = $"HTTP GET {url} check"
            };

            registration.AddHealthCheck(httpCheck);

            return registration;
        }



        public static IConsulRegistration AddTTLCheck(this IConsulRegistration registration, TimeSpan ttl, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            var ttlCheck = new AgentCheckRegistration
            {
                DockerContainerID = Environment.MachineName,
                DeregisterCriticalServiceAfter = deregisterFailedAfter,
                Interval = interval,
                TTL = ttl,
                Name = $"TTL check"
            };

            registration.AddHealthCheck(ttlCheck);

            return registration;
        }
    }
}
