using System.Collections.Generic;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.DataAccess.Repositories.Interfaces
{
    public interface IPhotoRepository : IRepository<Photo>
    {
        IEnumerable<Photo> GetMostRecentPhotos(int count);
    }
}
