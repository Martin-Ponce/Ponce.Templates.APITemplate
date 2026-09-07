namespace Ponce.Templates.APITemplate.API.Dtos.Configurations;

public class CorsConfiguration(string[] origins, string[] headers)
{
    public string[] Origins { get; } = origins;
    public string[] Headers { get; } = headers;
}
