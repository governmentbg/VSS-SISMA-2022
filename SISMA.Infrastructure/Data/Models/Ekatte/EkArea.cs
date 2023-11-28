using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Ekatte
{
    public class EkArea
    {
        [Key]
        public int AreadId { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string Name { get; set; }

        public string Document { get; set; }

        public string Abc { get; set; }

        public string NameEn { get; set; }
    }
}

