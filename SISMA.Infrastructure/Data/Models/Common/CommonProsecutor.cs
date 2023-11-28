using SISMA.Infrastructure.Contracts;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Common
{
    /// <summary>
    /// Прокуратури
    /// </summary>
    [Display(Name = "Прокуратури")]
    public class CommonProsecutor : BaseObjectParentNomenclature, ICommonLocationEntity
    {
        public int ProsecutorTypeId { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        [Display(Name = "Населено място")]
        public string CityCode { get; set; }

        public int? ApealRegionId { get; set; }

        public string EisppCode { get; set; }

        [ForeignKey(nameof(ProsecutorTypeId))]
        public virtual NomProsecutorType ProsecutorType { get; set; }

        [ForeignKey(nameof(ApealRegionId))]
        public virtual NomApealRegion ApealRegion { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual CommonProsecutor Parent { get; set; }

        ICommonNomenclature ICommonLocationEntity.EntityType
        {
            get => this.ProsecutorType;
        }
    }
}
