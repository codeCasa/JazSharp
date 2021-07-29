using JazSharp.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;

namespace JazSharp.TestAdapter
{
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [DefaultExecutorUri(TestAdapterConstants.ExecutorUriString)]
    [ExtensionUri(TestAdapterConstants.ExecutorUriString)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class TestDiscoverer : ITestDiscoverer, IDisposable
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private TestCollection _testCollection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Dispose();

            _testCollection = TestCollection.FromSources(sources);

            foreach (var test in _testCollection.Tests)
            {
                logger.SendMessage(TestMessageLevel.Informational, "Test found: " + test.FullName);
                discoverySink.SendTestCase(test.ToTestCase());
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Dispose()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (_testCollection != null)
            {
                _testCollection.Dispose();
                _testCollection = null;
            }
        }
    }
}
