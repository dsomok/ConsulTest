using Autofac;
using Microsoft.Extensions.Logging;

namespace ConsulTest.Library.Logging
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var loggerFactory = new LoggerFactory().AddConsole(LogLevel.Debug);

            builder.RegisterInstance(loggerFactory).AsImplementedInterfaces().SingleInstance();
        }
    }
}
