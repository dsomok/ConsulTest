using System;
using System.Text;
using Consul;
using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal class ConsulConfigurationProvider : ConfigurationProvider
    {
        private readonly Action<ConsulClientConfiguration> _clientConfiguration;

        public ConsulConfigurationProvider(Action<ConsulClientConfiguration> clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
        }

        public override void Load()
        {
            using (var consulClient = new ConsulClient(_clientConfiguration))
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