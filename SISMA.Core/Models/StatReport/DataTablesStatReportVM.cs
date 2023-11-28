using System.Collections.Generic;

namespace SISMA.Core.Models.StatReport
{
    public class DataTablesStatReportVM
    {
        public string ReportTitle { get; set; }
        public string ReportSubtitle { get; set; }
        public string RowLabel { get; set; }
        public int GroupType { get; set; }
        public bool NoData { get; set; }
        public List<DataTablesStatReportColumnsVM> Columns { get; set; }

        public dynamic[] Data { get; set; }
    }

    public class DataTablesStatReportColumnsVM
    {
        public string ColumnName { get; set; }
        public string Label { get; set; }
        public bool DataCol { get; set; }
        public bool MixedCol { get; set; }

        public DataTablesStatReportColumnsVM()
        {
            DataCol = true;
            MixedCol = false;
        }
    }
}
