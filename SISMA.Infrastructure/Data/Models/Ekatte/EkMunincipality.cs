using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Ekatte
{
    public class EkMunincipality
    {
        [Key]
        public int MunicipalityId { get; set; }

        public string Municipality { get; set; }

        public string Ekatte { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Document { get; set; }

        public string Abc { get; set; }

        public int? DistrictId { get; set; }

        [ForeignKey(nameof(DistrictId))]
        public EkDistrict District { get; set; }
    }
}
