using System.Collections.Generic;
using TLVParser.Models.ResourceInstances;

namespace TLVParser.Models.AccessControlObject
{
    public class AccessControlObject
    {
        public int ObjectId { get; set; }
        public int ObjectInstanceId { get; set; }
        public List<AccessControlResourceInstance> ACL { get; set; }
        public int AccessControlOwner { get; set; }
    }
}
