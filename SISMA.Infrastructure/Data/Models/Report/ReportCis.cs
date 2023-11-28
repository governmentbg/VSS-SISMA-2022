using SISMA.Infrastructure.Data.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от ЦИССС, по следствени отдели
    /// </summary>
    public class ReportCis : BaseReportDetails
    {
        public int InquestId { get; set; }

        public int InquestObjectId { get; set; }

        [ForeignKey(nameof(InquestId))]
        public virtual CommonInquest Inquest { get; set; }
    }
}
