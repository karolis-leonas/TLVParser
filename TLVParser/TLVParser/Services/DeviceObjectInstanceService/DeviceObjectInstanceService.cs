using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLVParser.Enums;
using TLVParser.Models;
using TLVParser.Models.DeviceObjectInstance;

namespace TLVParser.Services.DeviceObjectInstanceService
{
    public class DeviceObjectInstanceService: IDeviceObjectInstanceService
    {
        private ITLVParserService _tlvParserService;

        public DeviceObjectInstanceService()
        {
            _tlvParserService = new TLVParserService();
        }

        public IEnumerable<MultipleDeviceObjectInstance> ReadPayloadForMultipleObjectInstances(string payload)
        {
            throw new NotImplementedException();
        }

        public DeviceObjectInstance ReadPayloadForSingleObjectInstance(string payload)
        {
            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();

            var parsedTLVLines = _tlvParserService.ParseTLVPayload(tlvPayloadBytes);

            var deviceObjectInstance = new DeviceObjectInstance();
            foreach (var parsedTLVLine in parsedTLVLines)
            {
                switch ((DeviceObjectInstanceResourceId)parsedTLVLine.Id)
                {
                    case DeviceObjectInstanceResourceId.Manufacturer:
                        deviceObjectInstance.Manufacturer = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectInstanceResourceId.ModelNumber:
                        deviceObjectInstance.ModelNumber = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectInstanceResourceId.SerialNumber:
                        deviceObjectInstance.SerialNumber = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectInstanceResourceId.FirmwareVersion:
                        deviceObjectInstance.FirmwareVersion = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectInstanceResourceId.AvailablePowerSources:
                        deviceObjectInstance.AvailablePowerSources = _tlvParserService.ParseResourceInstances(parsedTLVLine.ValueHex);
                        break;
                    case DeviceObjectInstanceResourceId.PowerSourceVoltage:
                        deviceObjectInstance.PowerSourceVoltage = _tlvParserService.ParseResourceInstances(parsedTLVLine.ValueHex);
                        break;
                    case DeviceObjectInstanceResourceId.PowerSourceCurrent:
                        deviceObjectInstance.PowerSourceCurrent = _tlvParserService.ParseResourceInstances(parsedTLVLine.ValueHex);
                        break;
                    case DeviceObjectInstanceResourceId.BatteryLevel:
                        deviceObjectInstance.BatteryLevel = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case DeviceObjectInstanceResourceId.MemoryFree:
                        deviceObjectInstance.MemoryFree = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case DeviceObjectInstanceResourceId.ErrorCode:
                        deviceObjectInstance.ErrorCode = _tlvParserService.ParseResourceInstances(parsedTLVLine.ValueHex);
                        break;
                    case DeviceObjectInstanceResourceId.CurrentTime:
                        deviceObjectInstance.CurrentTime =
                            DateTimeOffset.FromUnixTimeSeconds(int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber)).DateTime;
                        break;
                    case DeviceObjectInstanceResourceId.UtcOffset:
                        deviceObjectInstance.UtcOffset = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectInstanceResourceId.SupportedBindingAndModes:
                        deviceObjectInstance.SupportedBindingAndModes = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                }
            }

            return deviceObjectInstance;
        }
    }
}
