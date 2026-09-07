namespace Ponce.Templates.APITemplate.Application.Dtos.MultiResult;

/// <summary>
/// Accumulates the outcome of a batch/bulk operation (one that processes many items and wants to
/// report per-item success/failure instead of failing the whole request). Used together with
/// the API layer's MultiResponse DTOs and <c>CustomResult.PartialContent</c>.
/// </summary>
public class MultiResultHandler
{
    private readonly List<Result> _success = [];
    private readonly List<Result> _errors = [];

    public IReadOnlyCollection<Result> Success => _success.AsReadOnly();
    public IReadOnlyCollection<Result> Errors => _errors.AsReadOnly();
    public int Processed => _success.Count + _errors.Count;
    public int Failed => _errors.Count;

    public void AddCreateAction(string key, string message) => _success.Add(new Result(key, true, message, "Created"));
    public void AddUpdateAction(string key, string message) => _success.Add(new Result(key, true, message, "Updated"));
    public void AddDeleteAction(string key, string message) => _success.Add(new Result(key, true, message, "Deleted"));
    public void AddError(string key, string message) => _errors.Add(new Result(key, false, message));
}
