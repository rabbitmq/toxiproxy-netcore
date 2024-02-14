using System.Linq;
using System.Threading.Tasks;
using Toxiproxy.Net;
using Toxiproxy.Net.Toxics;
using Xunit;

namespace ToxiproxyNetCore.Tests
{
    [Collection("Integration")]
    public class ProxyTests : ToxiproxyTestsBase
    {
        public ProxyTests(ConnectionFixture fixture) : base(fixture) { }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task GetAllToxicsFromAProxyShouldWork(ToxicDirection toxicDirection)
        {
            // Add two toxics to a proxy and check if they are present in the list
            // of the toxies for the given proxy
            var client = Fixture.Client;
            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var slicerToxic = new SlicerToxic
            {
                Name = "SlicerToxicTest",
                Stream = toxicDirection,
            };
            slicerToxic.Attributes.AverageSize = 10;
            slicerToxic.Attributes.Delay = 5;
            slicerToxic.Attributes.SizeVariation = 1;
            await newProxy.AddAsync(slicerToxic);

            var slowCloseToxic = new SlowCloseToxic
            {
                Name = "slowCloseToxic",
                Stream = ToxicDirection.DownStream,
                Toxicity = 80
            };
            slowCloseToxic.Attributes.Delay = 50;
            await newProxy.AddAsync(slowCloseToxic);

            // Retrieve the proxy and check the toxics
            var toxics = await newProxy.GetAllToxicsAsync();
            Assert.Equal(2, toxics.Count());

            var slicerToxicInTheProxy = toxics.OfType<SlicerToxic>().Single();
            Assert.Equal(slicerToxic.Name, slicerToxicInTheProxy.Name);
            Assert.Equal(slicerToxic.Stream, slicerToxicInTheProxy.Stream);
            Assert.Equal(slicerToxic.Attributes.AverageSize, slicerToxicInTheProxy.Attributes.AverageSize);
            Assert.Equal(slicerToxic.Attributes.Delay, slicerToxicInTheProxy.Attributes.Delay);
            Assert.Equal(slicerToxic.Attributes.SizeVariation, slicerToxicInTheProxy.Attributes.SizeVariation);

            var slowCloseToxicInTheProxy = toxics.OfType<SlowCloseToxic>().Single();
            Assert.Equal(slowCloseToxic.Name, slowCloseToxicInTheProxy.Name);
            Assert.Equal(slowCloseToxic.Stream, slowCloseToxicInTheProxy.Stream);
            Assert.Equal(slowCloseToxic.Attributes.Delay, slowCloseToxicInTheProxy.Attributes.Delay);
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task CreateANewLatencyToxicShouldWork(ToxicDirection toxicDirection)
        {
            var client = Fixture.Client;

            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var toxic = new LatencyToxic
            {
                Name = "LatencyToxicTest",
                Stream = toxicDirection
            };
            toxic.Attributes.Jitter = 10;
            toxic.Attributes.Latency = 5;
            var newToxic = await newProxy.AddAsync(toxic);

            // Need to retrieve the proxy and check the toxic's values
            Assert.Equal(toxic.Name, newToxic.Name);
            Assert.Equal(toxic.Stream, newToxic.Stream);
            Assert.Equal(toxic.Attributes.Jitter, newToxic.Attributes.Jitter);
            Assert.Equal(toxic.Attributes.Latency, newToxic.Attributes.Latency);
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task CreateANewSlowCloseToxicShouldWork(ToxicDirection toxicDirection)
        {
            var client = Fixture.Client;

            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var toxic = new SlowCloseToxic
            {
                Name = "SlowCloseToxicTest",
                Stream = toxicDirection,
                Toxicity = 0.5
            };
            toxic.Attributes.Delay = 100;
            var newToxic = await newProxy.AddAsync(toxic);

            // Need to retrieve the proxy and check the toxic's values
            Assert.Equal(toxic.Name, newToxic.Name);
            Assert.Equal(toxic.Stream, newToxic.Stream);
            Assert.Equal(toxic.Attributes.Delay, newToxic.Attributes.Delay);
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task CreateANewTimeoutToxicShouldWork(ToxicDirection toxicDirection)
        {
            var client = Fixture.Client;

            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var toxic = new TimeoutToxic
            {
                Name = "TimeoutToxicTest",
                Stream = toxicDirection,
                Toxicity = 0.5
            };
            toxic.Attributes.Timeout = 10;
            var newToxic = await newProxy.AddAsync(toxic);

            // Need to retrieve the proxy and check the toxic's values
            Assert.Equal(toxic.Name, newToxic.Name);
            Assert.Equal(toxic.Stream, newToxic.Stream);
            Assert.Equal(toxic.Attributes.Timeout, newToxic.Attributes.Timeout);
        }

        [Fact]
        public async Task CreateANewResetPeerToxicShouldWork()
        {
            var client = Fixture.Client;

            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var toxic = new ResetPeerToxic
            {
                Name = "ResetPeerToxicTest",
            };
            toxic.Attributes.Timeout = 5;
            var newToxic = await newProxy.AddAsync(toxic);

            // Need to retrieve the proxy and check the toxic's values
            Assert.Equal(toxic.Name, newToxic.Name);
            Assert.Equal(toxic.Attributes.Timeout, newToxic.Attributes.Timeout);

            // Check against current toxics in the proxy
            var currentToxics = await newProxy.GetAllToxicsAsync();
            Assert.Contains(currentToxics, t => t.Name == toxic.Name && t.Type == "reset_peer");
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task CreateANewBandwidthToxicShouldWork(ToxicDirection toxicDirection)
        {
            var client = Fixture.Client;
            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var toxic = new BandwidthToxic
            {
                Name = "BandwidthToxicTest",
                Stream = toxicDirection
            };
            toxic.Attributes.Rate = 100;
            var newToxic = await newProxy.AddAsync(toxic);

            // Need to retrieve the proxy and check the toxic's values
            Assert.Equal(toxic.Name, newToxic.Name);
            Assert.Equal(toxic.Stream, newToxic.Stream);
            Assert.Equal(toxic.Attributes.Rate, newToxic.Attributes.Rate);
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task CreateANewSlicerToxicShouldWork(ToxicDirection toxicDirection)
        {
            var client = Fixture.Client;
            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var toxic = new SlicerToxic
            {
                Name = "SlicerToxicTest",
                Stream = toxicDirection
            };
            toxic.Attributes.AverageSize = 10;
            toxic.Attributes.Delay = 5;
            toxic.Attributes.SizeVariation = 1;
            var newToxic = await newProxy.AddAsync(toxic);

            // Need to retrieve the proxy and check the toxic's values
            Assert.Equal(toxic.Name, newToxic.Name);
            Assert.Equal(toxic.Stream, newToxic.Stream);
            Assert.Equal(toxic.Attributes.AverageSize, newToxic.Attributes.AverageSize);
            Assert.Equal(toxic.Attributes.Delay, newToxic.Attributes.Delay);
            Assert.Equal(toxic.Attributes.SizeVariation, newToxic.Attributes.SizeVariation);
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task CreateANewLimitDataToxicShouldWork(ToxicDirection toxicDirection)
        {
            var client = Fixture.Client;
            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var toxic = new LimitDataToxic
            {
                Name = "LimitDataToxicTest",
                Stream = toxicDirection
            };
            toxic.Attributes.Bytes = 512;
            var newToxic = await newProxy.AddAsync(toxic);

            // Need to retrieve the proxy and check the toxic's values
            Assert.Equal(toxic.Name, newToxic.Name);
            Assert.Equal(toxic.Stream, newToxic.Stream);
            Assert.Equal(toxic.Attributes.Bytes, newToxic.Attributes.Bytes);
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task AddTwoToxicWithTheSameNameShouldThrowException(ToxicDirection toxicDirection)
        {
            var client = Fixture.Client;
            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var firstToxic = new SlicerToxic
            {
                Name = "SlicerToxicTest",
                Stream = toxicDirection
            };
            firstToxic.Attributes.AverageSize = 10;
            firstToxic.Attributes.Delay = 5;
            firstToxic.Attributes.SizeVariation = 1;
            await newProxy.AddAsync(firstToxic);

            var toxicWithSameName = new SlicerToxic
            {
                Name = firstToxic.Name,
                Stream = toxicDirection
            };

            await Assert.ThrowsAsync<ToxiProxiException>(
                async () => await newProxy.AddAsync(toxicWithSameName));
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task GetAnExistingToxicFromAProxyShouldWork(ToxicDirection toxicDirection)
        {
            // Add a toxics to a proxy.
            // After reload the toxic again and check that all the properties
            // are correctly saved 
            var client = Fixture.Client;
            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            proxy = await client.AddAsync(proxy);

            var toxic = new SlicerToxic
            {
                Name = "SlicerToxicTest",
                Stream = toxicDirection
            };
            toxic.Attributes.AverageSize = 10;
            toxic.Attributes.Delay = 5;
            toxic.Attributes.SizeVariation = 1;
            toxic = await proxy.AddAsync(toxic);

            // Reload the toxic and update the properties
            var toxicInProxy = await proxy.GetToxicByNameAsync(toxic.Name);

            // Assert
            Assert.Equal(toxicInProxy.Name, toxic.Name);
            Assert.Equal(toxicInProxy.Stream, toxic.Stream);
            Assert.IsType<SlicerToxic>(toxicInProxy);
            var specificToxicInProxy = (SlicerToxic)toxicInProxy;
            Assert.Equal(specificToxicInProxy.Attributes.AverageSize, toxic.Attributes.AverageSize);
            Assert.Equal(specificToxicInProxy.Attributes.Delay, toxic.Attributes.Delay);
            Assert.Equal(specificToxicInProxy.Attributes.SizeVariation, toxic.Attributes.SizeVariation);
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream)]
        [InlineData(ToxicDirection.DownStream)]
        public async Task DeleteAToxicShouldWork(ToxicDirection toxicDirection)
        {
            // Add two toxics to a proxy.
            // After delete the first one and check that
            // there is still the second toxic in the proxy
            var client = Fixture.Client;
            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            var newProxy = await client.AddAsync(proxy);

            var firstToxic = new SlicerToxic
            {
                Name = "SlicerToxicTest",
                Stream = toxicDirection
            };
            firstToxic.Attributes.AverageSize = 10;
            firstToxic.Attributes.Delay = 5;
            firstToxic.Attributes.SizeVariation = 1;
            await newProxy.AddAsync(firstToxic);

            var secondToxic = new SlowCloseToxic
            {
                Name = "slowCloseToxic",
                Stream = ToxicDirection.DownStream,
                Toxicity = 80
            };
            secondToxic.Attributes.Delay = 50;
            await newProxy.AddAsync(secondToxic);

            // Delete the first toxic
            await newProxy.RemoveToxicAsync(firstToxic.Name);

            // Retrieve the proxy and check that there is the
            // correct toxics
            var toxicsInProxy = await newProxy.GetAllToxicsAsync();
            Assert.True(1 == toxicsInProxy.Count());
            Assert.IsType<SlowCloseToxic>(toxicsInProxy.First());
            var singleToxicInProxy = (SlowCloseToxic)toxicsInProxy.First();
            Assert.Equal(secondToxic.Name, singleToxicInProxy.Name);
            Assert.Equal(secondToxic.Stream, singleToxicInProxy.Stream);
            Assert.Equal(secondToxic.Toxicity, singleToxicInProxy.Toxicity);
            Assert.Equal(secondToxic.Attributes.Delay, singleToxicInProxy.Attributes.Delay);
        }

        [Theory]
        [InlineData(ToxicDirection.UpStream, ToxicDirection.DownStream)]
        [InlineData(ToxicDirection.DownStream, ToxicDirection.UpStream)]
        public async Task UpdatingAToxicShouldWorks(ToxicDirection baseToxicDirection, ToxicDirection targetToxicDirection)
        {
            // Add a toxics to a proxy.
            // After update all the toxic's properties
            // Reload the toxic again and check that all the properties
            // are correctly updated 
            var client = Fixture.Client;
            var proxy = new Proxy
            {
                Name = "testingProxy",
                Enabled = true,
                Listen = "127.0.0.1:9090",
                Upstream = "google.com"
            };

            proxy = await client.AddAsync(proxy);

            var toxic = new SlicerToxic
            {
                Name = "SlicerToxicTest",
                Stream = baseToxicDirection
            };
            toxic.Attributes.AverageSize = 10;
            toxic.Attributes.Delay = 5;
            toxic.Attributes.SizeVariation = 1;
            toxic = await proxy.AddAsync(toxic);

            // Reload the toxic and update the properties
            var toxicInProxy = (SlicerToxic)await proxy.GetToxicByNameAsync(toxic.Name);

            // Update the toxic's property
            toxicInProxy.Name = "NewName";
            toxicInProxy.Stream = targetToxicDirection;
            toxicInProxy.Attributes.AverageSize = 20;
            toxicInProxy.Attributes.Delay = 10;
            toxicInProxy.Attributes.SizeVariation = 2;
            await proxy.UpdateToxicAsync(toxic.Name, toxicInProxy);

            // Reload again (we must use the initial name because the toxic update
            // cannot update the toxic name
            var updatedToxic = await proxy.GetToxicByNameAsync(toxic.Name);

            // Assert
            // WARNING: By design it's not possible to update the name and the stream properties of the proxy.
            Assert.NotEqual(toxicInProxy.Name, updatedToxic.Name);
            Assert.NotEqual(toxicInProxy.Stream, updatedToxic.Stream);
            Assert.IsType<SlicerToxic>(toxicInProxy);
            var specificToxicInProxy = (SlicerToxic)toxicInProxy;
            Assert.Equal(toxicInProxy.Attributes.AverageSize, specificToxicInProxy.Attributes.AverageSize);
            Assert.Equal(toxicInProxy.Attributes.Delay, specificToxicInProxy.Attributes.Delay);
            Assert.Equal(toxicInProxy.Attributes.SizeVariation, specificToxicInProxy.Attributes.SizeVariation);
        }
    }
}
