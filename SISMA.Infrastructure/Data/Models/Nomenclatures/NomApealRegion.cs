using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Апелативен район
    /// </summary>
    [Display(Name = "Апелативен район")]
    public class NomApealRegion : BaseCommonNomenclature
    {
        public int? ApealRegionType { get; set; }        
    }
}
