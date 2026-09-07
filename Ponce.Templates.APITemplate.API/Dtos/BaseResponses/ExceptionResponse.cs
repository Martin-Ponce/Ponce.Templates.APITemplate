namespace Ponce.Templates.APITemplate.API.Dtos.BaseResponses;

public class ExceptionResponse
{
    public int StatusCode { get; set; } = 500;
    public bool IsError { get; set; } = true;
    public required ErrorObjectResponse Error { get; set; }
}
