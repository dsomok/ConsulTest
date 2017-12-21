using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ConsulTest.Library.Registration
{
    internal class ConsulRegistration : IConsulRegistration
    {
        public ConsulRegistration(ConsulClient client, string serviceName, int port)
        {
            this.Checks = new List<AgentServiceCheck>();
            this.Client = client;

            var address = this.GetAddress();
            this.Registration = new AgentServiceRegistration
            {
                ID = this.GetServiceID(serviceName),
                Name = serviceName,
                Address = $"http://{address}",
                Port = port
            };
        }



        public AgentServiceRegistration Registration { get; }

        public ConsulClient Client { get; }

        public IList<AgentServiceCheck> Checks { get; }



        public Task Register()
        {
            this.Registration.Checks = this.Checks.ToArray();
            return this.Client.Agent.ServiceRegister(this.Registration);
        }

        public Task Deregister()
        {
            return this.Client.Agent.ServiceDeregister(this.Registration.ID);
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
