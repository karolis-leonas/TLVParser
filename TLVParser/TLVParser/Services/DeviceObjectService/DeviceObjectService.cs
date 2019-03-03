using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLVParser.Enums;
using TLVParser.Models.DeviceObject;

namespace TLVParser.Services.DeviceObjectService
{
    public class DeviceObjectService: IDeviceObjectService
    {
        private ITLVParserService _tlvParserService;

        public DeviceObjectService()
        {
            _tlvParserService = new TLVParserService();
        }

        public IEnumerable<MultiDeviceObject> ReadMultipleDeviceObjects(string payload)
        {
            var deviceObjects = new List<MultiDeviceObject>();

            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var deviceObjectsToBeRead = _tlvParserService.ParseTLVPayload(tlvPayloadBytes).ToList();

            foreach (var deviceObjectToBeRead in deviceObjectsToBeRead)
            {
                var deviceObject = new MultiDeviceObject()
                {
                    Id = deviceObjectToBeRead.Id,
                    DeviceObject = ParseDeviceObject(deviceObjectToBeRead.ValueHex)
                };

                deviceObjects.Add(deviceObject);
            }

            return deviceObjects;
        }

        public DeviceObject ReadSingleDeviceObject(string payload)
        {
            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var deviceObject = ParseDeviceObject(tlvPayloadBytes);

            return deviceObject;

        }

        private DeviceObject ParseDeviceObject(List<string> tlvPayloadBytes)
        {
            var parsedTLVLines = _tlvParserService.ParseTLVPayload(tlvPayloadBytes);

            var deviceObject = new DeviceObject();
            foreach (var parsedTLVLine in parsedTLVLines)
            {
                switch ((DeviceObjectResourceId)parsedTLVLine.Id)
                {
                    case DeviceObjectResourceId.Manufacturer:
                        deviceObject.Manufacturer = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectResourceId.ModelNumber:
                        deviceObject.ModelNumber = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectResourceId.SerialNumber:
                        deviceObject.SerialNumber = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectResourceId.FirmwareVersion:
                        deviceObject.FirmwareVersion = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectResourceId.AvailablePowerSources:
                        deviceObject.AvailablePowerSources = _tlvParserService.ParseResourceInstances(parsedTLVLine.ValueHex);
                        break;
                    case DeviceObjectResourceId.PowerSourceVoltage:
                        deviceObject.PowerSourceVoltage = _tlvParserService.ParseResourceInstances(parsedTLVLine.ValueHex);
                        break;
                    case DeviceObjectResourceId.PowerSourceCurrent:
                        deviceObject.PowerSourceCurrent = _tlvParserService.ParseResourceInstances(parsedTLVLine.ValueHex);
                        break;
                    case DeviceObjectResourceId.BatteryLevel:
                        deviceObject.BatteryLevel = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case DeviceObjectResourceId.MemoryFree:
                        deviceObject.MemoryFree = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case DeviceObjectResourceId.ErrorCode:
                        deviceObject.ErrorCode = _tlvParserService.ParseResourceInstances(parsedTLVLine.ValueHex);
                        break;
                    case DeviceObjectResourceId.CurrentTime:
                        deviceObject.CurrentTime =
                            DateTimeOffset.FromUnixTimeSeconds(int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber)).DateTime;
                        break;
                    case DeviceObjectResourceId.UtcOffset:
                        deviceObject.UtcOffset = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case DeviceObjectResourceId.SupportedBindingAndModes:
                        deviceObject.SupportedBindingAndModes = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                }
            }

            return deviceObject;
        }
    }
}
