namespace Chapter13.Commands;

public class CommandResult
{
    public Guid CorrelationId { get; init; }
    public bool IsSuccess => IsAuthorized && IsValid && !HasExceptions;
    public bool IsAuthorized { get; init; } = true;
    public bool IsValid => !ValidationResults.Any();
    public bool HasExceptions => ExceptionMessages.Any();
    public IEnumerable<ValidationResult> ValidationResults { get; init; } = Enumerable.Empty<ValidationResult>();
    public IEnumerable<string> ExceptionMessages { get; init; } = Enumerable.Empty<string>();
    public string ExceptionStackTrace { get; init; } = string.Empty;
    public object? Response { get; init; }
}
