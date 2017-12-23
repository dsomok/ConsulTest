using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal static class ConsulConfigurationExtensions
    {
        public static IConfigurationBuilder AddConsulConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Add(new ConsulConfigurationSource());
            return configurationBuilder;
        }
    }
}