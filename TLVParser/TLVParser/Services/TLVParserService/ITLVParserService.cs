using System.Collections.Generic;
using TLVParser.Models;

namespace TLVParser
{
    public interface ITLVParserService
    {
        IEnumerable<TLVLine> ParseTLVPayload(string tlvPayloadBytes);
    }
}
