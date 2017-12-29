using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consul;
using Microsoft.Extensions.Configuration;

namespace ConsulTest.Configuration
{
    internal class ConsulConfigurationProvider : ConfigurationProvider
    {
        private readonly Action<ConsulClientConfiguration> _clientConfiguration;

        public ConsulConfigurationProvider(Action<ConsulClientConfiguration> clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
        }

        public override void Load()
        {
            using (var consulClient = new ConsulClient(_clientConfiguration))
            {
                var keys = consulClient.KV.Keys("sym").Result;

                foreach (var key in GetRelevantKeys("sym.store.bot.bot574", keys.Response))
                {
                    var configEntry = consulClient.KV.Get(key.KeyPath).Result.Response;
                    Data[key.KeyName] = Encoding.UTF8.GetString(configEntry.Value);
                }
            }
        }

        private List<KVEntry> GetRelevantKeys(string applicationName, string[] allEntries)
        {
            var keys = allEntries.Where(k => !k.EndsWith('/')).ToArray();
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
                if (relevantKeys.All(k => k.KeyName != keyName))
                {
                    relevantKeys.Add(new KVEntry(){KeyPath = key, KeyName = keyName});
                }
            }
            var parent = GetParentDirectory(root);
            if (parent == null)
                return;
            GetKeysRecursive(parent, keys, relevantKeys);
        }

        private IEnumerable<string> GetDirectoryEntries(string directory, string[] allKeys)
        {
            foreach (var key in allKeys)
            {
                if (key.StartsWith(directory) && key.Count(c => c == '/') == directory.Count(c => c == '/') + 1)
                    yield return key;
            }
        }

        private string GetParentDirectory(string directory)
        {
            if (directory.EndsWith('/'))
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