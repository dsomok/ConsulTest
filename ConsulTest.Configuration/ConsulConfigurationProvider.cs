using System;
using System.Text;
using Consul;
using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal class ConsulConfigurationProvider : ConfigurationProvider
    {
        public override void Load()
        {
            using (var consulClient = new ConsulClient(config => config.Address = new Uri("http://localhost:8500")))
            {
                var response = consulClient.KV.List("").Result.Response;
                if (response == null)
                    return;
                foreach (var kvPair in response)
                    Data[kvPair.Key] = Encoding.UTF8.GetString(kvPair.Value);
            }
        }
    }
}