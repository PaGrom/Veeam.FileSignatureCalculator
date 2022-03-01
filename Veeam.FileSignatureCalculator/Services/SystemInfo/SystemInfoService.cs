using System;

namespace Veeam.FileSignatureCalculator.Services.SystemInfo
{
    ///<inheritdoc cref="ISystemInfoService"/>
    public sealed class SystemInfoService : ISystemInfoService
    {
        ///<inheritdoc cref="ISystemInfoService.AvailableMemorySize"/>
        public long AvailableMemorySize => GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        ///<inheritdoc cref="ISystemInfoService.ProcessCount"/>
        public int ProcessCount => Environment.ProcessorCount;
    }
}
