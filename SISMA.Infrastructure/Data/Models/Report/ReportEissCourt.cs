using SISMA.Infrastructure.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    public class ReportEissCourt
    {
        [Key]
        public long Id { get; set; }

        public long ReportEissId { get; set; }

        public int CourtId { get; set; }

        public int CourtObjectId { get; set; }

        public int CountValue { get; set; }


        [ForeignKey(nameof(ReportEissId))]
        public virtual ReportEiss ReportEiss { get; set; }

        [ForeignKey(nameof(CourtId))]
        public virtual CommonCourt Court { get; set; }
    }
}
