using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationRoot = ConsulConfiguration.Create(args);
            Console.WriteLine($"foo = {configurationRoot["foo"]}");
            Console.ReadKey();
        }
    }

    public static class ConsulConfiguration
    {
        public static IConfigurationRoot Create(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .AddCommandLine(args, new Dictionary<string, string>() { { "-foo", "foo" } })
                    .AddEnvironmentVariables()
                    .AddConsulConfiguration("sym.store.bot.bot574")
                ;
            return builder.Build();
        }
    }
}
