using System.ComponentModel.DataAnnotations;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Вид групиране за статичен отчет
    /// </summary>
    [Display(Name = "Вид групиране за статичен отчет")]
    public class NomStatReportType : BaseCommonNomenclature
    {
        public string DefaultRowLabel { get; set; }
    }
}
