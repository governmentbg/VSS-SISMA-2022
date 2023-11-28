using SISMA.Infrastructure.Contracts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Статистически отчет
    /// </summary>
    [Display(Name = "Статистически отчет")]
    public class NomStatReport : BaseCommonNomenclature, IOrderable
    {
        [Display(Name = "Категория")]
        [AddToLog]
        [Required(ErrorMessage = "Изберете {0}.")]
        [Range(1, int.MaxValue, ErrorMessage = "Изберете {0}.")]
        public int? ReportCategoryId { get; set; }

        [NotMapped]
        public int IntegrationId { get; set; }

        [Display(Name = "Каталог")]
        [Range(1, int.MaxValue, ErrorMessage = "Изберете {0}.")]
        [AddToLog]
        public int CatalogId { get; set; }

        [Display(Name = "Вид отчет")]
        [Range(1, int.MaxValue, ErrorMessage = "Изберете {0}.")]
        [AddToLog]
        public int ReportTypeId { get; set; }

        [Display(Name = "Наименование на група")]
        [AddToLog]
        public string RowLabel { get; set; }

        public string EntityList { get; set; }


        [Display(Name = "Вторична интеграция")]
        [AddToLog]
        public int? SecondIntegrationId { get; set; }

        [Display(Name = "Каталог")]
        [AddToLog]
        public int? SecondCatalogId { get; set; }

        [Display(Name = "ИБД код")]
        [AddToLog]
        public int? SecondCatalogCodeId { get; set; }

        [Display(Name = "Множител")]
        [AddToLog]
        public int? RatioMultiplier { get; set; }

        [ForeignKey(nameof(ReportCategoryId))]
        public virtual NomStatReportCategory ReportCategory { get; set; }

        [ForeignKey(nameof(CatalogId))]
        public virtual NomCatalog Catalog { get; set; }

        [ForeignKey(nameof(ReportTypeId))]
        public virtual NomStatReportType ReportType { get; set; }

        [ForeignKey(nameof(SecondIntegrationId))]
        public virtual NomIntegration SecondIntegration { get; set; }

        [ForeignKey(nameof(SecondCatalogId))]
        public virtual NomCatalog SecondCatalog { get; set; }

        [ForeignKey(nameof(SecondCatalogCodeId))]
        public virtual NomCatalogCode SecondCatalogCode { get; set; }

        public virtual ICollection<NomStatReportCode> Codes { get; set; }
        public virtual ICollection<NomStatReportCol> Columns { get; set; }
    }
}
