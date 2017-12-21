using Consul;
using ConsulTest.Library.Registration;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ConsulTest.Library
{
    public class ConsulRegistry
    {
        private readonly ConsulClient _client;


        public ConsulRegistry()
        {
            _client = new ConsulClient(config =>
            {
                config.Address = new Uri("http://127.0.0.1:8500");
                //config.Address = new Uri("http://172.22.164.38:8500");
            });
        }



        public IConsulRegistration StartServiceRegistration(string name, int port)
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
