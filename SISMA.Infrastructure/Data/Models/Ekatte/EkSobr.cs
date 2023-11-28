using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Ekatte
{
    public class EkSobr
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        public string Ekatte { get; set; }

        public string Kind { get; set; }

        public string Name { get; set; }

        public string Area1 { get; set; }

        public string Area2 { get; set; }

        public string Document { get; set; }

        public string Abc { get; set; }
    }
}
