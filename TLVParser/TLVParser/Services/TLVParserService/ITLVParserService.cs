using System.Collections.Generic;
using TLVParser.Models;
using TLVParser.Models.ObjectLink;
using TLVParser.Models.ResourceInstances;

namespace TLVParser
{
    public interface ITLVParserService
    {
        IEnumerable<TLVLine> ParseTLVPayload(List<string> tlvPayloadBytes);
        List<TLVResourceInstance> ParseResourceInstances(List<string> resourceInstanceBytes);
        byte[] GetResultFromHexString(string hexString);
        ObjectLink ParseObjectLink(List<string> objectLinkByteValues);
    }
}
