using Consul;
using System;
using System.Threading.Tasks;

namespace ConsulTest.Library.Registration
{
    public interface IConsulRegistration
    {
        string ID { get; }
        int Port { get; }
        string Address { get; }

        void AddHealthCheck(AgentCheckRegistration healthCheck);

        Task Register();
        Task Deregister();
    }
}
