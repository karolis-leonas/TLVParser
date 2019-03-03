using System.Collections.Generic;
using TLVParser.Models.DeviceObject;
using TLVParser.Models.ServerObject;

namespace TLVParser.Services.ServerObjectService
{
    public interface IServerObjectService
    {
        IEnumerable<ExtendedServerObject> ReadMultipleServerObjects(string payload);
        ServerObject ReadSingleServerObject(string payload);
    }
}
