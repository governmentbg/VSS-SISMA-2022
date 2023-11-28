using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    /// <summary>
    /// Статистически шифри на дело
    /// </summary>
    [Display(Name = "Статистически шифри на дело")]
    public class NomCaseCode : BaseCommonNomenclature
    {

        public virtual ICollection<NomCaseCodeCatalog> CaseCodeCatalogs { get; set; }

        public NomCaseCode()
        {
            CaseCodeCatalogs = new HashSet<NomCaseCodeCatalog>();
        }
    }
}
