using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsulTest.Library.Registration
{
    internal class ConsulRegistration : IConsulRegistration
    {
        private readonly AgentServiceRegistration _registration;
        private readonly IConsulClient _client;
        private IList<AgentServiceCheck> _healthChecks = new List<AgentServiceCheck>();



        public ConsulRegistration(IConsulClient client, string serviceName, int port)
        {
            this._client = client;

            var address = this.GetAddress();
            this._registration = new AgentServiceRegistration
            {
                ID = this.GetServiceID(serviceName),
                Name = serviceName,
                Address = $"http://{address}",
                Port = port
            };
        }



        public IConsulRegistration AddHttpHealthCheckEndpoint(int healthCheckPort)
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

            return this;
        }

        public IConsulRegistration AddHttpCheck(TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            return this.AddHttpCheck(this._registration.Port, interval, deregisterFailedAfter);
        }

        public IConsulRegistration AddHttpCheck(int port, TimeSpan interval, TimeSpan deregisterFailedAfter)
        {
            var httpCheck = new AgentServiceCheck
            {
                DockerContainerID = Environment.MachineName,
                DeregisterCriticalServiceAfter = deregisterFailedAfter,
                Interval = interval,
                HTTP = $"{this._registration.Address}:{port}"
            };

            this._healthChecks.Add(httpCheck);

            return this;
        }

        public Task Register()
        {
            this._registration.Checks = this._healthChecks.ToArray();
            return this._client.Agent.ServiceRegister(this._registration);
        }

        public Task Deregister()
        {
            return this._client.Agent.ServiceDeregister(this._registration.ID);
        }



        private string GetServiceID(string name)
        {
            return $"{name}_{Environment.MachineName}";
        }

        private IPAddress GetAddress()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            return addresses.FirstOrDefault(addr => addr.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
