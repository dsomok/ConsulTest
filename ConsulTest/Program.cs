using ConsulTest.Library;
using ConsulTest.Library.Registration;
using System;

namespace ConsulTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var consulRegistry = new ConsulRegistry("http://consul:8500");

            using (var server = new Server(new[] { "http://*:1234/" }))
            {
                consulRegistry.StartServiceRegistration("console-host", 1234)
                              .AddHttpCheck(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(2))
                              .Register()
                              .Wait();

                Console.WriteLine("Registered service in Consul");

                server.Start();
            }
        }
    }
}
