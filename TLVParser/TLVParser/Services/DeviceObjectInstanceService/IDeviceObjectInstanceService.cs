using System.Collections.Generic;
using TLVParser.Models.DeviceObjectInstance;

namespace TLVParser.Services.DeviceObjectInstanceService
{
    public interface IDeviceObjectInstanceService
    {
        IEnumerable<MultipleDeviceObjectInstance> ReadPayloadForMultipleObjectInstances(string payload);
        DeviceObjectInstance ReadPayloadForSingleObjectInstance(string payload);
    }
}
