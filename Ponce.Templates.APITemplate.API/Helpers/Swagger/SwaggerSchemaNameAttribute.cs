namespace Ponce.Templates.APITemplate.API.Helpers.Swagger;

[AttributeUsage(AttributeTargets.Class)]
public class SwaggerSchemaNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
