using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLVParser.Enums;
using TLVParser.Models;
using TLVParser.Models.AccessControlObject;
using TLVParser.Models.ResourceInstances;

namespace TLVParser.Services.AccessControlObjectService
{
    public class AccessControlObjectService : IAccessControlObjectService
    {
        private ITLVParserService _tlvParserService;

        public AccessControlObjectService()
        {
            _tlvParserService = new TLVParserService();
        }

        public IEnumerable<ExtendedAccessControlObject> ReadMultipleAccessControlObjects(string payload)
        {
            var multipleAccessControlObjects = new List<ExtendedAccessControlObject>();

            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var parsedTLVObjectInstanceObjects = _tlvParserService.ParseTLVPayload(tlvPayloadBytes).ToList();

            foreach (var parsedTLVObjectInstanceObject in parsedTLVObjectInstanceObjects)
            {
                var accessControlObject = new ExtendedAccessControlObject()
                {
                    Id = parsedTLVObjectInstanceObject.Id,
                    AccessControlObject = ReadDeviceObjectInstance(parsedTLVObjectInstanceObject.ValueHex)
                };

                multipleAccessControlObjects.Add(accessControlObject);
            }

            return multipleAccessControlObjects;
        }

        public AccessControlObject ReadSingleAccessControlObject(string payload)
        {
            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var deviceObjectInstance = ReadDeviceObjectInstance(tlvPayloadBytes);

            return deviceObjectInstance;

        }

        private AccessControlObject ReadDeviceObjectInstance(List<string> tlvPayloadBytes)
        {
            var parsedTLVLines = _tlvParserService.ParseTLVPayload(tlvPayloadBytes);

            var deviceObjectInstance = new AccessControlObject();
            foreach (var parsedTLVLine in parsedTLVLines)
            {
                switch ((AccessControlObjectId)parsedTLVLine.Id)
                {
                    case AccessControlObjectId.ObjectId:
                        deviceObjectInstance.ObjectId = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case AccessControlObjectId.ObjectInstanceId:
                        deviceObjectInstance.ObjectInstanceId = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case AccessControlObjectId.ACL:
                        deviceObjectInstance.ACL = ReadACLValues(parsedTLVLine.ValueHex).ToList();
                        break;
                    case AccessControlObjectId.AccessControlOwner:
                        deviceObjectInstance.AccessControlOwner = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                }
            }

            return deviceObjectInstance;
        }

        private IEnumerable<AccessControlResourceInstance> ReadACLValues(List<string> valueBytes)
        {
            var ACLResult = new List<AccessControlResourceInstance>();

            var resourceInstances = _tlvParserService.ParseResourceInstances(valueBytes);

            foreach (var resourceInstance in resourceInstances)
            {
                var byteValue = Convert.ToString(resourceInstance.Value, 2);
                var byteLength = ((byteValue.Length / 8) * 8) + 8;

                byteValue = byteValue.PadLeft(byteLength, '0');

                var parsedAvailablePowerSourceItem = new AccessControlResourceInstance()
                {
                    Id = resourceInstance.Id,
                    Value = resourceInstance.Value,
                    ByteValue = byteValue
                };

                 ACLResult.Add(parsedAvailablePowerSourceItem);
            }

            return ACLResult;
        }
    }
}
