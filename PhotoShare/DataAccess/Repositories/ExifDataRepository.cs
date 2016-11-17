using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.DataAccess.Repositories.Interfaces;

namespace PhotoShare.DataAccess.Repositories
{
    public class ExifDataRepository: Repository<ExifData>, IExifDataRepository
    {
        public ExifDataRepository(PhotoShareDbContext context) : base(context)
        {
        }
        
    }
}