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
            var manufacturerResourcePayLoad = "C8 00 14 4F 70 65 6E 20 4D 6F 62 69 6C 65 20 41 6C 6C 69 61 6E 63 65";
            var expectedParserValueResult = "Open Mobile Alliance";

            var tlvParserService = new TLVParserService();
            var result = tlvParserService.ParseTLVPayload(manufacturerResourcePayLoad).First();

            Assert.AreEqual(result.Value, expectedParserValueResult);
        }

        [TestMethod]
        public void SingleObjectInstanceRequestText()
        {
            const string tlvPayloadBytes = @"
                C8 00 14 4F 70 65 6E 20 4D 6F 62 69 6C 65 20 41 6C 6C 69 61 6E 63 65
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

        [TestMethod]
        public void MultipleObjectInstanceRequestTest1()
        {
            const string tlvPayloadBytes = @"
                08 00 79
                    C8 00 14 4F 70 65 6E 20 4D 6F 62 69 6C 65 20 41 6C 6C 69 61 6E 63 65
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

        [TestMethod]
        public void MultipleObjectInstanceRequestWithTwoInstancesTest()
        {
            const string tlvPayloadBytes = @"
                08 00 0E
                    C1 00 01
                    C1 01 00
                    83 02
                        41 7F 07
                    C1 03 7F
                08 02 12
                    C1 00 03
                    C1 01 00
                    86 02 41 7F 07 61 01 36 01
                    C1 03 7F";
        }

        [TestMethod]
        public void MultipleObjectInstanceRequestWithSingleInstanceTest()
        {
            const string tlvPayloadBytes =@"
                08 00 0D
                    C1 00 01
                    C4 01 00 01 51 80
                    C1 06 01
                    C1 07 55";
        }

        [TestMethod]
        public void ObjectInstanceRequestWithObjectLinkTest1()
        {
            const string tlvPayloadBytes = @"
                88 00 0C
                    44 00 00 42 00 00
                    44 01 00 42 00 01
                C8 01 0D 38 36 31 33 38 30 30 37 35 35 35 30 30
                C4 02 12 34 56 78";
        }

        [TestMethod]
        public void ObjectInstanceRequestWithObjectLinkTest2()
        {
            const string tlvPayloadBytes = @"
                08 00 23
                    C8 00 0B 6D 79 53 65 72 76 69 63 65 20 31
                    C8 01 0F 49 6E 74 65 72 6E 65 74 2E 31 35 2E 32 33 34
                    C4 02 00 43 00 00
                08 01 23
                    C8 00 0B 6D 79 53 65 72 76 69 63 65 20 32
                    C8 01 0F 49 6E 74 65 72 6E 65 74 2E 31 35 2E 32 33 35
                    C4 02 FF FF FF FF";
        }
    }
}
