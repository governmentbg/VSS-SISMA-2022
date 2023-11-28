using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Код на ред от справка
    /// </summary>
    [Display(Name = "Код на ред от справка")]
    public class NomCatalogCode : BaseCommonNomenclature
    {
        public int CatalogId { get; set; }

        [ForeignKey(nameof(CatalogId))]
        public virtual NomCatalog Catalog { get; set; }
    }
}
