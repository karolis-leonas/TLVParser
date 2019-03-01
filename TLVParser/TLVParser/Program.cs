using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TLVParser.Enums;
using TLVParser.Models;

namespace TLVParser
{
    class Program
    {
        static void Main(string[] args)
        {
            const string tlvPayloadBytes = @"C8 00 14 4F 70 65 6E 20 4D 6F 62 69 6C 65 20 41 6C 6C 69 61 6E 63 65
C8 01 16 4C 69 67 68 74 77 65 69 67 74 20 4D 32 4D 20 43 6C 69 65 6E 74
C8 02 09 33 34 35 30 30 30 31 32 33
C3 03 31 2E 30
86 06
41 00 01
41 01 05
88 07 08
42 00 0E D8
42 01 13 88
87 08
41 00 7D
42 01 03 84
C1 09 64
C1 0A 0F
83 0B
41 00 00
C4 0D 51 82 42 8F
C6 0E 2B 30 32 3A 30 30
C1 10 55";

            var tlvByteArray = tlvPayloadBytes.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

            var parsedTLVLines = GetTLVLineValues(tlvByteArray);

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }

        public static IEnumerable<TLVLine> GetTLVLineValues(string[] requestBytes)
        {
            var parsedTLVLines = new List<TLVLine>();

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
                        currentlyReadValueFieldLength = (int) currentlyReadResource.Type.LengthType;
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

        public static TLVType ParseTLVType(string maskedField)
        {
            var parsedTypeNumber = int.Parse(maskedField, System.Globalization.NumberStyles.HexNumber);

            var parsedTypeBytes = Convert.ToString(parsedTypeNumber, 2).PadLeft(8, '0');

            if (parsedTypeBytes.Length != 8 || !CheckIfBinary(parsedTypeBytes))
            {
                throw new Exception();
            }

            var parsedTLVType = new TLVType()
            {
                IdentifierType = (TLVIdentifierType) Convert.ToInt32(parsedTypeBytes.Substring(0, 2), 2),
                IdentifierLength = (TLVIdentifierLength) Convert.ToInt32(parsedTypeBytes.Substring(2, 1), 2),
                LengthType = (TLVLengthType) Convert.ToInt32(parsedTypeBytes.Substring(3, 2), 2),
                ValueLength = Convert.ToInt32(parsedTypeBytes.Substring(5, 3))
            };

            return parsedTLVType;
        }

        public static bool CheckIfBinary(string text)
        {
            return Regex.IsMatch(text, @"^[01]+$");
        }

        public static bool CheckIfHexNumber(string text)
        {
            return Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static byte[] GetResultFromHexString(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return bytes;
        }
    }
}
