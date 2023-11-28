using SISMA.Infrastructure.Data.Models.Ekatte;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Данни от ЕИСПП, по общини
    /// </summary>
    public class ReportEisppMunicipality : BaseReportDetails
    {
        public int MunicipalityId { get; set; }

        public string CityCode { get; set; }

        [ForeignKey(nameof(MunicipalityId))]
        public virtual EkMunincipality Munincipality { get; set; }
    }
}
