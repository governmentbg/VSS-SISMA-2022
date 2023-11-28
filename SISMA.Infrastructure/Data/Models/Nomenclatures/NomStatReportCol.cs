using SISMA.Infrastructure.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Колони в статистически отчет
    /// </summary>
    [Display(Name = "Кодове към елемент от статистически отчет - ИБД и статистически")]
    public class NomStatReportCol : IOrderable
    {
        [Key]
        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public int StatReportId { get; set; }

        [Display(Name ="Код по ИБД")]
        [AddToLog]
        public int CatalogCodeId { get; set; }


        [Display(Name ="Наименование")]
        [AddToLog]
        public string Label { get; set; }

        [Display(Name ="Активен запис")]
        [AddToLog]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(StatReportId))]
        public virtual NomStatReport StatReport { get; set; }

        [ForeignKey(nameof(CatalogCodeId))]
        public virtual NomCatalogCode CatalogCode { get; set; }
    }
}
