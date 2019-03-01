using TLVParser.Enums;

namespace TLVParser
{
    public class TLVType
    {
        public TLVIdentifierType IdentifierType { get; set; }
        public TLVIdentifierLength IdentifierLength { get; set; }
        public TLVLengthType LengthType { get; set; }
        public int ValueLength { get; set; }
    }
}
