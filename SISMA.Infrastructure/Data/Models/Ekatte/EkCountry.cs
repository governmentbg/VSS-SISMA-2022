using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Ekatte
{
    public class EkCountry
    {
        [Key]
        public int CountryId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string EISPPCode { get; set; }
    }
}
