using System.Text.Json.Serialization;

namespace ReceitasCulinarias.API.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<string>? Errors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StackTrace { get; set; }

    public ErrorResponse(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public ErrorResponse(int statusCode, string message, IEnumerable<string> errors)
        : this(statusCode, message)
    {
        Errors = errors;
    }
}
