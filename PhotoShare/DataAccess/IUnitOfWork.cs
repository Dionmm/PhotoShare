using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> Save();
    }
}
