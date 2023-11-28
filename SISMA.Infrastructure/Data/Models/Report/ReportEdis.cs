using SISMA.Infrastructure.Data.Models.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от ЕДИС, по съдилища
    /// </summary>
    public class ReportEdis : BaseReportDetails
    {
        public int CourtId { get; set; }

        public int CourtObjectId { get; set; }

        [ForeignKey(nameof(CourtId))]
        public virtual CommonCourt Court { get; set; }
        public virtual ICollection<ReportEdisSubject> Subjects { get; set; }
    }
}
