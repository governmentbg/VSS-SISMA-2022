namespace SISMA.Infrastructure.Contracts
{
    public interface IObjectParentNomenclature : ICommonNomenclature
    {
        int ObjectId { get; set; }

        int? ParentId { get; set; }

        int? ParentObjectId { get; set; }
    }
}
