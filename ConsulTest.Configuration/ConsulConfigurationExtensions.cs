using System;
using Consul;
using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal static class ConsulConfigurationExtensions
    {
        public static IConfigurationBuilder AddConsulConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Add(new ConsulConfigurationSource(c => { }));
            return configurationBuilder;
        }

        public static IConfigurationBuilder AddConsulConfiguration(this IConfigurationBuilder configurationBuilder, Action<ConsulClientConfiguration> clientConfiguration)
        {
            configurationBuilder.Add(new ConsulConfigurationSource(clientConfiguration));
            return configurationBuilder;
        }
    }
}