using System;
using System.Threading.Tasks;

namespace ConsulTest.Library.Registration
{
    public interface IConsulRegistration
    {
        IConsulRegistration AddHttpHealthCheckEndpoint(int healthCheckPort);
        IConsulRegistration AddHttpCheck(TimeSpan interval, TimeSpan deregisterFailedAfter);
        IConsulRegistration AddHttpCheck(int port, TimeSpan interval, TimeSpan deregisterFailedAfter);

        Task Register();
        Task Deregister();
    }
}
