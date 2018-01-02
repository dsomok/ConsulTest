using System;
using Consul;
using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal static class ConsulConfigurationExtensions
    {
        public static IConfigurationBuilder AddConsulConfiguration(this IConfigurationBuilder configurationBuilder, string applicationName)
        {
            configurationBuilder.Add(new ConsulConfigurationSource(c => { }, applicationName));
            return configurationBuilder;
        }

        public static IConfigurationBuilder AddConsulConfiguration(this IConfigurationBuilder configurationBuilder, string applicationName, 
            Action<ConsulClientConfiguration> clientConfiguration)
        {
            configurationBuilder.Add(new ConsulConfigurationSource(clientConfiguration, applicationName));
            return configurationBuilder;
        }
    }
}