using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Common
{
    /// <summary>
    /// Лица 
    /// </summary>
    [Display(Name = "Лица")]
    public class CommonSubject : BaseCommonNomenclature
    {
        public int SubjectTypeId { get; set; }

        [ForeignKey(nameof(SubjectTypeId))]
        public virtual NomSubjectType SubjectType { get; set; }
    }
}
