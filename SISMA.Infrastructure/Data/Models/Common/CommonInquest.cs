using SISMA.Infrastructure.Contracts;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Common
{
    /// <summary>
    /// Следствия/Следствени отдели
    /// </summary>
    [Display(Name = "Следствия/Следствени отдели")]
    public class CommonInquest : BaseObjectParentNomenclature, ICommonLocationEntity
    {
        public int InquestTypeId { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        [Display(Name = "Населено място")]
        public string CityCode { get; set; }

        public int? ApealRegionId { get; set; }

        [ForeignKey(nameof(InquestTypeId))]
        public virtual NomInquestType InquestType { get; set; }

        [ForeignKey(nameof(ApealRegionId))]
        public virtual NomApealRegion ApealRegion { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual CommonProsecutor Parent { get; set; }
        ICommonNomenclature ICommonLocationEntity.EntityType
        {
            get => this.InquestType;
        }
    }
}
