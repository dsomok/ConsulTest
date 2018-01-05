using System;
using System.Text;
using Consul;

namespace ConsulTest.Configuration
{
    internal class ConsulClientWrapper : IConsulClientWrapper
    {
        private readonly IConsulClient _client;

        public ConsulClientWrapper(IConsulClient client)
        {
            _client = client;
        }

        public string GetKeyValue(string keyName)
        {
            var reply = _client.KV.Get(keyName).Result.Response;
            if (reply.Value == null) return null;
            return Encoding.UTF8.GetString(reply.Value);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public QueryResult<string[]> GetKeys(string prefix)
        {
            return _client.KV.Keys(prefix).Result;
        }
    }

    internal interface IConsulClientWrapper : IDisposable
    {
        string GetKeyValue(string keyName);
        QueryResult<string[]> GetKeys(string prefix);
    }
}