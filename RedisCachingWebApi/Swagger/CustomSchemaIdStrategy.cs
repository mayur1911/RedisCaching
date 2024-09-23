using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RedisCachingWebApi.Swagger
{
    public class CustomSchemaIdStrategy : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Set a unique title for the schema based on the type's declaring type and name
            var typeName = context.Type.Name;
            if (context.Type.DeclaringType != null)
            {
                schema.Title = $"{context.Type.DeclaringType.Name}.{typeName}";
            }
            else
            {
                schema.Title = typeName;
            }
        }
    }

    public static class CustomSchemaIdExtensions
    {
        public static void UseCustomSchemaIds(this SwaggerGenOptions options)
        {
            options.CustomSchemaIds(type =>
            {
                // Use the full type name (including namespace) to ensure unique schema IDs
                return type.FullName.Replace('+', '.');
            });
        }
    }
}
