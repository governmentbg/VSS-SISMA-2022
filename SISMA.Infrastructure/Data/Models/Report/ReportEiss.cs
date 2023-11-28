using SISMA.Infrastructure.Data.Models.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от ЕИСС, по съдилища
    /// </summary>
    public class ReportEiss : BaseReportDetails
    {
        public int CourtId { get; set; }

        public int CourtObjectId { get; set; }

        [ForeignKey(nameof(CourtId))]
        public virtual CommonCourt Court { get; set; }

        public virtual ICollection<ReportEissCode> Codes { get; set; }
        public virtual ICollection<ReportEissSubject> Subjects { get; set; }
        public virtual ICollection<ReportEissCourt> Courts { get; set; }
    }
}
