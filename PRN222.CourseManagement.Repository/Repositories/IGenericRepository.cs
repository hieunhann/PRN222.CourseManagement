using System.Linq.Expressions;

namespace PRN222.CourseManagement.Repository.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Hàm lấy dữ liệu có hỗ trợ lọc (filter) và kèm bảng phụ (includeProperties)
        IEnumerable<T> Get(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "");

        T? GetById(object id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(object id);
        void Delete(T entity);
    }
}