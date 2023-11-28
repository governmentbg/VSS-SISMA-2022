using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace SISMA.Api.Filters
{
    /// <summary>
    /// Филтър за добавяне на коментар към контролер в OpenApi
    /// </summary>
    public class ApplyTagDescriptions : IDocumentFilter
    {
        /// <summary>
        /// Добавя коментар в контролерите
        /// </summary>
        /// <param name="swaggerDoc">Документ, съдържащ документацията</param>
        /// <param name="context">Контекст на заявката</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Operations will have been tagged with corresponding controller name (Swashbuckle default)
            // To provide descriptions with these tags, provide an entry below for each controller
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = "Data", Description = "Подаване на пакети със статистически данни" }
            };
        }
    }
}
