using SISMA.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SISMA.Core.Models.StatReport
{
    public class StatReportGetDataVM
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int ReportTypeId { get; set; }
        public string RowLabel { get; set; }
        public string DefaultRowLabel { get; set; }
        public int IntegrationId { get; set; }
        public int CatalogId { get; set; }
        public int? SecondIntegrationId { get; set; }
        public int? SecondCatalogId { get; set; }
        public int? SecondCatalogCodeId { get; set; }
        public int? RatioMultiplier { get; set; }
        public int PeriodNo { get; set; }
        public int PeriodYear { get; set; }

        public string ReportEntityList { get; set; }
        public int[] FilterEntityList { get; set; }

        public List<StatReportColVM> Columns { get; set; }

        public int[] CatalogCodeIds
        {
            get
            {
                return this.Columns.Select(x => x.CatalogCodeId).ToArray();
            }
        }

        public int[] EntityList
        {
            get
            {
                if (FilterEntityList.Length > 0)
                {
                    return FilterEntityList;
                }
                return ReportEntityList.ToIntArray();
            }
        }

        public int[] Last3Years
        {
            get
            {
                return Enumerable.Range(PeriodYear - 2, 3).ToArray();
            }
        }

    }
}
