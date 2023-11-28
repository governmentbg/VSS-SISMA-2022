using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    public class NomCaseCodeCatalog
    {
        public int Id { get; set; }
        public int CaseCodeId { get; set; }
        public int CatalogId { get; set; }

        [ForeignKey(nameof(CaseCodeId))]
        public virtual NomCaseCode CaseCode { get; set; }

        [ForeignKey(nameof(CatalogId))]
        public virtual NomCatalog Catalog { get; set; }
    }
}
