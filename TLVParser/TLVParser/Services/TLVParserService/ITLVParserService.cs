using System.Collections.Generic;
using TLVParser.Models;

namespace TLVParser
{
    public interface ITLVParserService
    {
        IEnumerable<TLVLine> ParseTLVPayload(List<string> tlvPayloadBytes);
        List<TLVResourceInstance> ParseResourceInstances(List<string> resourceInstanceBytes);
        byte[] GetResultFromHexString(string hex);
    }
}
