using System;
using Consul;
using ConsulTest.Library.ServiceRegistry;

namespace ConsulTest.Library.Consul.Registration
{
    public interface IConsulServiceRegistrationBuilder
    {
        string ID { get; }
        int Port { get; }
        string Address { get; }
        string Name { get; }

        IConsulServiceRegistrationBuilder WithServiceName(string name);
        IConsulServiceRegistrationBuilder WithPort(int port);
        IConsulServiceRegistrationBuilder WithTTLUpdatingPeriod(TimeSpan period);
        IConsulServiceRegistrationBuilder WithHealthCheck(AgentCheckRegistration healthCheck);
        IServiceRegistration Build();
    }
}
