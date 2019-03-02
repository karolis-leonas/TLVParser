using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TLVParser.Services.DeviceObjectInstanceService;

namespace TLVParser
{
    public class Program
    {
        static void Main(string[] args)
        {

            const string tlvPayloadBytes = @"C8 00 14 4F 70 65 6E 20 4D 6F 62 69 6C 65 20 41 6C 6C 69 61 6E 63 65
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

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IDeviceObjectInstanceService, DeviceObjectInstanceService>()
                .AddSingleton<ITLVParserService, TLVParserService>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            //do the actual work here
            var deviceObjectInstanceService = serviceProvider.GetService<IDeviceObjectInstanceService>();
            var result = deviceObjectInstanceService.ReadPayloadForSingleObjectInstance(tlvPayloadBytes);

            logger.LogDebug("TLV payload has been parsed!");
            logger.LogDebug("Press enter to close...");
            Console.ReadLine();
        }
    }
}
