using SISMA.Infrastructure.Contracts;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Common
{
    /// <summary>
    /// Съдилища
    /// </summary>
    [Display(Name = "Съдилища")]
    public class CommonCourt : BaseObjectParentNomenclature, ICommonLocationEntity
    {
        public int CourtTypeId { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        [Display(Name = "Населено място")]
        public string CityCode { get; set; }

        public int? ApealRegionId { get; set; }

        [ForeignKey(nameof(CourtTypeId))]
        public virtual NomCourtType CourtType { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual CommonCourt Parent { get; set; }

        [ForeignKey(nameof(ApealRegionId))]
        public virtual NomApealRegion ApealRegion { get; set; }

        ICommonNomenclature ICommonLocationEntity.EntityType
        {
            get => this.CourtType;
        }
    }
}
