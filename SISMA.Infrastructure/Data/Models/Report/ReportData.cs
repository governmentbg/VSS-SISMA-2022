using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Report
{
    /// <summary>
    /// Регистър на входящите файлове
    /// </summary>
    [Display(Name = "Регистър на входящите файлове")]
    public class ReportData
    {
        [Key]
        public long Id { get; set; }

        public int IntegrationId { get; set; }
        public int ReportSourceId { get; set; }

        public int CatalogId { get; set; }

        /// <summary>
        /// Пореден номер месец, тримесечие или шестмесечие,1 при годишен
        /// </summary>
        public int ReportPeriod { get; set; }
        public int ReportYear { get; set; }

        public DateTime ReportDate { get; set; }

        public int ReportStateId { get; set; }

        [ForeignKey(nameof(IntegrationId))]
        public virtual NomIntegration Integration { get; set; }

        [ForeignKey(nameof(CatalogId))]
        public virtual NomCatalog Catalog { get; set; }

        [ForeignKey(nameof(ReportSourceId))]
        public virtual NomReportSource ReportSource { get; set; }

        [ForeignKey(nameof(ReportStateId))]
        public virtual NomReportState ReportState { get; set; }

        public virtual ICollection<ReportEiss> DetailsEISS { get; set; }
        public virtual ICollection<ReportEdis> DetailsEDIS { get; set; }
        public virtual ICollection<ReportCis> DetailsCIS { get; set; }
        public virtual ICollection<ReportUis> DetailsUIS { get; set; }
        public virtual ICollection<ReportEispp> DetailsEISPP { get; set; }
        public virtual ICollection<ReportEisppMunicipality> DetailsEisppMunicipality { get; set; }
        public virtual ICollection<ReportNsi> DetailsNSI { get; set; }
    }
}
