using Consul;
using ConsulTest.Library;
using System;

namespace ConsulTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var consulClient = new ConsulClient(config => config.Address = new Uri("http://consul:8500"));
            var consulRegistry = new ConsulRegistry(consulClient);

            using (var server = new Server(new[] { "http://*:1234/" }))
            {
                consulRegistry.CreateServiceRegistration("console-host", 1234)
                              .AddHttpCheck(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(2))
                              .Register()
                              .Wait();

                Console.WriteLine("Registered service in Consul");

                server.Start();
            }
        }
    }
}
