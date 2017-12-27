using Consul;
using ConsulTest.Library;
using ConsulTest.Library.Registration;
using System;
using System.Threading;

namespace ConsulTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();

            var consulClient = new ConsulClient(config => config.Address = new Uri("http://consul:8500"));
            var consulRegistry = new ConsulRegistry(consulClient);
            consulRegistry.CreateServiceRegistration("console-client", 80)
                .AddHttpHealthCheckEndpoint(1235)
                .AddHttpCheck(1235, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(2))
                .Register()
                .Wait();

            while (true)
            {
                try
                {
                    var uri = consulRegistry.Discover("console-host").Result;

                    Console.WriteLine($"Quering URI: {uri}");

                    var response = client.Get(uri).Result;

                    Console.WriteLine($"Request succeded: {response}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request failed: {ex.Message}");
                }

                Thread.Sleep(5 * 1000);
            }
        }
    }
}
