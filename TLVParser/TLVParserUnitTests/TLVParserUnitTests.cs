using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TLVParser.Models.AccessControlObject;
using TLVParser.Models.DeviceObject;
using TLVParser.Models.ResourceInstances;
using TLVParser.Services.AccessControlObjectService;
using TLVParser.Services.DeviceObjectService;

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

            var deviceObjectInstanceService = new DeviceObjectService();
            var result = deviceObjectInstanceService.ReadSingleDeviceObject(manufacturerResourcePayLoad);

            Assert.AreEqual(result.Manufacturer, expectedParserValueResult);
        }

        [TestMethod]
        public void SingleDeviceObjectTest()
        {
            const string tlvPayloadBytes = @"
                C8 00 14 4F 70 65 6E 20 4D 6F 62 69 6C 65 20 41 6C 6C 69 61 6E 63 65
                C8 01 16 4C 69 67 68 74 77 65 69 67 68 74 20 4D 32 4D 20 43 6C 69 65 6E 74
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

            var deviceObjectService = new DeviceObjectService();
            var result = deviceObjectService.ReadSingleDeviceObject(tlvPayloadBytes);

            CheckDeviceObjectResult(result);
        }

        [TestMethod]
        public void MultipleDeviceObjectTest()
        {
            const string tlvPayloadBytes = @"
                08 00 79
                    C8 00 14 4F 70 65 6E 20 4D 6F 62 69 6C 65 20 41 6C 6C 69 61 6E 63 65
                    C8 01 16 4C 69 67 68 74 77 65 69 67 68 74 20 4D 32 4D 20 43 6C 69 65 6E 74
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

            var deviceObjectService = new DeviceObjectService();
            var deviceObjects = deviceObjectService.ReadMultipleDeviceObjects(tlvPayloadBytes).ToList();

            Assert.AreEqual(deviceObjects[0].Id, 0);
            CheckDeviceObjectResult(deviceObjects[0].DeviceObject);
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
                    87 02 41 7F 07 61 01 36 01
                    C1 03 7F";

            var accessControlObjectService = new AccessControlObjectService();
            var parsedAccessControlObjects = accessControlObjectService.ReadPayloadForMultipleAccessControlObjectInstances(tlvPayloadBytes).ToList();

            CheckAccessControlObjectInstances(parsedAccessControlObjects);
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


        private void CheckDeviceObjectResult(DeviceObject deviceObject)
        {
            Assert.AreEqual("Open Mobile Alliance", deviceObject.Manufacturer);
            Assert.AreEqual("Lightweight M2M Client", deviceObject.ModelNumber);
            Assert.AreEqual("345000123", deviceObject.SerialNumber);
            Assert.AreEqual("1.0", deviceObject.FirmwareVersion);

            var expectedAvailablePowerSources = new List<TLVResourceInstance>()
            {
                new TLVResourceInstance()
                {
                    Id = 0,
                    Value = 1
                },
                new TLVResourceInstance()
                {
                    Id = 1,
                    Value = 5
                },
            };


            Assert.AreEqual(expectedAvailablePowerSources[0].Id, deviceObject.AvailablePowerSources[0].Id);
            Assert.AreEqual(expectedAvailablePowerSources[0].Value, deviceObject.AvailablePowerSources[0].Value);
            Assert.AreEqual(expectedAvailablePowerSources[1].Id, deviceObject.AvailablePowerSources[1].Id);
            Assert.AreEqual(expectedAvailablePowerSources[1].Value, deviceObject.AvailablePowerSources[1].Value);

            var expectedPowerSourceVoltages = new List<TLVResourceInstance>()
            {
                new TLVResourceInstance()
                {
                    Id = 0,
                    Value = 3800
                },
                new TLVResourceInstance()
                {
                    Id = 1,
                    Value = 5000
                },
            };

            Assert.AreEqual(expectedPowerSourceVoltages[0].Id, deviceObject.PowerSourceVoltage[0].Id);
            Assert.AreEqual(expectedPowerSourceVoltages[0].Value, deviceObject.PowerSourceVoltage[0].Value);
            Assert.AreEqual(expectedPowerSourceVoltages[1].Id, deviceObject.PowerSourceVoltage[1].Id);
            Assert.AreEqual(expectedPowerSourceVoltages[1].Value, deviceObject.PowerSourceVoltage[1].Value);

            var expectedPowerSourceCurrents = new List<TLVResourceInstance>()
            {
                new TLVResourceInstance()
                {
                    Id = 0,
                    Value = 125
                },
                new TLVResourceInstance()
                {
                    Id = 1,
                    Value = 900
                },
            };

            Assert.AreEqual(expectedPowerSourceCurrents[0].Id, deviceObject.PowerSourceCurrent[0].Id);
            Assert.AreEqual(expectedPowerSourceCurrents[0].Value, deviceObject.PowerSourceCurrent[0].Value);
            Assert.AreEqual(expectedPowerSourceCurrents[1].Id, deviceObject.PowerSourceCurrent[1].Id);
            Assert.AreEqual(expectedPowerSourceCurrents[1].Value, deviceObject.PowerSourceCurrent[1].Value);

            Assert.AreEqual(100, deviceObject.BatteryLevel);
            Assert.AreEqual(15, deviceObject.MemoryFree);

            var expectedErrorCode = new TLVResourceInstance()
            {
                Id = 0,
                Value = 0
            };

            Assert.AreEqual(deviceObject.ErrorCode[0].Id, expectedErrorCode.Id);
            Assert.AreEqual(deviceObject.ErrorCode[0].Value, expectedErrorCode.Value);

            var expectedCurrentTime = DateTimeOffset.FromUnixTimeSeconds(1367491215).DateTime;
            Assert.AreEqual(expectedCurrentTime, deviceObject.CurrentTime);


            Assert.AreEqual("+02:00", deviceObject.UtcOffset);
            Assert.AreEqual("U", deviceObject.SupportedBindingAndModes);
        }

        private void CheckAccessControlObjectInstances(List<MultipleAccessControlObject> accessControlObjects)
        {
            var expectedAccessControlObjects = new List<MultipleAccessControlObject>()
            {
                new MultipleAccessControlObject()
                {
                    Id = 0,
                    AccessControlObject = new AccessControlObject()
                    {
                        ObjectId = 1,
                        ObjectInstanceId = 0,
                        ACL = new List<AccessControlResourceInstance>()
                        {
                            new AccessControlResourceInstance()
                            {
                                Id = 127,
                                Value = 7,
                                ByteValue = "00000111"
                            }
                        },
                        AccessControlOwner = 127
                    },
                },
                new MultipleAccessControlObject()
                {
                    Id = 2,
                    AccessControlObject = new AccessControlObject()
                    {
                        ObjectId = 3,
                        ObjectInstanceId = 0,
                        ACL = new List<AccessControlResourceInstance>()
                        {
                            new AccessControlResourceInstance()
                            {
                                Id = 127,
                                Value = 7,
                                ByteValue = "00000111"
                            },
                            new AccessControlResourceInstance()
                            {
                                Id = 310,
                                Value = 1,
                                ByteValue = "00000001"
                            }
                        },
                        AccessControlOwner = 127
                    }
                },
            };

            for (var accessControlObjectIndex = 0; accessControlObjectIndex < accessControlObjects.Count; accessControlObjectIndex++)
            {
                var realAccessControlItem = accessControlObjects[accessControlObjectIndex];
                var expectedAccessControlItem = expectedAccessControlObjects[accessControlObjectIndex];

                Assert.AreEqual(expectedAccessControlItem.Id, realAccessControlItem.Id);

                Assert.AreEqual(expectedAccessControlItem.AccessControlObject.ObjectId,
                    realAccessControlItem.AccessControlObject.ObjectId);

                Assert.AreEqual(expectedAccessControlItem.AccessControlObject.ObjectInstanceId,
                    realAccessControlItem.AccessControlObject.ObjectInstanceId);

                for (var accessControlObjectACLIndex = 0;
                    accessControlObjectACLIndex < realAccessControlItem.AccessControlObject.ACL.Count;
                    accessControlObjectACLIndex++)
                {

                    var realACLItem = realAccessControlItem.AccessControlObject.ACL[accessControlObjectACLIndex];
                    var expectedACLItem = expectedAccessControlItem.AccessControlObject.ACL[accessControlObjectACLIndex];


                    Assert.AreEqual(expectedACLItem.Id, realACLItem.Id);
                    Assert.AreEqual(expectedACLItem.Value, realACLItem.Value);
                    Assert.AreEqual(expectedACLItem.ByteValue, realACLItem.ByteValue);
                }

                Assert.AreEqual(expectedAccessControlItem.AccessControlObject.AccessControlOwner,
                    realAccessControlItem.AccessControlObject.AccessControlOwner);
            }
        }
    }
}
