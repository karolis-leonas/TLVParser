using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TLVParser;

namespace TLVParserUnitTests
{
    [TestClass]
    public class TLVParserUnitTests
    {


        [TestMethod]
        public void SingleLineTest()
        {
            string manufacturerResourcePayLoad = "C8 00 14 4F 70 65 6E 20 4D 6F 62 69 6C 65 20 41 6C 6C 69 61 6E 63 65";
            string expectedParserValueResult = "Open Mobile Alliance";

            var tlvParserService = new TLVParserService();
            var result = tlvParserService.ParseTLVPayload(manufacturerResourcePayLoad).First();

            Assert.AreEqual(result.Value, expectedParserValueResult);
        }

        [TestMethod]
        public void MultiplePayloadTest()
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
        }
    }
}