using SISMA.Infrastructure.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    public class ReportUisSubject
    {
        [Key]
        public long Id { get; set; }

        public long ReportUisId { get; set; }

        public int SubjectId { get; set; }

        public int CountValue { get; set; }


        [ForeignKey(nameof(ReportUisId))]
        public virtual ReportUis ReportUis { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual CommonSubject Subject { get; set; }
    }
}
