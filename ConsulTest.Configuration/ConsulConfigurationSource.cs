using System;
using Consul;
using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal class ConsulConfigurationSource : IConfigurationSource
    {
        private readonly Action<ConsulClientConfiguration> _clientConfiguration;

        public ConsulConfigurationSource(Action<ConsulClientConfiguration> clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return (IConfigurationProvider) new ConsulConfigurationProvider(_clientConfiguration);
        }
    }
}