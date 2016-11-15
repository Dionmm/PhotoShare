using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public IEnumerable<Photo> GetMostRecentPhotos(int count)
        {
            return Context.Photos.OrderByDescending(x => x.CreatedDateTime).Take(count).ToList();
        }

        public Photo GetPhotoByName(string name)
        {
            return Context.Photos.FirstOrDefault(p => p.Name == name);
        }
    }
}