using SISMA.Infrastructure.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Статистически кодове за групиране на данни в статистически отчет - по редове
    /// </summary>
    [Display(Name = "Кодове към ")]
    public class NomStatReportCode : IOrderable
    {
        [Key]
        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public int StatReportId { get; set; }

        [Display(Name = "Шифър")]
        [AddToLog]
        public int CaseCodeId { get; set; }

        [Display(Name = "Наименование")]
        [AddToLog]
        public string Label { get; set; }

        [Display(Name = "Активен запис")]
        [AddToLog]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(StatReportId))]
        public virtual NomStatReport StatReport { get; set; }

        [ForeignKey(nameof(CaseCodeId))]
        public virtual NomCaseCode CaseCode { get; set; }
    }
}
