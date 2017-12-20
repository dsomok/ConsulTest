using System;
using System.Net.Sockets;

namespace ConsulTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // var consulRegistry = new ConsulRegistry();
            var address = new Uri("http://127.0.0.1:1234");
            using (var server = new Server(address))
            {
                // consulRegistry.AddService("console", 1234).Wait();
                Console.WriteLine("Registered service in Consul");

                server.Start();
            }
        }
    }
}
