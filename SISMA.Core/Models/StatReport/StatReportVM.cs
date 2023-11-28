using System;
using System.ComponentModel.DataAnnotations;

namespace SISMA.Core.Models.StatReport
{
    public class StatReportVM
    {
        public int Id { get; set; }
        public int IntegrationId { get; set; }
        public string IntegrationName { get; set; }
        public string CategoryName { get; set; }
        public string CatalogName { get; set; }
        public string Label { get; set; }
        public string ReportTypeName { get; set; }
        public DateTime DateStart { get; set; }
    }

    public class StatReportColVM
    {
        public int Id { get; set; }
        public int CatalogCodeId { get; set; }
        public string IbdCode { get; set; }
        public string IbdName { get; set; }
        public string Label { get; set; }
    }

    public class StatReportCaseCodeVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string CodeName { get; set; }
        public string Label { get; set; }
    }

    public class FilterStatReportVM
    {
        [Display(Name = "Интеграция")]
        public int IntegrationId { get; set; }

        [Display(Name = "Категория")]
        public int? ReportCategoryId { get; set; }

        [Display(Name = "Каталог")]
        public int? CatalogId { get; set; }

        [Display(Name = "Наименование")]
        public string Label { get; set; }

        [Display(Name = "Вид отчет")]
        public int? ReportTypeId { get; set; }

        public void Sanitize()
        {
            if (ReportCategoryId <= 0)
            {
                ReportCategoryId = null;
            }


            if (CatalogId <= 0)
            {
                CatalogId = null;
            }

            if (ReportTypeId <= 0)
            {
                ReportTypeId = null;
            }
            if (string.IsNullOrEmpty(Label))
            {
                Label = null;
            }
        }
    }

    public class StatReportResultGroupVM
    {
        public int RowNo { get; set; }
        public string RowLabel { get; set; }
        public int ColNo { get; set; }
        public int Count { get; set; }
    }

    public class FilterStatReportCategoryVM
    {
        [Display(Name = "Интеграция")]
        public int? IntegrationId { get; set; }


        public void Sanitize()
        {
            if (IntegrationId <= 0)
            {
                IntegrationId = null;
            }

        }
    }

    public class StatReportCategoryVM
    {
        public int Id { get; set; }
        public string IntegrationName { get; set; }
        public string Label { get; set; }

        public DateTime DateStart { get; set; }

        public bool IsActive { get; set; }
    }
}
