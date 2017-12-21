using Consul;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsulTest.Library.Registration
{
    public interface IConsulRegistration
    {
        ConsulClient Client { get; }
        AgentServiceRegistration Registration { get; }
        IList<AgentServiceCheck> Checks { get; }

        Task Register();
        Task Deregister();
    }
}
