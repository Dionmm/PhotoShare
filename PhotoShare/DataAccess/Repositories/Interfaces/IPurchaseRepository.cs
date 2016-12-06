using PhotoShare.DataAccess.Entities;

namespace PhotoShare.DataAccess.Repositories.Interfaces
{
    public interface IPurchaseRepository: IRepository<Purchase>
    {
        int GetNumberOfSalesByPhotoId(int photoId);

    }
}
