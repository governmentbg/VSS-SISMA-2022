using SISMA.Infrastructure.Data.Models.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от УИС, по прокуратури
    /// </summary>
    public class ReportUis : BaseReportDetails
    {
        public int ProsecutorId { get; set; }

        public int ProsecutorObjectId { get; set; }

        [ForeignKey(nameof(ProsecutorId))]
        public virtual CommonProsecutor Prosecutor { get; set; }
        //public virtual ICollection<ReportUisSubject> Subjects { get; set; }
    }
}
