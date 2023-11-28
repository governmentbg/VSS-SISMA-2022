using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Ekatte
{
    public class EkDistrict
    {
        [Key]
        public int DistrictId { get; set; }

        public string Oblast { get; set; }

        public string Ekatte { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public string Document { get; set; }

        public string Abc { get; set; }

        public int CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public EkCountry Country { get; set; }


    }
}
