using System;
using Autofac;
using Consul;
using ConsulTest.Library.Consul.Registration;
using Microsoft.Extensions.Logging;

namespace ConsulTest.Library.Consul
{
    public class ConsuleModule : Module
    {
        private readonly Uri _consulEndpoint;



        public ConsuleModule(Uri consulEndpoint)
        {
            _consulEndpoint = consulEndpoint;
        }



        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new ConsulClient(config => config.Address = this._consulEndpoint)).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ConsulServiceRegistrationBuilder>().AsImplementedInterfaces();
            builder.RegisterType<ConsulServiceRegistry>().AsImplementedInterfaces().SingleInstance();

            base.Load(builder);
        }
    }
}
