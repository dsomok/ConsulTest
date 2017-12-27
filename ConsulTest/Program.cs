using Consul;
using ConsulTest.Library;
using ConsulTest.Library.Registration;
using System;

namespace ConsulTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var consulClient = new ConsulClient(config => config.Address = new Uri("http://127.0.0.1:8500"));
            var consulRegistry = new ConsulRegistry(consulClient);

            using (var server = new Server(new[] { "http://*:1234/" }))
            {
                consulRegistry.CreateServiceRegistration("console-host", 1234)
                              .AddHttpCheck(
                                  interval: TimeSpan.FromSeconds(10), 
                                  deregisterFailedAfter: TimeSpan.FromMinutes(2)
                              )
                              .AddTTLCheck(
                                  ttl: TimeSpan.FromSeconds(30), 
                                  interval: TimeSpan.FromSeconds(10), 
                                  deregisterFailedAfter: TimeSpan.FromMinutes(2)
                              )
                              .Register()
                              .Wait();

                Console.WriteLine("Registered service in Consul");

                server.Start();
            }
        }
    }
}
