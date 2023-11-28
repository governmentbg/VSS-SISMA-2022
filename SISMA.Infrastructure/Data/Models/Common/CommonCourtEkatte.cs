using SISMA.Infrastructure.Data.Models.Ekatte;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Common
{
    /// <summary>
    /// Връзка съдилища към населени места
    /// </summary>
    [Display(Name = "Връзка съдилища към населени места")]
    public class CommonCourtEkatte
    {
        public int CourtId { get; set; }

        public int EkEkatteId { get; set; }

        [ForeignKey(nameof(CourtId))]
        public virtual CommonCourt Court { get; set; }

        [ForeignKey(nameof(EkEkatteId))]
        public virtual EkEkatte Ekatte { get; set; }
    }
}
