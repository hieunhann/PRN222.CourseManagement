using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repositories;
using PRN222.CourseManagement.Service.Services;
using PRN222.CourseManagement.Service.Interfaces;
using System;

namespace PRN222.CourseManagement.Repository.Tests
{
    [TestFixture]
    public class TddBusinessRuleTests
    {
        private CourseManagementContext _context;
        private IUnitOfWork _unitOfWork;
        private IEnrollmentService _enrollmentService;

        [SetUp]
        public void Setup()
        {
            // Tạo database mới cho mỗi lần test
            var options = new DbContextOptionsBuilder<CourseManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new CourseManagementContext(options);
            _unitOfWork = new UnitOfWork(_context);
            _enrollmentService = new EnrollmentService(_unitOfWork);
        }

        [TearDown]
        public void Cleanup() { _unitOfWork?.Dispose(); _context?.Dispose(); }

        // --- BR26: Student Age must be >= 18 ---
        [Test]
        public void TC26_Enroll_StudentUnder18_ShouldFail()
        {
            // Given: Setup Department, Course
            var dept = new Department { Name = "SE" };
            _context.Departments.Add(dept);
            _context.SaveChanges();

            var course = new Course { CourseCode = "C1", Title = "Test Course", Credits = 3, DepartmentId = dept.DepartmentId, IsActive = true };
            _context.Courses.Add(course);

            // Given: Student sinh năm 2015 (Hiện tại chưa đủ 18 tuổi)
            var student = new Student
            {
                StudentCode = "S1",
                FullName = "Kid",
                Email = "kid@school.com",
                DepartmentId = dept.DepartmentId,
                DateOfBirth = DateTime.Now.AddYears(-15), // 15 tuổi
                IsActive = true
            };
            _context.Students.Add(student);
            _context.SaveChanges();

            // When: Cố gắng enroll
            var result = _enrollmentService.Enroll(student.StudentId, course.CourseId);

            // Then: Phải thất bại
            Assert.That(result.IsSuccess, Is.False, "BR26: Should not enroll student under 18");
        }

        // --- BR27: Course must have at least 1 credit ---
        [Test]
        public void TC27_Enroll_CourseZeroCredits_ShouldFail()
        {
            // Given: Course có 0 tín chỉ
            var dept = new Department { Name = "SE" };
            _context.Departments.Add(dept);
            _context.SaveChanges();

            var course = new Course { CourseCode = "C0", Title = "Zero Credit Course", Credits = 0, DepartmentId = dept.DepartmentId, IsActive = true };
            _context.Courses.Add(course);

            var student = new Student { StudentCode = "S1", FullName = "Student A", Email = "a@test.com", DepartmentId = dept.DepartmentId, DateOfBirth = DateTime.Now.AddYears(-20), IsActive = true };
            _context.Students.Add(student);
            _context.SaveChanges();

            // When: Enroll
            var result = _enrollmentService.Enroll(student.StudentId, course.CourseId);

            // Then: Fail
            Assert.That(result.IsSuccess, Is.False, "BR27: Should not enroll in course with 0 credits");
        }

        // --- BR28: Student cannot enroll in inactive courses ---
        [Test]
        public void TC28_Enroll_InactiveCourse_ShouldFail()
        {
            // Given: Course Inactive (IsActive = false)
            var dept = new Department { Name = "SE" };
            _context.Departments.Add(dept);
            _context.SaveChanges();

            var course = new Course { CourseCode = "C_Inactive", Title = "Old Course", Credits = 3, DepartmentId = dept.DepartmentId, IsActive = false };
            _context.Courses.Add(course);

            var student = new Student { StudentCode = "S1", FullName = "Student A", Email = "a@test.com", DepartmentId = dept.DepartmentId, DateOfBirth = DateTime.Now.AddYears(-20), IsActive = true };
            _context.Students.Add(student);
            _context.SaveChanges();

            // When: Enroll
            var result = _enrollmentService.Enroll(student.StudentId, course.CourseId);

            // Then: Fail
            Assert.That(result.IsSuccess, Is.False, "BR28: Should not enroll in inactive course");
        }

        // --- BR29: Inactive student cannot be enrolled ---
        [Test]
        public void TC29_Enroll_InactiveStudent_ShouldFail()
        {
            // Given: Student Inactive (IsActive = false)
            var dept = new Department { Name = "SE" };
            _context.Departments.Add(dept);
            _context.SaveChanges();

            var course = new Course { CourseCode = "C1", Title = "Course 1", Credits = 3, DepartmentId = dept.DepartmentId, IsActive = true };
            _context.Courses.Add(course);

            var student = new Student { StudentCode = "S_Inactive", FullName = "Inactive Student", Email = "inactive@test.com", DepartmentId = dept.DepartmentId, DateOfBirth = DateTime.Now.AddYears(-20), IsActive = false };
            _context.Students.Add(student);
            _context.SaveChanges();

            // When: Enroll
            var result = _enrollmentService.Enroll(student.StudentId, course.CourseId);

            // Then: Fail
            Assert.That(result.IsSuccess, Is.False, "BR29: Should not enroll inactive student");
        }

        // --- BR30: Grade assignment allowed only within grading period (e.g., 30 days) ---
        [Test]
        public void TC30_Grade_OutsideGradingPeriod_ShouldFail()
        {
            // Given: Enrollment đã diễn ra quá lâu (ví dụ 40 ngày trước)
            var dept = new Department { Name = "SE" };
            _context.Departments.Add(dept);
            _context.SaveChanges();

            var course = new Course { CourseCode = "C1", Title = "Course 1", Credits = 3, DepartmentId = dept.DepartmentId, IsActive = true };
            _context.Courses.Add(course);

            var student = new Student { StudentCode = "S1", FullName = "Student A", Email = "a@test.com", DepartmentId = dept.DepartmentId, DateOfBirth = DateTime.Now.AddYears(-20), IsActive = true };
            _context.Students.Add(student);
            _context.SaveChanges();

            // Enroll từ 40 ngày trước
            var pastDate = DateTime.Now.AddDays(-40);
            var enrollment = new Enrollment { StudentId = student.StudentId, CourseId = course.CourseId, EnrollDate = pastDate, Grade = null };
            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            // When: Cố gắng nhập điểm
            var result = _enrollmentService.UpdateGrade(student.StudentId, course.CourseId, 8.0m);

            // Then: Fail (Vì quá hạn chấm điểm)
            Assert.That(result.IsSuccess, Is.False, "BR30: Should not assign grade after grading period (30 days)");
        }
    }
}