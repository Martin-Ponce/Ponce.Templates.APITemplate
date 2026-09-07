using Ponce.Templates.APITemplate.API.Dtos.BaseResponses;
using Ponce.Templates.APITemplate.Application.Dtos.MultiResult;

namespace Ponce.Templates.APITemplate.API.Helpers.Strategies.MultiResults;

public interface IMultiResponseStrategy
{
    MultiResponse GetResponse(MultiResultHandler results);
}
