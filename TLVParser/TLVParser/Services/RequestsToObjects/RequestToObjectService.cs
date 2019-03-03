using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLVParser.Enums;
using TLVParser.Models;
using TLVParser.Models.ObjectLink;
using TLVParser.Models.RequestsToObjects.RequestToObject65;
using TLVParser.Models.RequestsToObjects.RequestToObject66;
using TLVParser.Models.ResourceInstances;
using TLVParser.Models.ServerObject;
using TLVParser.Services.RequestsToObjects;

namespace TLVParser.Services.ServerObjectService
{
    public class RequestToObjectService : IRequestToObjectService
    {
        private ITLVParserService _tlvParserService;

        public RequestToObjectService()
        {
            _tlvParserService = new TLVParserService();
        }

        public IEnumerable<ExtendedRequestToObject65> ReadRequestToObject65WithMultipleInstances(string payload)
        {
            var serverObjects = new List<ExtendedRequestToObject65>();

            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var serverObjectsToBeRead = _tlvParserService.ParseTLVPayload(tlvPayloadBytes).ToList();

            foreach (var serverObjectToBeRead in serverObjectsToBeRead)
            {
                var serverObject = new ExtendedRequestToObject65()
                {
                    Id = serverObjectToBeRead.Id,
                    RequestToObject65 = ParseRequestToObject65Object(serverObjectToBeRead.ValueHex)
                };

                serverObjects.Add(serverObject);
            }

            return serverObjects;
        }

        public IEnumerable<ExtendedRequestToObject66> ReadRequestToObject66WithMultipleInstances(string payload)
        {
            var serverObjects = new List<ExtendedRequestToObject66>();

            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var serverObjectsToBeRead = _tlvParserService.ParseTLVPayload(tlvPayloadBytes).ToList();

            foreach (var serverObjectToBeRead in serverObjectsToBeRead)
            {
                var serverObject = new ExtendedRequestToObject66()
                {
                    Id = serverObjectToBeRead.Id,
                    RequestToObject66 = ParseRequestToObject66Object(serverObjectToBeRead.ValueHex)
                };

                serverObjects.Add(serverObject);
            }

            return serverObjects;
        }

        public RequestToObject65 ReadRequestToObject65WithSingleInstance(string payload)
        {
            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var serverObject = ParseRequestToObject65Object(tlvPayloadBytes);

            return serverObject;
        }

        public RequestToObject66 ReadRequestToObject66WithSingleInstance(string payload)
        {
            var tlvPayloadBytes = payload.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            var requestObject = ParseRequestToObject66Object(tlvPayloadBytes);

            return requestObject;
        }

        private RequestToObject65 ParseRequestToObject65Object(List<string> tlvPayloadBytes)
        {
            var parsedTLVLines = _tlvParserService.ParseTLVPayload(tlvPayloadBytes);

            var requestObject = new RequestToObject65();
            foreach (var parsedTLVLine in parsedTLVLines)
            {
                switch ((RequestObjectId)parsedTLVLine.Id)
                {
                    case RequestObjectId.Res0:
                        requestObject.Res0 = ParseMultipleObjectLinks(parsedTLVLine.ValueHex).ToList();
                        break;
                    case RequestObjectId.Res1:
                        requestObject.Res1 = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case RequestObjectId.Res2:
                        requestObject.Res2 = int.Parse(string.Join(null, parsedTLVLine.ValueHex), System.Globalization.NumberStyles.HexNumber);
                        break;
                }
            }

            return requestObject;
        }

        private RequestToObject66 ParseRequestToObject66Object(List<string> tlvPayloadBytes)
        {
            var parsedTLVLines = _tlvParserService.ParseTLVPayload(tlvPayloadBytes);

            var requestObject = new RequestToObject66();
            foreach (var parsedTLVLine in parsedTLVLines)
            {
                switch ((RequestObjectId)parsedTLVLine.Id)
                {
                    case RequestObjectId.Res0:
                        requestObject.Res0 = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case RequestObjectId.Res1:
                        requestObject.Res1 = Encoding.UTF8.GetString(_tlvParserService.GetResultFromHexString(string.Join(null, parsedTLVLine.ValueHex)));
                        break;
                    case RequestObjectId.Res2:
                        requestObject.Res2 = _tlvParserService.ParseObjectLink(parsedTLVLine.ValueHex);
                        break;
                }
            }

            return requestObject;
        }

        private IEnumerable<ExtendedObjectLink> ParseMultipleObjectLinks(List<string> resourceInstanceBytes)
        {
            var objectLinks = new List<ExtendedObjectLink>();

            var objectLinkLines = _tlvParserService.ParseTLVPayload(resourceInstanceBytes).ToList();

            if (objectLinkLines.Any())
            {
                foreach (var objectLinkLine in objectLinkLines)
                {
                    var parsedAvailablePowerSourceItem = new ExtendedObjectLink()
                    {
                        Id = objectLinkLine.Id,
                        ObjectLink = _tlvParserService.ParseObjectLink(objectLinkLine.ValueHex)
                    };
                    objectLinks.Add(parsedAvailablePowerSourceItem);
                }
            }

            return objectLinks;
        }
    }
}
