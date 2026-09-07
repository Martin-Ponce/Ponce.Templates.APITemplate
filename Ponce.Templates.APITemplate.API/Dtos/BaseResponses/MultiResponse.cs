namespace Ponce.Templates.APITemplate.API.Dtos.BaseResponses;

public class MultiResponse
{
    public int StatusCode { get; set; } = 206;
    public int Processed { get; set; }
    public int Failed { get; set; }
    public IEnumerable<FailedMultiResponseResult> Errors { get; set; } = [];
}
