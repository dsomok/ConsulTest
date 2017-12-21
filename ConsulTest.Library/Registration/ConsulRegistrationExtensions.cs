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
            return registration.AddHttpCheck(registration.Registration.Port, interval, deregisterFailedAfter);
        }

        public static IConsulRegistration AddHttpCheck(this IConsulRegistration registration, int port, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            var httpCheck = new AgentServiceCheck
            {
                DockerContainerID = Environment.MachineName,
                DeregisterCriticalServiceAfter = deregisterFailedAfter,
                Interval = interval,
                HTTP = $"{registration.Registration.Address}:{port}"
            };

            registration.Checks.Add(httpCheck);

            return registration;
        }
    }
}
