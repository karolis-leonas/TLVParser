using System.Collections.Generic;
using TLVParser.Models.DeviceObject;

namespace TLVParser.Services.DeviceObjectService
{
    public interface IDeviceObjectService
    {
        IEnumerable<MultiDeviceObject> ReadMultipleDeviceObjects(string payload);
        DeviceObject ReadSingleDeviceObject(string payload);
    }
}
