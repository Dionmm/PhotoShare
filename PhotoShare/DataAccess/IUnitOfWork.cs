using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoShare.DataAccess.Repositories.Interfaces;

namespace PhotoShare.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IPhotoRepository Photos { get; }
        int Save();
    }
}
