using Veeam.FileSignatureCalculator.Services.SystemInfo;

namespace Veeam.FileSignatureCalculator.Tests.Mocks
{
    public sealed class TestSystemInfoService : ISystemInfoService
    {
        public long AvailableMemorySize { get; set; }
        public int ProcessCount { get; set; }
    }
}
