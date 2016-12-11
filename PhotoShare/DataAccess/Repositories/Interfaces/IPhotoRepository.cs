using System.Collections.Generic;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.DataAccess.Repositories.Interfaces
{
    public interface IPhotoRepository : IRepository<Photo>
    {
        IEnumerable<Photo> GetMostRecentPhotos(int pageSize, int page);
        IEnumerable<Photo> GetUsersMostRecentPhotos(int pageSize, int page, string userId);
    }
}
