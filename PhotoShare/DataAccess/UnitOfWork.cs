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
            ExifData = new ExifDataRepository(_context);
            Purchases = new PurchaseRepository(_context);
        }

        public IPhotoRepository Photos { get; }
        public IExifDataRepository ExifData { get; }
        public IPurchaseRepository Purchases { get; }


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