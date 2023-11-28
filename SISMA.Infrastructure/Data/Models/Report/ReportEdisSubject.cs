using SISMA.Infrastructure.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от ЕДИС, по съдилища/съдии
    /// </summary>
    public class ReportEdisSubject
    {
        [Key]
        public long Id { get; set; }

        public long ReportEdisId { get; set; }

        public int SubjectId { get; set; }

        public int CountValue { get; set; }

        [ForeignKey(nameof(ReportEdisId))]
        public virtual ReportEdis ReportEdis { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual CommonSubject Subject { get; set; }
    }
}
