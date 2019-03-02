using System.Collections.Generic;

namespace TLVParser.Models
{
    public class TLVLine
    {
        public TLVType Type { get; set; }
        public int Id { get; set; }
        public int? Length { get; set; }
        public List<string> ValueHex { get; set; }
    }
}
