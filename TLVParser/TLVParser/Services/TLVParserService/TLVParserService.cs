using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TLVParser.Enums;
using TLVParser.Models;
using TLVParser.Models.ObjectLink;
using TLVParser.Models.ResourceInstances;

namespace TLVParser.Services.TLVParserService
{
    public class TLVParserService: ITLVParserService
    {
        public IEnumerable<TLVLine> ParseTLVPayload(List<string> tlvPayloadBytes)
        {
            var parsedTLVLines = new List<TLVLine>();

            var isTypeByteBeingRead = true;
            var isIdByteBeingRead = false;
            var isLengthByteBeingRead = false;
            var isValueBeingRead = false;

            var currentlyReadIdByteValue = string.Empty;
            var currentlyReadLengthByteValue = string.Empty;
            var currentlyReadValueByteValue = new List<string>();

            int? currentlyReadLengthFieldLength = null;
            int? currentlyReadValueFieldLength = null;

            var currentlyReadResource = new TLVLine();

            foreach (var requestByte in tlvPayloadBytes)
            {
                if (isTypeByteBeingRead)
                {
                    currentlyReadResource = new TLVLine();
                    currentlyReadResource.Type = ParseTLVType(requestByte);

                    isTypeByteBeingRead = false;
                    isIdByteBeingRead = true;
                }
                else if (isIdByteBeingRead)
                {
                    if (currentlyReadResource.Type.IdentifierLength == TLVIdentifierLength.EightBits ||
                        currentlyReadResource.Type.IdentifierLength == TLVIdentifierLength.SixteenBits &&
                        !string.IsNullOrEmpty(currentlyReadIdByteValue))
                    {
                        isIdByteBeingRead = false;
                    }

                    currentlyReadIdByteValue += requestByte;

                    if (!isIdByteBeingRead)
                    {
                        currentlyReadResource.Id = int.Parse(currentlyReadIdByteValue,
                            System.Globalization.NumberStyles.HexNumber);
                        currentlyReadIdByteValue = string.Empty;

                        if (currentlyReadResource.Type.LengthType == TLVLengthType.NoLength)
                        {
                            isValueBeingRead = true;
                        }
                        else
                        {
                            isLengthByteBeingRead = true;
                        }
                    }
                }
                else if (isLengthByteBeingRead)
                {
                    if (currentlyReadValueFieldLength == null)
                    {
                        currentlyReadValueFieldLength = (int)currentlyReadResource.Type.LengthType;
                    }

                    currentlyReadLengthByteValue += requestByte;
                    currentlyReadValueFieldLength--;

                    if (currentlyReadValueFieldLength == 0)
                    {
                        currentlyReadResource.Length = int.Parse(currentlyReadLengthByteValue,
                            System.Globalization.NumberStyles.HexNumber);

                        currentlyReadValueFieldLength = null;
                        currentlyReadLengthByteValue = string.Empty;
                        isLengthByteBeingRead = false;
                        isValueBeingRead = true;
                    }
                }
                else if (isValueBeingRead)
                {
                    if (currentlyReadLengthFieldLength == null)
                    {
                        if (currentlyReadResource.Length != null)
                        {
                            currentlyReadLengthFieldLength = currentlyReadResource.Length;
                        }
                        else
                        {
                            currentlyReadLengthFieldLength = currentlyReadResource.Type.ValueLength;
                        }
                    }

                    currentlyReadValueByteValue.Add(requestByte);
                    currentlyReadLengthFieldLength--;

                    if (currentlyReadLengthFieldLength == 0)
                    {
                        currentlyReadResource.ValueHex = currentlyReadValueByteValue;
                        currentlyReadValueByteValue = new List<string>();
                        currentlyReadLengthFieldLength = null;

                        parsedTLVLines.Add(currentlyReadResource);
                        currentlyReadResource = new TLVLine();
                        isValueBeingRead = false;
                        isTypeByteBeingRead = true;
                    }
                }
            }

            return parsedTLVLines;
        }

        public List<TLVResourceInstance> ParseResourceInstances(List<string> resourceInstanceBytes)
        {
            var resourceInstances = new List<TLVResourceInstance>();

            var tlvResourceInstanceItems = ParseTLVPayload(resourceInstanceBytes).ToList();

            if (tlvResourceInstanceItems.Any())
            {
                foreach (var tlvResourceInstanceItem in tlvResourceInstanceItems)
                {
                    var parsedAvailablePowerSourceItem = new TLVResourceInstance()
                    {
                        Id = tlvResourceInstanceItem.Id,
                        Value = int.Parse(string.Join(null, tlvResourceInstanceItem.ValueHex), System.Globalization.NumberStyles.HexNumber)
                    };
                    resourceInstances.Add(parsedAvailablePowerSourceItem);
                }
            }

            return resourceInstances;
        }

        public byte[] GetResultFromHexString(string hexString)
        {
            var hexStringLength = hexString.Length;
            var bytes = new byte[hexStringLength / 2];
            for (int hexByteIndex = 0; hexByteIndex < hexStringLength; hexByteIndex += 2)
            {
                bytes[hexByteIndex / 2] = Convert.ToByte(hexString.Substring(hexByteIndex, 2), 16);
            }

            return bytes;
        }

        public ObjectLink ParseObjectLink(List<string> objectLinkByteValues)
        {
            var objectLinkResult = new ObjectLink()
            {
                ObjectId = int.Parse(objectLinkByteValues[0] + objectLinkByteValues[1], System.Globalization.NumberStyles.HexNumber),
                ObjectInstanceId = int.Parse(objectLinkByteValues[2] + objectLinkByteValues[3], System.Globalization.NumberStyles.HexNumber)
            };

            return objectLinkResult;
        }

        private TLVType ParseTLVType(string maskedField)
        {
            var parsedTypeNumber = int.Parse(maskedField, System.Globalization.NumberStyles.HexNumber);

            var parsedTypeBytes = Convert.ToString(parsedTypeNumber, 2).PadLeft(8, '0');

            if (parsedTypeBytes.Length != 8 || !CheckIfBinary(parsedTypeBytes))
            {
                throw new Exception();
            }

            var parsedTLVType = new TLVType()
            {
                IdentifierType = (TLVIdentifierType)Convert.ToInt32(parsedTypeBytes.Substring(0, 2), 2),
                IdentifierLength = (TLVIdentifierLength)Convert.ToInt32(parsedTypeBytes.Substring(2, 1), 2),
                LengthType = (TLVLengthType)Convert.ToInt32(parsedTypeBytes.Substring(3, 2), 2),
                ValueLength = Convert.ToInt32(parsedTypeBytes.Substring(5, 3), 2)
            };

            return parsedTLVType;
        }

        private bool CheckIfBinary(string text)
        {
            return Regex.IsMatch(text, @"^[01]+$");
        }
    }
}
