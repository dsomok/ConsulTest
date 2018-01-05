using System;
using System.Linq;
using Consul;
using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal class ConsulConfigurationSource : IConfigurationSource
    {
        private readonly string _applicationName;
        private readonly Action<ConsulClientConfiguration> _clientConfiguration;

        public ConsulConfigurationSource(Action<ConsulClientConfiguration> clientConfiguration, string applicationName)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (!applicationName.StartsWith(ConsulConfigurationProvider.DEFAULT_PREFIX))
            {
                throw new ArgumentException($"Application name has to start with {ConsulConfigurationProvider.DEFAULT_PREFIX}", nameof(applicationName));
            }

            if (applicationName.Contains('/'))
            {
                throw new ArgumentException("Application name can't contain slashes", nameof(applicationName));
            }

            _clientConfiguration = clientConfiguration;
            _applicationName = applicationName;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var client = new ConsulClient(_clientConfiguration);
            return new ConsulConfigurationProvider(() => new ConsulClientWrapper(client), _applicationName);
        }
    }
}