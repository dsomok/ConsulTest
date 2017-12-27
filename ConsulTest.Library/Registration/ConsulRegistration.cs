using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsulTest.Library.Registration
{
    internal class ConsulRegistration : IConsulRegistration
    {
        private readonly AgentServiceRegistration _registration;
        private readonly IConsulClient _client;
        private IList<AgentCheckRegistration> _healthChecks = new List<AgentCheckRegistration>();
        private Timer _ttlStatusUpdater = null;



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



        public string ID => this._registration.ID;

        public int Port => this._registration.Port;

        public string Address => this._registration.Address;



        public void AddHealthCheck(AgentCheckRegistration healthCheck)
        {
            healthCheck.ID = this.GetCheckID(healthCheck);
            this._healthChecks.Add(healthCheck);
        }



        public Task Register()
        {
            if (!this.Validate(out string errorMessage))
            {
                throw new InvalidOperationException($"Failed to register service in Consul. Error: {errorMessage}");
            }

            this.StartTTLStatusUpdater();
            this._registration.Checks = this._healthChecks.ToArray();

            return this._client.Agent.ServiceRegister(this._registration);
        }

        public Task Deregister()
        {
            this._ttlStatusUpdater?.Dispose();
            return this._client.Agent.ServiceDeregister(this._registration.ID);
        }



        protected virtual bool Validate(out string errorMessage)
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

            return $"service:{this._registration.ID}:{this._healthChecks.Count + 1}";
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

        private void StartTTLStatusUpdater()
        {
            var ttlCheck = this._healthChecks.SingleOrDefault(check => check.TTL != null);
            if (ttlCheck != null)
            {
                this._ttlStatusUpdater = new Timer(async state =>
                {
                    try
                    {
                        await this._client.Agent.PassTTL(ttlCheck.ID, "OK");
                    }
                    catch { }
                }, null, TimeSpan.Zero, ttlCheck.Interval.Value);

                ttlCheck.Interval = null;
            }
        }
    }
}
