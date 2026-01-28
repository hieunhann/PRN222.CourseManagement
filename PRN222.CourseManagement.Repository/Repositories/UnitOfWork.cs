using PRN222.CourseManagement.Repository.Models;
using System;

namespace PRN222.CourseManagement.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseManagementContext _context;

        // Khai báo biến backing field (có thể null ban đầu)
        private IGenericRepository<Student>? _studentRepository;
        private IGenericRepository<Course>? _courseRepository;
        private IGenericRepository<Department>? _departmentRepository;
        private IGenericRepository<Enrollment>? _enrollmentRepository;

        public UnitOfWork(CourseManagementContext context)
        {
            _context = context;
        }

        // Dùng Lazy Loading: Nếu null thì new, nếu có rồi thì dùng tiếp
        public IGenericRepository<Student> StudentRepository =>
            _studentRepository ??= new GenericRepository<Student>(_context);

        public IGenericRepository<Course> CourseRepository =>
            _courseRepository ??= new GenericRepository<Course>(_context);

        public IGenericRepository<Department> DepartmentRepository =>
            _departmentRepository ??= new GenericRepository<Department>(_context);

        public IGenericRepository<Enrollment> EnrollmentRepository =>
            _enrollmentRepository ??= new GenericRepository<Enrollment>(_context);

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}