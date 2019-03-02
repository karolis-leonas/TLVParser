using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TLVParser.Enums;
using TLVParser.Models;

namespace TLVParser
{
    public class TLVParserService: ITLVParserService
    {
        public IEnumerable<TLVLine> ParseTLVPayload(string tlvPayloadBytes)
        {
            var parsedTLVLines = new List<TLVLine>();

            var requestBytes = tlvPayloadBytes.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            var isTypeByteBeingRead = true;
            var isIdByteBeingRead = false;
            var isLengthByteBeingRead = false;
            var isValueBeingRead = false;

            var currentlyReadIdByteValue = string.Empty;
            var currentlyReadLengthByteValue = string.Empty;
            var currentlyReadValueByteValue = string.Empty;

            int? currentlyReadLengthFieldLength = null;
            int? currentlyReadValueFieldLength = null;

            var currentlyReadResource = new TLVLine();

            foreach (var requestByte in requestBytes)
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

                    currentlyReadValueByteValue += requestByte;
                    currentlyReadLengthFieldLength--;

                    if (currentlyReadLengthFieldLength == 0)
                    {
                        currentlyReadResource.Value = Encoding.UTF8.GetString(GetResultFromHexString(currentlyReadValueByteValue));
                        currentlyReadValueByteValue = string.Empty;

                        parsedTLVLines.Add(currentlyReadResource);
                        currentlyReadResource = new TLVLine();
                        isValueBeingRead = false;
                        isTypeByteBeingRead = true;
                    }
                }
            }

            return parsedTLVLines;
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
                ValueLength = Convert.ToInt32(parsedTypeBytes.Substring(5, 3))
            };

            return parsedTLVType;
        }

        private bool CheckIfBinary(string text)
        {
            return Regex.IsMatch(text, @"^[01]+$");
        }

        private bool CheckIfHexNumber(string text)
        {
            return Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        private byte[] GetResultFromHexString(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return bytes;
        }
    }
}
