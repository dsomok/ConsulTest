using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Consul;
using ConsulTest.Library.Consul.Registration;
using ConsulTest.Library.ServiceRegistry;
using ConsulTest.Library.ServiceRegistry.Exceptions;
using Microsoft.Extensions.Logging;

namespace ConsulTest.Library.Consul
{
    internal class ConsulServiceRegistry : ServiceRegistry<ConsulServiceRegistration>
    {
        private readonly IConsulClient _client;
        private readonly ILogger _logger;



        public ConsulServiceRegistry(IConsulClient client, ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<ConsulServiceRegistry>())
        {
            _client = client;
        }



        protected override string Name => "Consul";



        public override async Task<Uri> Discover(string serviceName)
        {
            var result = await this._client.Health.Service(serviceName, tag: null, passingOnly: true);
            ServiceEntry first = null;
            foreach (var entry in result.Response)
            {
                first = entry;
                break;
            }

            var service = first?.Service;

            if (service == null)
            {
                throw new Exception("Service not found");
            }

            return new Uri($"{service.Address}:{service.Port}");
        }

        protected override async Task RegisterServiceAsync(ConsulServiceRegistration serviceRegistration)
        {
            var result = await this._client.Agent.ServiceRegister(serviceRegistration.Registration);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new RegistryNotAccessibleException();
            }

            serviceRegistration.StartTTLUpdater(checkId => this._client.Agent.PassTTL(checkId, "OK"));
        }

        protected override async Task DeregisterServiceAsync(ConsulServiceRegistration serviceRegistration)
        {
            var result = await this._client.Agent.ServiceDeregister(serviceRegistration.ID);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new RegistryNotAccessibleException();
            }

            serviceRegistration.StopTTLUpdater();
        }
    }
}
