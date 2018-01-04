using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using ConsulTest.Library.Consul;
using ConsulTest.Library.Consul.Registration;
using ConsulTest.Library.ServiceRegistry;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace ConsulTest.Tests
{
    public class ConsulServiceRegistryTest
    {
        private readonly AgentServiceRegistration _agentServiceRegistration;
        private readonly ILoggerFactory _loggerFactory;

        public ConsulServiceRegistryTest()
        {
            this._agentServiceRegistration = new AgentServiceRegistration
            {
                ID = "service1",
                Name = "Test",
                Port = 80,
                Address = "http://127.0.0.1",
                Checks = new[]
                {
                    new AgentCheckRegistration
                    {
                        ID = "check1",
                        Name = "TTL Check",
                        TTL = TimeSpan.FromMinutes(2)
                    }
                }
            };
            this._loggerFactory = new LoggerFactory();
        }

        [Fact]
        public async Task ServiceRegistrationProperlyTest()
        {
            var clientMock = new Mock<IConsulClient>();
            clientMock
                .Setup(c => c.Agent.ServiceRegister(It.IsAny<AgentServiceRegistration>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new WriteResult {StatusCode = HttpStatusCode.OK}));

            IServiceRegistry registry = new ConsulServiceRegistry(clientMock.Object, this._loggerFactory);

            var registration = new ConsulServiceRegistration(this._agentServiceRegistration, TimeSpan.FromSeconds(10),
                this._loggerFactory.CreateLogger<ConsulServiceRegistryTest>());

            var exceptions = await Record.ExceptionAsync(() => registry.RegisterServiceAsync(registration));

            exceptions.ShouldBeNull();
            clientMock.Verify(client => client.Agent.ServiceRegister(this._agentServiceRegistration, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task ServiceRegistrationConsulNotAccessible()
        {
            var clientMock = new Mock<IConsulClient>();
            clientMock
                .Setup(c => c.Agent.ServiceRegister(It.IsAny<AgentServiceRegistration>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new WriteResult { StatusCode = HttpStatusCode.NotFound }));

            IServiceRegistry registry = new ConsulServiceRegistry(clientMock.Object, this._loggerFactory);

            var registration = new ConsulServiceRegistration(this._agentServiceRegistration, TimeSpan.FromSeconds(10),
                this._loggerFactory.CreateLogger<ConsulServiceRegistryTest>());

            var exceptions = await Record.ExceptionAsync(() => registry.RegisterServiceAsync(registration));

            exceptions.ShouldBeNull();
            clientMock.Verify(client => client.Agent.ServiceRegister(this._agentServiceRegistration, default(CancellationToken)), Times.Once);
        }
    }
}
