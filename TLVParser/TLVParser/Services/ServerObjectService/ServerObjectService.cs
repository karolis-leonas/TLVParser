using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLVParser.Enums;
using TLVParser.Models.ServerObject;

namespace TLVParser.Services.ServerObjectService
{
    public class ServerObjectService : IServerObjectService
    {
        private ITLVParserService _tlvParserService;

        public ServerObjectService()
        {
            _tlvParserService = new TLVParserService.TLVParserService();
        }

        public IEnumerable<ExtendedServerObject> ReadMultipleServerObjects(string payload)
        {
            var serverObjects = new List<ExtendedServerObject>();

            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var serverObjectsToBeRead = _tlvParserService.ParseTLVPayload(tlvPayloadBytes).ToList();

            foreach (var serverObjectToBeRead in serverObjectsToBeRead)
            {
                var serverObject = new ExtendedServerObject()
                {
                    Id = serverObjectToBeRead.Id,
                    ServerObject = ParseServerObject(serverObjectToBeRead.ValueHex)
                };

                serverObjects.Add(serverObject);
            }

            return serverObjects;
        }

        public ServerObject ReadSingleServerObject(string payload)
        {
            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var serverObject = ParseServerObject(tlvPayloadBytes);

            return serverObject;
        }

        private ServerObject ParseServerObject(List<string> tlvPayloadBytes)
        {
            var parsedTLVLines = _tlvParserService.ParseTLVPayload(tlvPayloadBytes);

            var serverObject = new ServerObject();
            foreach (var parsedTLVLine in parsedTLVLines)
            {
                switch ((ServerObjectId)parsedTLVLine.Id)
                {
                    case ServerObjectId.ShortServerId:
                        serverObject.ShortServerId = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case ServerObjectId.Lifetime:
                        serverObject.Lifetime = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case ServerObjectId.DefaultMinimumPeriod:
                        serverObject.DefaultMinimumPeriod = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case ServerObjectId.DefaultMaximumPeriod:
                        serverObject.DefaultMaximumPeriod = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case ServerObjectId.DisableTimeout:
                        serverObject.DisableTimeout = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                    case ServerObjectId.AreNotificationsStored:
                        serverObject.AreNotificationsStored = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber) > 0;
                        break;
                    case ServerObjectId.BindingPreference:
                        serverObject.BindingPreference = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                }
            }

            return serverObject;
        }
    }
}
