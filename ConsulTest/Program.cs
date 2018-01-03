using ConsulTest.Library;
using System;
using System.Reflection;
using Autofac;
using ConsulTest.Library.Consul;
using ConsulTest.Library.Consul.Registration;
using ConsulTest.Library.Logging;
using ConsulTest.Library.ServiceRegistry;

namespace ConsulTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = BuildDependencies();

            var consulRegistry = container.Resolve<IServiceRegistry>();
            var registrationBuilder = container.Resolve<IConsulServiceRegistrationBuilder>();
            var registration = registrationBuilder.WithServiceName("console-host")
                                                  .WithPort(1234)
                                                  .AddDefaultHttpCheck()
                                                  .AddDefaultTTLCheck()
                                                  .Build();

            consulRegistry.RegisterServiceAsync(registration).Wait();

            using (var server = container.Resolve<Server>())
            {

                server.Start();
            }
        }


        private static IContainer BuildDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new Server(new[] { "http://*:1234/" }));
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule(new ConsuleModule(new Uri("http://consul:8500")));

            return builder.Build();
        }
    }
}
