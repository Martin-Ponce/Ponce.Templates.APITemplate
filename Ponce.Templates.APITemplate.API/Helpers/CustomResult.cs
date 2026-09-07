using System.Text.Json;
using Ponce.Templates.APITemplate.API.Dtos.BaseResponses;
using Ponce.Templates.APITemplate.API.Helpers.Factories;
using Ponce.Templates.APITemplate.Application.Dtos.MultiResult;

namespace Ponce.Templates.APITemplate.API.Helpers;

/// <summary>
/// Builds an HTTP 206 (Partial Content) response for bulk operations, choosing between a simple
/// or an extended payload shape based on the "X-Response-Format" request header.
/// </summary>
public static class CustomResult
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    private static readonly IReadOnlyCollection<string> _allowedFormats =
    [
        ApiConstants.ResponseFormats.EXTENDED_MULTI_RESULT,
        ApiConstants.ResponseFormats.SIMPLE_MULTI_RESULT
    ];

    public static IResult PartialContent(MultiResultHandler results, HttpContext context)
    {
        var responseFormat = context.Request.Headers["X-Response-Format"].ToString();
        if (!_allowedFormats.Contains(responseFormat))
        {
            responseFormat = ApiConstants.ResponseFormats.SIMPLE_MULTI_RESULT;
        }
        var strategy = MultiResponseStrategyFactory.GetStrategy(responseFormat);
        var response = strategy.GetResponse(results);
        context.Response.Headers["X-Response-Format"] = responseFormat;
        if (response is ExtendedMultiResponse extendedResponse)
        {
            return Results.Content(JsonSerializer.Serialize(extendedResponse, _jsonSerializerOptions), contentType: responseFormat, statusCode: response.StatusCode);
        }
        return Results.Content(JsonSerializer.Serialize(response, _jsonSerializerOptions), contentType: responseFormat, statusCode: response.StatusCode);
    }
}
