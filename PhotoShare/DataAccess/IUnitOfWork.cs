using System;
using PhotoShare.DataAccess.Repositories.Interfaces;

namespace PhotoShare.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IPhotoRepository Photos { get; }
        IExifDataRepository ExifData { get; }
        IPurchaseRepository Purchases { get; }
        int Save();
    }
}
