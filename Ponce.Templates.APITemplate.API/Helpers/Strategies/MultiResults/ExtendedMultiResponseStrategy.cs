using Mapster;
using Ponce.Templates.APITemplate.API.Dtos.BaseResponses;
using Ponce.Templates.APITemplate.Application.Dtos.MultiResult;

namespace Ponce.Templates.APITemplate.API.Helpers.Strategies.MultiResults;

public class ExtendedMultiResponseStrategy : IMultiResponseStrategy
{
    public MultiResponse GetResponse(MultiResultHandler results)
    {
        return new ExtendedMultiResponse
        {
            StatusCode = 206,
            Processed = results.Processed,
            Failed = results.Failed,
            Errors = results.Errors.Adapt<List<FailedMultiResponseResult>>(),
            Success = results.Success.Adapt<List<ProcessedResponseResult>>()
        };
    }
}
