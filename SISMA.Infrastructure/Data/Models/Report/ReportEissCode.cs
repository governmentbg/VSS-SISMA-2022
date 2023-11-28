using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    public class ReportEissCode
    {
        [Key]
        public long Id { get; set; }

        public long ReportEissId { get; set; }

        public int CaseCodeId { get; set; }

        public int CountValue { get; set; }


        [ForeignKey(nameof(ReportEissId))]
        public virtual ReportEiss ReportEiss { get; set; }

        [ForeignKey(nameof(CaseCodeId))]
        public virtual NomCaseCode CaseCode { get; set; }
    }
}
