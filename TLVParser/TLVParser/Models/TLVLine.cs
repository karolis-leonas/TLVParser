namespace TLVParser.Models
{
    public class TLVLine
    {
        public TLVType Type { get; set; }
        public int Id { get; set; }
        public int? Length { get; set; }
        public object Value { get; set; }
    }
}
