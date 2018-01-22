using System.Collections.Generic;
using System.Linq;
using System.Net;
using Consul;
using ConsulTest.Configuration;
using Moq;
using Shouldly;
using Xunit;

namespace ConsulTest.Tests
{
    public class ConsulConfigurationProviderTest
    {
        private readonly string[] _allDirectories =
        {
            "sym/store/bot/bot574/",
            "sym/store/bot/bot573/",
            "sym/store/bot/",
            "sym/store/",
            "sym/",
            "sym/store/bot/bot573/amqp_uri",
            "sym/store/bot/amqp_uri",
            "sym/store/bot/scheduler_uri",
            "sym/store/store_id",
            "sym/site_id"
        };

        private readonly Dictionary<string, string> _allEntries = new Dictionary<string, string>
        {
            {"sym/store/bot/bot573/amqp_uri", "amqp://bot573"},
            {"sym/store/bot/amqp_uri", "amqp://common"},
            {"sym/store/bot/scheduler_uri", "http://scheduler"},
            {"sym/store/store_id", "3211"},
            {"sym/site_id", "somesite"}
        };

        private IConsulClientWrapper GetClientMock()
        {
            var client = new Mock<IConsulClientWrapper>();
            client.Setup(c => c.GetKeys(It.IsAny<string>()))
                .Returns((string e) =>
                {
                    var directiories = _allDirectories.Where(dir => dir.StartsWith(e)).ToArray();
                    return new QueryResult<string[]>
                    {
                        StatusCode = directiories.Any() ? HttpStatusCode.OK : HttpStatusCode.NotFound,
                        Response = directiories
                    };
                });
            client.Setup(c => c.GetKeyValue(It.IsAny<string>()))
                .Returns((string e) => _allEntries[e]);

            return client.Object;
        }

        [Fact]
        private void Bot573Test()
        {
            var provider = new ConsulConfigurationProvider(() => GetClientMock(), "sym.store.bot.bot573");
            provider.Load();

            provider.TryGet("amqp_uri", out var amqp_uri).ShouldBe(true);
            amqp_uri.ShouldBe("amqp://bot573");

            provider.TryGet("scheduler_uri", out var scheduler_uri).ShouldBe(true);
            scheduler_uri.ShouldBe("http://scheduler");

            provider.TryGet("store_id", out var store_id).ShouldBe(true);
            store_id.ShouldBe("3211");

            provider.TryGet("site_id", out var site_id).ShouldBe(true);
            site_id.ShouldBe("somesite");

            provider.TryGet("another_key", out var another_key).ShouldBe(false);
            another_key.ShouldBe(null);
        }

        [Fact]
        private void Bot574Test()
        {
            var provider = new ConsulConfigurationProvider(() => GetClientMock(), "sym.store.bot.bot574");
            provider.Load();

            provider.TryGet("amqp_uri", out var amqp_uri).ShouldBe(true);
            amqp_uri.ShouldBe("amqp://common");

            provider.TryGet("scheduler_uri", out var scheduler_uri).ShouldBe(true);
            scheduler_uri.ShouldBe("http://scheduler");

            provider.TryGet("store_id", out var store_id).ShouldBe(true);
            store_id.ShouldBe("3211");

            provider.TryGet("site_id", out var site_id).ShouldBe(true);
            site_id.ShouldBe("somesite");

            provider.TryGet("another_key", out var another_key).ShouldBe(false);
            another_key.ShouldBe(null);
        }

        [Fact]
        private void StoreTest()
        {
            var provider = new ConsulConfigurationProvider(() => GetClientMock(), "sym.store");
            provider.Load();

            provider.TryGet("amqp_uri", out var amqp_uri).ShouldBe(false);
            amqp_uri.ShouldBe(null);

            provider.TryGet("scheduler_uri", out var scheduler_uri).ShouldBe(false);
            scheduler_uri.ShouldBe(null);

            provider.TryGet("store_id", out var store_id).ShouldBe(true);
            store_id.ShouldBe("3211");

            provider.TryGet("site_id", out var site_id).ShouldBe(true);
            site_id.ShouldBe("somesite");

            provider.TryGet("another_key", out var another_key).ShouldBe(false);
            another_key.ShouldBe(null);
        }

        [Fact]
        private void TestTuple()
        {
            InnerTupleTool.GetTuple().Item1.ShouldBe("Key");
            ;
        }

        [Fact]
        private void TestLibTuple()
        {
           TupleTool.GetTuple().Item1.ShouldBe("Key");
        }
    }

    public class InnerTupleTool
    {
        public static (string, string) GetTuple()
        {
            return ("Key", "Value");
        }
    }
}