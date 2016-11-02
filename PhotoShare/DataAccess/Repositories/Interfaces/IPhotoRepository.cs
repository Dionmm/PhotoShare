using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.DataAccess.Repositories.Interfaces
{
    public interface IPhotoRepository : IRepository<Photo>
    {
        IEnumerable<Photo> GetMostRecentPhotos(int count);

    }
}
