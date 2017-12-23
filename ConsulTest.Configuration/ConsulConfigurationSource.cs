using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal class ConsulConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return (IConfigurationProvider) new ConsulConfigurationProvider();
        }
    }
}