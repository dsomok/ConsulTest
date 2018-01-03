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
    }

    //internal class ConsulServiceRegistration : IConsulServiceRegistration
    //{
    //    private readonly AgentServiceRegistration _registration;
    //    private readonly IConsulClient _client;
    //    private readonly IList<AgentCheckRegistration> _healthChecks = new List<AgentCheckRegistration>();
    //    private Timer _ttlStatusUpdater = null;

    //    public ConsulServiceRegistration(IConsulClient client, string serviceName, int port)
    //    {
    //        this._client = client;

    //        var address = this.GetAddress();
    //        this._registration = new AgentServiceRegistration
    //        {
    //            ID = this.GetServiceID(serviceName),
    //            Name = serviceName,
    //            Address = $"http://{address}",
    //            Port = port
    //        };
    //    }

    //    public string ID => this._registration.ID;
    //    public int Port => this._registration.Port;
    //    public string Address => this._registration.Address;
    //    public string Name => this._registration.Name;

    //    public void AddHealthCheck(AgentCheckRegistration healthCheck)
    //    {
    //        healthCheck.ID = this.GetCheckID(healthCheck);
    //        this._healthChecks.Add(healthCheck);
    //    }

    //    public Task Register()
    //    {
    //        if (!this.Validate(out string errorMessage))
    //        {
    //            throw new InvalidOperationException($"Failed to register service in Consul. Error: {errorMessage}");
    //        }

    //        this.StartTTLStatusUpdater();
    //        this._registration.Checks = this._healthChecks.ToArray();

    //        return this._client.Agent.ServiceRegister(this._registration);
    //    }

    //    public Task Deregister()
    //    {
    //        this._ttlStatusUpdater?.Dispose();
    //        return this._client.Agent.ServiceDeregister(this._registration.ID);
    //    }

    //    protected virtual bool Validate(out string errorMessage)
    //    {
    //        if (this._healthChecks.Count(check => check.TTL != null) > 1)
    //        {
    //            errorMessage = "Multiple TTL checks are not supported";
    //            return false;
    //        }

    //        errorMessage = string.Empty;
    //        return true;
    //    }

    //    private string GetCheckID(AgentCheckRegistration healthCheck)
    //    {
    //        if (!string.IsNullOrEmpty(healthCheck.ID))
    //        {
    //            return healthCheck.ID;
    //        }

    //        return $"service:{this._registration.ID}:{this._healthChecks.Count + 1}";
    //    }

    //    private string GetServiceID(string name)
    //    {
    //        return $"{name}_{Environment.MachineName}";
    //    }

    //    private IPAddress GetAddress()
    //    {
    //        var addresses = Dns.GetHostAddresses(Dns.GetHostName());
    //        return addresses.FirstOrDefault(addr => addr.AddressFamily == AddressFamily.InterNetwork);
    //    }

    //    private void StartTTLStatusUpdater()
    //    {
    //        var ttlCheck = this._healthChecks.SingleOrDefault(check => check.TTL != null);
    //        if (ttlCheck != null)
    //        {
    //            this._ttlStatusUpdater = new Timer(async state =>
    //            {
    //                try
    //                {
    //                    await this._client.Agent.PassTTL(ttlCheck.ID, "OK");
    //                }
    //                catch
    //                {
    //                }
    //            }, null, TimeSpan.Zero, ttlCheck.Interval.Value);

    //            ttlCheck.Interval = null;
    //        }
    //    }
    //}
}
