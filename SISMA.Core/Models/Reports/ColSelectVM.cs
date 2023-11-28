namespace SISMA.Core.Models.Reports
{
    public class ColSelectVM
    {
        public int StatReportId { get; set; }
        public int CatalogId { get; set; }

        public string Label { get; set; }

        public string[] SelectedList { get; set; }

        public string SelectedIds
        {
            get
            {
                return string.Join(",", SelectedList);
            }
        }
    }
}
