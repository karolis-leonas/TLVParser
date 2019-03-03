using System.Collections.Generic;
using TLVParser.Models.DeviceObject;

namespace TLVParser.Services.DeviceObjectService
{
    public interface IDeviceObjectService
    {
        IEnumerable<ExtendedDeviceObject> ReadMultipleDeviceObjects(string payload);
        DeviceObject ReadSingleDeviceObject(string payload);
    }
}
