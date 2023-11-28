using SISMA.Infrastructure.Data.Models.Ekatte;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от НСИ, по населени места
    /// </summary>
    public class ReportNsi : BaseReportDetails
    {
        public int EkatteId { get; set; }

        public string CityCode { get; set; }

        public decimal AmmountValue { get; set; }

        [ForeignKey(nameof(EkatteId))]
        public virtual EkEkatte Ekatte { get; set; }
    }
}
