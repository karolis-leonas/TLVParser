using System.Collections.Generic;
using TLVParser.Models.RequestsToObjects.RequestToObject65;
using TLVParser.Models.RequestsToObjects.RequestToObject66;

namespace TLVParser.Services.RequestsToObjects
{
    public interface IRequestToObjectService
    {
        IEnumerable<ExtendedRequestToObject65> ReadRequestToObject65WithMultipleInstances(string payload);
        IEnumerable<ExtendedRequestToObject66> ReadRequestToObject66WithMultipleInstances(string payload);

        RequestToObject65 ReadRequestToObject65WithSingleInstance(string payload);
        RequestToObject66 ReadRequestToObject66WithSingleInstance(string payload);
    }
}
