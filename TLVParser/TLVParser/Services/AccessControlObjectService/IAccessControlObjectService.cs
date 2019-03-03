using System.Collections.Generic;
using TLVParser.Models.AccessControlObject;

namespace TLVParser.Services.AccessControlObjectService
{
    public interface IAccessControlObjectService
    {
        IEnumerable<MultipleAccessControlObject> ReadPayloadForMultipleAccessControlObjectInstances(string payload);
        AccessControlObject ReadPayloadForSingleAccessControlObjectInstance(string payload);
    }
}
