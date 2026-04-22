using Xunit;

namespace NurseRecordingBackend.Tests.Helpers;

[CollectionDefinition("SharedCollection")]
public class SharedCollection : ICollectionFixture<object>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
