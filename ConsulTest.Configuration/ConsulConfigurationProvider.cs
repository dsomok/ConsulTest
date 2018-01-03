using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Consul;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ConsulTest.Configuration
{
    internal class ConsulConfigurationProvider : ConfigurationProvider
    {
        internal const string DEFAULT_PREFIX = "sym";
        private readonly string _applicationName;
        private readonly Action<ConsulClientConfiguration> _clientConfiguration;
        private readonly ILogger _logger;

        public ConsulConfigurationProvider(Action<ConsulClientConfiguration> clientConfiguration, string applicationName)
        {
            _clientConfiguration = clientConfiguration;
            _applicationName = applicationName;
            _logger = Log.Logger;
        }

        public override void Load()
        {
            using (var consulClient = new ConsulClient(_clientConfiguration))
            {
                var reply = consulClient.KV.Keys(DEFAULT_PREFIX).Result;

                if (reply.StatusCode != HttpStatusCode.OK)
                {
                    _logger.Warning("Error reading configuration from consul, unexpeced HTTP response {HTTPResponseCode}", reply.StatusCode);
                    return;
                }

                var releavantKeys = GetRelevantKeys(_applicationName, reply.Response);
                foreach (var key in releavantKeys)
                {
                    var configEntry = GetKeyValue(consulClient, key.KeyPath);
                    _logger.Debug("Loaded configuration setting from Consul: {keyName} : {keyValue}", key.KeyName, configEntry);
                    Data[key.KeyName] = configEntry;
                }
            }
        }

        private string GetKeyValue(ConsulClient client, string keyName)
        {
            var reply = client.KV.Get(keyName).Result.Response;
            if (reply.Value == null) return null;
            return Encoding.UTF8.GetString(reply.Value);
        }

        private List<KVEntry> GetRelevantKeys(string applicationName, string[] allEntries)
        {
            var keys = allEntries.Where(k => !k.EndsWith("/")).ToArray();
            var applicationFolder = applicationName.Replace('.', '/');
            var relevantKeys = new List<KVEntry>();
            GetKeysRecursive(applicationFolder, keys, relevantKeys);
            return relevantKeys;
        }

        private void GetKeysRecursive(string root, string[] keys, List<KVEntry> relevantKeys)
        {
            foreach (var key in GetDirectoryEntries(root, keys))
            {
                var keyName = key.Substring(key.LastIndexOf('/') + 1);
                if (relevantKeys.All(k => k.KeyName != keyName)) relevantKeys.Add(new KVEntry { KeyPath = key, KeyName = keyName });
            }

            var parent = GetParentDirectory(root);
            if (parent == null)
                return;
            GetKeysRecursive(parent, keys, relevantKeys);
        }

        private IEnumerable<string> GetDirectoryEntries(string directory, string[] allKeys)
        {
            foreach (var key in allKeys)
                if (key.StartsWith(directory) && key.Count(c => c == '/') == directory.Count(c => c == '/') + 1)
                    yield return key;
        }

        private string GetParentDirectory(string directory)
        {
            if (directory.EndsWith("/"))
                directory = directory.Substring(0, directory.Length - 1);
            if (!directory.Contains('/'))
                return null;
            return directory.Substring(0, directory.LastIndexOf('/'));
        }

        private class KVEntry
        {
            public string KeyName { get; set; }
            public string KeyPath { get; set; }
        }
    }
}