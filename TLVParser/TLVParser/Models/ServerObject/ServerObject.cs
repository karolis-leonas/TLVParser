namespace TLVParser.Models.ServerObject
{
    public class ServerObject
    {
        public int ShortServerId { get; set; }
        public int Lifetime { get; set; }
        public int DefaultMinimumPeriod { get; set; }
        public int DefaultMaximumPeriod { get; set; }
        public int DisableTimeout { get; set; }
        public bool AreNotificationsStored { get; set; }
        public string BindingPreference { get; set; }
    }
}
