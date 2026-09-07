namespace Ponce.Templates.APITemplate.Application.Dtos.MultiResult;

public class Result
{
    public string Key { get; }
    public bool IsSuccess { get; }
    public string Message { get; }
    public string Action { get; }

    internal Result(string key, bool isSuccess, string message, string action = "")
    {
        Key = key;
        IsSuccess = isSuccess;
        Message = message;
        Action = action;
    }
}
