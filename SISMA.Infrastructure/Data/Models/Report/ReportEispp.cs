using SISMA.Infrastructure.Data.Models.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от ЕИСПП, по следствени отдели
    /// </summary>
    public class ReportEispp : BaseReportDetails
    {
        public int InquestId { get; set; }

        public int InquestObjectId { get; set; }

        [ForeignKey(nameof(InquestId))]
        public virtual CommonInquest Inquest { get; set; }
        public virtual ICollection<ReportEisppSubject> Subjects { get; set; }
    }
}
