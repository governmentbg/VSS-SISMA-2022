using SISMA.Infrastructure.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Категория статичен отчет
    /// </summary>
    [Display(Name = "Категория статичен отчет")]
    public class NomStatReportCategory : BaseCommonNomenclature
    {
        [Display(Name = "Интеграция")]
        [Range(1, int.MaxValue, ErrorMessage = "Изберете {0}.")]
        [AddToLog]
        public int IntegrationId { get; set; }

        [ForeignKey(nameof(IntegrationId))]
        public virtual NomIntegration Integration { get; set; }
    }
}
