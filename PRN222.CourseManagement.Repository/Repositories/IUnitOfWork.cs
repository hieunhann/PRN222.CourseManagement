using PRN222.CourseManagement.Repository.Models;

namespace PRN222.CourseManagement.Repository.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Department> DepartmentRepository { get; }
        IGenericRepository<Student> StudentRepository { get; }
        IGenericRepository<Course> CourseRepository { get; }
        IGenericRepository<Enrollment> EnrollmentRepository { get; }

        void Save();
    }
}