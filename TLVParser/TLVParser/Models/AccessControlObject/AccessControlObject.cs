using System.Collections.Generic;

namespace TLVParser.Models.AccessControlObject
{
    public class AccessControlObject
    {
        public int ObjectId { get; set; }
        public int ObjectInheritanceId { get; set; }
        public List<TLVResourceInstance> ACL { get; set; }
        public int AccessControlOwner { get; set; }
    }
}
