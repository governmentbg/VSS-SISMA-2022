using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISMA.Core.Models.StatReport
{
    public class StatReportFilterVM
    {
        public int IntegrationId { get; set; }

        [Display(Name = "Категория")]
        public int ReportCategoryId { get; set; }

        [Display(Name = "Отчет")]
        public int ReportTypeId { get; set; }

        [Display(Name = "Период")]
        public int PeriodNo { get; set; }

        [Display(Name = "Година")]
        public int PeriodYear { get; set; }
    }
}
