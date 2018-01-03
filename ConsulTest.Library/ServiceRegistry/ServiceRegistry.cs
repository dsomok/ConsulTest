using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ConsulTest.Library.ServiceRegistry
{
    public abstract class ServiceRegistry<TRegistration> : IServiceRegistry
        where TRegistration : IServiceRegistration
    {
        protected ServiceRegistry(ILogger logger)
        {
            Logger = logger;
        }

        protected abstract string Name { get; }

        protected ILogger Logger { get; }

        public async Task RegisterServiceAsync(IServiceRegistration serviceRegistration)
        {
            try
            {
                this.Logger.LogDebug($"Registering service {serviceRegistration.Name} in {this.Name}");
                await this.RegisterServiceAsync((TRegistration) serviceRegistration);
            }
            catch
            {
                this.Logger.LogWarning($"Failed to register service {serviceRegistration.Name} in {this.Name}");
            }
        }

        public async Task DeregisterServiceAsync(IServiceRegistration serviceRegistration)
        {
            try
            {
                this.Logger.LogDebug($"Deregistering service {serviceRegistration.Name} from {this.Name}");
                await this.DeregisterServiceAsync((TRegistration) serviceRegistration);
            }
            catch
            {
                this.Logger.LogWarning($"Failed to deregister service {serviceRegistration.Name} from {this.Name}");
            }
        }

        public abstract Task<Uri> Discover(string serviceName);


        protected abstract Task RegisterServiceAsync(TRegistration serviceRegistration);
        protected abstract Task DeregisterServiceAsync(TRegistration serviceRegistration);
    }
}
