using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Ekatte
{
    public class EkStreet
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Ekatte { get; set; }

        public string Name { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
