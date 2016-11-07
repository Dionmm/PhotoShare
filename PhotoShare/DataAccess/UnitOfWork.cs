using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Repositories;
using PhotoShare.DataAccess.Repositories.Interfaces;

namespace PhotoShare.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PhotoShareDbContext _context;

        public UnitOfWork(PhotoShareDbContext context)
        {
            _context = context;
            //Add Repositories in here
            Photos = new PhotoRepository(_context);
        }

        public IPhotoRepository Photos { get; private set; }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}