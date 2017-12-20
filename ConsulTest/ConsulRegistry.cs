using Consul;
using System;
using System.Threading.Tasks;

namespace ConsulTest
{
    internal class ConsulRegistry
    {
        private readonly ConsulClient _client;



        public ConsulRegistry()
        {
            _client = new ConsulClient();
        }



        public Task AddService(string name, int port)
        {
            //var httpCheck = new AgentServiceCheck
            //{
            //    DockerContainerID = Environment.MachineName,
            //    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
            //    Interval = TimeSpan.FromSeconds(10),
            //    HTTP = $"http://app:{port}",
                
            //};

            var registration = new AgentServiceRegistration
            {
                //Check = httpCheck,
                Name = name,
                Port = port
            };

            return this._client.Agent.ServiceRegister(registration);
        }
    }
}
