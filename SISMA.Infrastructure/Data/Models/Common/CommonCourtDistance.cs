using SISMA.Infrastructure.Data.Models.Ekatte;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Common
{
    /// <summary>
    /// Разстояния на неселени места до съдилища
    /// </summary>
    [Display(Name = "Разстояния на неселени места до съдилища")]
    public class CommonCourtDistance
    {
        [Key]
        public int Id { get; set; }
        public int CourtId { get; set; }

        public int EkEkatteId { get; set; }

        public decimal Distance { get; set; }
        public decimal Duration { get; set; }

        public int? DistanceType { get; set; }

        [ForeignKey(nameof(CourtId))]
        public virtual CommonCourt Court { get; set; }

        [ForeignKey(nameof(EkEkatteId))]
        public virtual EkEkatte Ekatte { get; set; }
    }
}
