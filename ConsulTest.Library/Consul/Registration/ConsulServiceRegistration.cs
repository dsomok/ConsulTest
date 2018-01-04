using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using ConsulTest.Library.ServiceRegistry;
using Microsoft.Extensions.Logging;

namespace ConsulTest.Library.Consul.Registration
{
    internal class ConsulServiceRegistration : IServiceRegistration
    {
        private readonly TimeSpan _ttlPeriod;
        private readonly ILogger _logger;
        private Timer _ttlStatusUpdater;

        public ConsulServiceRegistration(AgentServiceRegistration registration, TimeSpan ttlPeriod, ILogger logger)
        {
            this._ttlPeriod = ttlPeriod;
            this._logger = logger;
            this.Registration = registration;
        }

        internal AgentServiceRegistration Registration { get; }

        public string ID => this.Registration.ID;
        public int Port => this.Registration.Port;
        public string Address => this.Registration.Address;
        public string Name => this.Registration.Name;

        public void StartTTLUpdater(Func<string, Task> ttlPassCallback)
        {
            var ttlCheck = this.Registration.Checks?.Cast<AgentCheckRegistration>().SingleOrDefault(check => check.TTL != null);
            if (ttlCheck != null)
            {
                this._ttlStatusUpdater = new Timer(async state =>
                {
                    try
                    {
                        await ttlPassCallback(ttlCheck.ID);
                    }
                    catch(Exception ex)
                    {
                        this._logger.LogWarning(ex, "Failed to update TTL status in service registry");
                    }
                }, null, TimeSpan.Zero, this._ttlPeriod);
            }
        }

        public void StopTTLUpdater()
        {
            this._ttlStatusUpdater.Dispose();
        }
    }
}
