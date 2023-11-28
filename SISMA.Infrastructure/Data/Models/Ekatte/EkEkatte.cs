using SISMA.Infrastructure.Data.Models.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Ekatte
{
    public class EkEkatte
    {
        [Key]
        public int Id { get; set; }

        public string Ekatte { get; set; }

        public string TVM { get; set; }

        public string Name { get; set; }

        public string Oblast { get; set; }

        public string Obstina { get; set; }

        public string Kmetstvo { get; set; }

        public string Kind { get; set; }

        public string Category { get; set; }

        public string Altitude { get; set; }

        public string Document { get; set; }

        public string Tsb { get; set; }

        public string Abc { get; set; }

        public string Lat { get; set; }
        public string Lon { get; set; }

        public int? DistrictId { get; set; }

        public int CountryId { get; set; }

        public int? MunicipalId { get; set; }

        public string EisppCode { get; set; }

        [ForeignKey(nameof(DistrictId))]
        public EkDistrict District { get; set; }

        [ForeignKey(nameof(CountryId))]
        public EkCountry Country { get; set; }

        [ForeignKey(nameof(MunicipalId))]
        public EkMunincipality Munincipality { get; set; }

        public virtual ICollection<CommonCourtDistance> Distances { get; set; }

    }
}
