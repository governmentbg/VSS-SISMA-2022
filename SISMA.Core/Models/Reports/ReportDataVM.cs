using SISMA.Core.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace SISMA.Core.Models.Reports
{
    public class ReportDataVM
    {
        public long Id { get; set; }
        public DateTime ReportDate { get; set; }
        public int IntegrationId { get; set; }
        [Display(Name = "Интеграция")]
        public string IntegrationName { get; set; }
        [Display(Name = "Каталог")]
        public string CatalogName { get; set; }
        [Display(Name = "Период")]
        public int PeriodNo { get; set; }
        public string PeriodName { get; set; }
        [Display(Name = "Година")]
        public int PeriodYear { get; set; }
        public string ReportSourceName { get; set; }
        [Display(Name = "Статус")]
        public int ReportStateId { get; set; }
        public string StatusName { get; set; }
    }

    public class FilterReportData
    {
        public long? Id { get; set; }
        [Display(Name = "Интеграция")]
        public int? IntegrationId { get; set; }
        [Display(Name = "Каталог")]
        public int? CatalogId { get; set; }
        [Display(Name = "Дата от")]
        public DateTime? DateFrom { get; set; }
        [Display(Name = "Дата до")]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Период")]
        public int? PeriodNo { get; set; }
        [Display(Name = "Година")]
        public int? PeriodYear { get; set; }

        [Display(Name = "Статус")]
        public int? ReportStateId { get; set; }

        public void Sanitize()
        {
            IntegrationId = IntegrationId.EmptyToNull();
            CatalogId = CatalogId.EmptyToNull();
            PeriodNo = PeriodNo.EmptyToNull();
            PeriodYear = PeriodYear.EmptyToNull();
            ReportStateId = ReportStateId.EmptyToNull();
        }
    }
}
