using Microsoft.EntityFrameworkCore;
using PRN222.CourseManagement.Repository.Models;
using System.Linq.Expressions;

namespace PRN222.CourseManagement.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal CourseManagementContext context;
        internal DbSet<T> dbSet;

        public GenericRepository(CourseManagementContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public T? GetById(object id)
        {
            return dbSet.Find(id);
        }

        public void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(object id)
        {
            T? entityToDelete = dbSet.Find(id);
            if (entityToDelete != null) Delete(entityToDelete);
        }

        public void Delete(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }
    }
}