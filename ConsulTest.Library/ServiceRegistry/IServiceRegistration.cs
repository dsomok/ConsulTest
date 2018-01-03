namespace ConsulTest.Library.ServiceRegistry
{
    public interface IServiceRegistration
    {
        string ID { get; }
        int Port { get; }
        string Address { get; }
        string Name { get; }
    }
}
