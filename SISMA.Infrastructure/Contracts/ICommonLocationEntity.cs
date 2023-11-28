using SISMA.Infrastructure.Data.Models.Nomenclatures;

namespace SISMA.Infrastructure.Contracts
{
    public interface ICommonLocationEntity : IObjectParentNomenclature
    {
        string Longitude { get; set; }

        string Latitude { get; set; }

        string CityCode { get; set; }

        ICommonNomenclature EntityType { get; }
    }
}
