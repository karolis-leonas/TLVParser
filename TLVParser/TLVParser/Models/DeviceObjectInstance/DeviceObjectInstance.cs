using System;
using System.Collections.Generic;

namespace TLVParser.Models.DeviceObjectInstance
{
    public class DeviceObjectInstance
    {
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public List<TLVResourceInstance> AvailablePowerSources { get; set; }
        public List<TLVResourceInstance> PowerSourceVoltage { get; set; }
        public List<TLVResourceInstance> PowerSourceCurrent { get; set; }
        public int BatteryLevel { get; set; }
        public int MemoryFree { get; set; }
        public List<TLVResourceInstance> ErrorCode { get; set; }
        public DateTime CurrentTime { get; set; }
        public string UtcOffset { get; set; }
        public string SupportedBindingAndModes { get; set; }
    }
}
