using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Consul;
using ConsulTest.Library.ServiceRegistry;
using Microsoft.Extensions.Logging;

namespace ConsulTest.Library.Consul.Registration
{
    internal class ConsulServiceRegistrationBuilder : IConsulServiceRegistrationBuilder
    {
        private readonly ILogger _logger;
        private readonly IList<AgentCheckRegistration> _healthChecks;
        private TimeSpan _ttlUpdatingPeriod = TimeSpan.Zero;

        public ConsulServiceRegistrationBuilder(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<ConsulServiceRegistrationBuilder>();
            this.Address = this.GetAddress();
            this._healthChecks = new List<AgentCheckRegistration>();
        }

        public string ID { get; private set; }
        public int Port { get; private set; }
        public string Address { get; private set; }
        public string Name { get; private set; }

        public IConsulServiceRegistrationBuilder WithServiceName(string name)
        {
            this.ID = $"{name}_{Environment.MachineName}";
            this.Name = name;

            return this;
        }

        public IConsulServiceRegistrationBuilder WithPort(int port)
        {
            this.Port = port;
            return this;
        }

        public IConsulServiceRegistrationBuilder WithTTLUpdatingPeriod(TimeSpan period)
        {
            this._ttlUpdatingPeriod = period;
            return this;
        }

        public IConsulServiceRegistrationBuilder WithHealthCheck(AgentCheckRegistration healthCheck)
        {
            healthCheck.ID = this.GetCheckID(healthCheck);
            this._healthChecks.Add(healthCheck);
            
            this._logger.LogDebug($"Added {this.GetHealthCheckType(healthCheck)} health check.");
            return this;
        }

        public IServiceRegistration Build()
        {
            if (!this.Validate(out string errorMessage))
            {
                throw new InvalidOperationException($"Failed to register service in Consul. Error: {errorMessage}");
            }

            var serviceRegistration = new AgentServiceRegistration
            {
                ID = this.ID,
                Name = this.Name,
                Address = this.Address,
                Port = this.Port,
                Checks = this._healthChecks.ToArray()
            };

            return new ConsulServiceRegistration(serviceRegistration, this._ttlUpdatingPeriod, this._logger);
        }

        private bool Validate(out string errorMessage)
        {
            if (this._healthChecks.Count(check => check.TTL != null) > 1)
            {
                errorMessage = "Multiple TTL checks are not supported";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private string GetCheckID(AgentCheckRegistration healthCheck)
        {
            if (!string.IsNullOrEmpty(healthCheck.ID))
            {
                return healthCheck.ID;
            }

            return $"service:{this.ID}:{this._healthChecks.Count + 1}";
        }

        private string GetAddress()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var ipAddress = addresses.FirstOrDefault(addr => addr.AddressFamily == AddressFamily.InterNetwork);

            return $"http://{ipAddress}";
        }

        private string GetHealthCheckType(AgentCheckRegistration heathCheck)
        {
            if (heathCheck.TTL != null) return "TTL";
            if (heathCheck.HTTP != null) return "HTTP";
            if (heathCheck.TCP != null) return "TCP";
            if (heathCheck.Script != null) return "SCRIPT";
            if (heathCheck.Shell != null) return "DOCKER";

            return "Unknown";
        }
    }
}
