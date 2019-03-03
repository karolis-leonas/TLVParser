using System.Collections.Generic;
using TLVParser.Models.AccessControlObject;

namespace TLVParser.Services.AccessControlObjectService
{
    public interface IAccessControlObjectService
    {
        IEnumerable<ExtendedAccessControlObject> ReadMultipleAccessControlObjects(string payload);
        AccessControlObject ReadSingleAccessControlObject(string payload);
    }
}
