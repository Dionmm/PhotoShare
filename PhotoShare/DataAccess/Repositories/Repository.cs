using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Repositories.Interfaces;

namespace PhotoShare.DataAccess.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly PhotoShareDbContext Context;
        private readonly DbSet<T> _entities;
        protected Repository(PhotoShareDbContext context)
        {
            Context = context;
            _entities = Context.Set<T>();
        }

        public T Get(int id)
        {
            return _entities.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.ToList();
        }

        public void Add(T entity)
        {
             _entities.Add(entity);
        }

        public void AddRange(IEnumerable<T> entity)
        {
            _entities.AddRange(entity);
        }

        public void Remove(T entity)
        {
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            _entities.RemoveRange(entity);
        }
    }
}