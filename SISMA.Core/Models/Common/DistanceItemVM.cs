using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SISMA.Core.Models.Common
{
    public class FilterEkatteItemVM
    {
        [Display(Name = "Населено място")]
        public string EkatteCode { get; set; }

        [Display(Name = "Населено място")]
        public string CityName { get; set; }

        [Display(Name = "Вид съд")]
        public string CourtTypes { get; set; }

        [Display(Name = "Съдилища")]
        public int DistanceType { get; set; }

        public FilterEkatteItemVM()
        {
            CourtTypes = "7,8,10,11";
        }
    }

    public class EkatteItemVM
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public string MunicipalityName { get; set; }
        public string Latitude { get; set; }
        public string Longitute { get; set; }
        public List<DistanceItemVM> Distances { get; set; }
    }
    public class DistanceItemVM
    {
        public int Id { get; set; }
        public string CourtName { get; set; }
        public decimal Distance { get; set; }
        public decimal Duration { get; set; }
        public string Latitude { get; set; }
        public string Longitute { get; set; }
    }
}
