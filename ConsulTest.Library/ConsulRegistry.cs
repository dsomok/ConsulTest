using Consul;
using ConsulTest.Library.Registration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsulTest.Library
{
    public class ConsulRegistry : IConsulRegistry
    {
        private readonly IConsulClient _client;


        public ConsulRegistry(IConsulClient client)
        {
            this._client = client;
        }



        public IConsulRegistration CreateServiceRegistration(string name, int port)
        {
            return new ConsulRegistration(this._client, name, port);
        }


        public async Task<Uri> Discover(string serviceName)
        {
            var result = await this._client.Health.Service(serviceName, tag: null, passingOnly: true);
            var service = result.Response.FirstOrDefault()?.Service;

            if (service == null)
            {
                throw new Exception("Service not found");
            }

            return new Uri($"{service.Address}:{service.Port}");
        }


    }
}
