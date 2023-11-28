using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    public class BaseReportDetails
    {
        [Key]
        public long Id { get; set; }

        public long ReportDataId { get; set; }

        public int CatalogCodeId { get; set; }

        public int CountValue { get; set; }

        [ForeignKey(nameof(ReportDataId))]
        public virtual ReportData ReportData { get; set; }

        [ForeignKey(nameof(CatalogCodeId))]
        public virtual NomCatalogCode CatalogCode { get; set; }
    }
}
