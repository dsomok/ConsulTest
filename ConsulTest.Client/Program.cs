using System;
using System.Threading;
using Autofac;
using ConsulTest.Library;
using ConsulTest.Library.Consul;
using ConsulTest.Library.Consul.Registration;
using ConsulTest.Library.Logging;
using ConsulTest.Library.ServiceRegistry;

namespace ConsulTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = BuildDependencies();

            var client = container.Resolve<Client>();
            
            var consulRegistry = container.Resolve<IServiceRegistry>();
            var registrationBuilder = container.Resolve<IConsulServiceRegistrationBuilder>();
            var registration = registrationBuilder.WithServiceName("console-client")
                                                  .WithPort(1235)
                                                  .AddHttpHealthCheckEndpoint(1235)
                                                  .AddDefaultHttpCheck()
                                                  .Build();

            consulRegistry.RegisterServiceAsync(registration).Wait();

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


        private static IContainer BuildDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Client>();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule(new ConsuleModule(new Uri("http://consul:8500")));

            return builder.Build();
        }
    }
}
