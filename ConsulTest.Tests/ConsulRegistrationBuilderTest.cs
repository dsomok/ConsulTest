using System;
using Consul;
using ConsulTest.Library.Consul.Registration;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace ConsulTest.Tests
{
    public class ConsulRegistrationBuilderTest
    {
        private readonly ILoggerFactory _loggerFactory;

        public ConsulRegistrationBuilderTest()
        {
            this._loggerFactory = new LoggerFactory();
        }

        [Fact]
        public void WithServiceNameTest()
        {
            var registrationBuilder = new ConsulServiceRegistrationBuilder(this._loggerFactory);

            var serviceName = "test";
            var id = $"{serviceName}_{Environment.MachineName}";
            registrationBuilder.WithServiceName(serviceName);

            registrationBuilder.ID.ShouldBe(id);
            registrationBuilder.Name.ShouldBe(serviceName);

            var registration = registrationBuilder.Build();
            registration.ShouldNotBeNull();
            registration.ID.ShouldBe(id);
            registration.Name.ShouldBe(serviceName);

            var consulRegistration = registration as ConsulServiceRegistration;
            consulRegistration.ShouldNotBeNull();
            consulRegistration.Registration.ID.ShouldBe(id);
            consulRegistration.Registration.Name.ShouldBe(serviceName);
        }

        [Fact]
        public void WithPortTest()
        {
            var registrationBuilder = new ConsulServiceRegistrationBuilder(this._loggerFactory);

            var port = 123;
            registrationBuilder.WithPort(port);

            registrationBuilder.Port.ShouldBe(port);

            var registration = registrationBuilder.Build();
            registration.ShouldNotBeNull();
            registration.Port.ShouldBe(port);

            var consulRegistration = registration as ConsulServiceRegistration;
            consulRegistration.ShouldNotBeNull();
            consulRegistration.Registration.Port.ShouldBe(port);
        }

        [Fact]
        public void WithHealthCheckTest()
        {
            var registrationBuilder = new ConsulServiceRegistrationBuilder(this._loggerFactory);

            var serviceName = "test";
            var id = $"{serviceName}_{Environment.MachineName}";

            var httpCheck = new AgentCheckRegistration
            {
                ID = "http_check",
                Name = "HTTP Check",
                HTTP = "http://127.0.0.1",
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(2)
            };

            var ttlCheck = new AgentCheckRegistration
            {
                Name = "TTL Check",
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(2),
                TTL = TimeSpan.FromSeconds(30)
            };

            registrationBuilder.WithServiceName(serviceName);
            registrationBuilder.WithHealthCheck(httpCheck);
            registrationBuilder.WithHealthCheck(ttlCheck);

            var registration = registrationBuilder.Build();
            registration.ShouldNotBeNull();

            var consulRegistration = registration as ConsulServiceRegistration;
            consulRegistration.ShouldNotBeNull();
            consulRegistration.Registration.Checks.Length.ShouldBe(2);

            consulRegistration.Registration.Checks[0].ShouldNotBeNull();
            var httpRegistrationCheck = consulRegistration.Registration.Checks[0] as AgentCheckRegistration;
            httpRegistrationCheck.ShouldNotBeNull();
            httpRegistrationCheck.ID.ShouldBe(httpCheck.ID);
            httpRegistrationCheck.Name.ShouldBe(httpCheck.Name);
            httpRegistrationCheck.HTTP.ShouldBe(httpCheck.HTTP);

            consulRegistration.Registration.Checks[1].ShouldNotBeNull();
            var ttlRegistrationCheck = consulRegistration.Registration.Checks[1] as AgentCheckRegistration;
            ttlRegistrationCheck.ShouldNotBeNull();
            ttlRegistrationCheck.ID.ShouldBe($"service:{id}:2");
            ttlRegistrationCheck.Name.ShouldBe(ttlCheck.Name);
            ttlRegistrationCheck.TTL.ShouldBe(ttlCheck.TTL);
        }
    }
}
