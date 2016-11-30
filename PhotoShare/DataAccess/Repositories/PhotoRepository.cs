using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.DataAccess.Repositories.Interfaces;

namespace PhotoShare.DataAccess.Repositories
{
    public class PhotoRepository : Repository<Photo>, IPhotoRepository
    {
        public PhotoRepository(PhotoShareDbContext context) : base(context)
        {
        }

        public IEnumerable<Photo> GetMostRecentPhotos(int pageSize, int page)
        {
            var skipRows = page*pageSize;
            return Context.Photos.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
        }
    }
}