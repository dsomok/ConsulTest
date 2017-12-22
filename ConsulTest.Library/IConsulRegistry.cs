using System;
using System.Threading.Tasks;
using ConsulTest.Library.Registration;

namespace ConsulTest.Library
{
    public interface IConsulRegistry
    {
        IConsulRegistration CreateServiceRegistration(string serviceName, int port);
        Task<Uri> Discover(string serviceName);
    }
}