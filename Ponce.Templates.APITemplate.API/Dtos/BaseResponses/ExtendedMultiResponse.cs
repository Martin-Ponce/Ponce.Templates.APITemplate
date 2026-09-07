namespace Ponce.Templates.APITemplate.API.Dtos.BaseResponses;

public class ExtendedMultiResponse : MultiResponse
{
    public IEnumerable<ProcessedResponseResult> Success { get; set; } = [];
}
