using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.DataAccess.Repositories.Interfaces
{
    public interface IMessageRepository: IRepository<Message>
    {
        IEnumerable<Message> GetMostRecentMessages(int pageSize, int page = 0);
    }
}
