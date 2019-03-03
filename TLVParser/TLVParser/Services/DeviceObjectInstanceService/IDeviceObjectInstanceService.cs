using System.Collections.Generic;
using TLVParser.Models.DeviceObjectInstance;

namespace TLVParser.Services.DeviceObjectInstanceService
{
    public interface IDeviceObjectInstanceService
    {
        IEnumerable<MultipleDeviceObjectInstance> ReadPayloadForMultipleDeviceObjectInstances(string payload);
        DeviceObjectInstance ReadPayloadForSingleDeviceObjectInstance(string payload);
    }
}
