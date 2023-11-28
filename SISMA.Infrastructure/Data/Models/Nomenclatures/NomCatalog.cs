using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Вид справка
    /// </summary>
    [Display(Name = "Вид справка")]
    public class NomCatalog : BaseCommonNomenclature
    {
        public int IntegrationId { get; set; }

        /// <summary>
        /// 1-Месечен,3-тримесечен,6-шестмесечен,12-годишен
        /// </summary>
        public int PeriodType { get; set; }

        public int? DetailType { get; set; }

        [ForeignKey(nameof(IntegrationId))]
        public virtual NomIntegration Integration { get; set; }


    }
}
