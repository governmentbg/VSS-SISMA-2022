using SISMA.Infrastructure.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от ЕИСПП, по прокурори
    /// </summary>
    public class ReportEisppSubject
    {
        [Key]
        public long Id { get; set; }

        public long ReportEisppId { get; set; }

        public int SubjectId { get; set; }

        public int CountValue { get; set; }


        [ForeignKey(nameof(ReportEisppId))]
        public virtual ReportEispp ReportEispp { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual CommonSubject Subject { get; set; }
    }
}
