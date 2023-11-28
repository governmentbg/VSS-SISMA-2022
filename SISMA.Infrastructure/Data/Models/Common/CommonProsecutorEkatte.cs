using SISMA.Infrastructure.Data.Models.Ekatte;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Common
{
    /// <summary>
    /// Връзка прокуратури към населени места
    /// </summary>
    [Display(Name = "Връзка прокуратури към населени места")]
    public class CommonProsecutorEkatte
    {
        public int ProsecutorId { get; set; }

        public int EkEkatteId { get; set; }

        [ForeignKey(nameof(ProsecutorId))]
        public virtual CommonProsecutor Prosecutor { get; set; }

        [ForeignKey(nameof(EkEkatteId))]
        public virtual EkEkatte Ekatte { get; set; }
    }
}
