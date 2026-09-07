using Mapster;
using Ponce.Templates.APITemplate.API.Dtos.BaseResponses;
using Ponce.Templates.APITemplate.Application.Dtos.MultiResult;

namespace Ponce.Templates.APITemplate.API.Helpers.Strategies.MultiResults;

public class SimpleMultiResponseStrategy : IMultiResponseStrategy
{
    public MultiResponse GetResponse(MultiResultHandler results)
    {
        return new MultiResponse
        {
            Processed = results.Processed,
            Failed = results.Failed,
            Errors = results.Errors.Adapt<List<FailedMultiResponseResult>>(),
            StatusCode = 206
        };
    }
}
