using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Ekatte
{
    public class EkRegion
    {
        [Key]
        public int Id { get; set; }
        public string Raion { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Document { get; set; }

        public int? SettlementId { get; set; }

        public string EisppCode { get; set; }


        [ForeignKey(nameof(SettlementId))]
        public EkEkatte Settlement { get; set; }
    }
}
