namespace Ponce.Templates.APITemplate.API.Dtos.BaseResponses;

public class ErrorObjectResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public IEnumerable<object> Data { get; set; } = [];
}
