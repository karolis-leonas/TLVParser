using System.Collections.Generic;
using TLVParser.Models.AccessControlObject;

namespace TLVParser.Services.AccessControlObjectService
{
    public interface IAccessControlObjectService
    {
        IEnumerable<MultipleAccessControlObject> ReadMultipleAccessControlObjects(string payload);
        AccessControlObject ReadSingleAccessControlObject(string payload);
    }
}
