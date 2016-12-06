using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.DataAccess.Repositories.Interfaces;

namespace PhotoShare.DataAccess.Repositories
{
    public class MessageRepository: Repository<Message>, IMessageRepository
    {
        public MessageRepository(PhotoShareDbContext context) : base(context)
        {
        }

        public IEnumerable<Message> GetMostRecentMessages(int pageSize, int page = 0)
        {
            var skipRows = page * pageSize;
            return Context.Messages.OrderByDescending(x => x.CreatedDateTime).Where(h => h.Hidden == false).Skip(skipRows).Take(pageSize).ToList();
        }
    }
}