using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repositories;
using System;
using System.Linq;

namespace PRN222.CourseManagement.Repository.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        private CourseManagementContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<CourseManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new CourseManagementContext(options);
        }

        [Test]
        public void GetAll_ShouldReturnData_WhenDataExists()
        {
            using var context = CreateInMemoryContext();

            // Tạo Department trước vì Student cần DepartmentId
            var department = new Department { Name = "SE", Description = "Software Engineering" };
            context.Departments.Add(department);
            context.SaveChanges();

            context.Students.Add(new Student { StudentCode = "SE1", FullName = "A", Email = "a@fpt.edu.vn", DepartmentId = department.DepartmentId });
            context.Students.Add(new Student { StudentCode = "SE2", FullName = "B", Email = "b@fpt.edu.vn", DepartmentId = department.DepartmentId });
            context.SaveChanges();

            using var uow = new UnitOfWork(context);
            var result = uow.StudentRepository.Get().ToList();

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void Add_ShouldIncreaseCount_WhenAddingStudent()
        {
            using var context = CreateInMemoryContext();

            // Tạo Department trước
            var department = new Department { Name = "SE", Description = "Software Engineering" };
            context.Departments.Add(department);
            context.SaveChanges();

            using var uow = new UnitOfWork(context);

            var newStudent = new Student { StudentCode = "SE99", FullName = "New", Email = "n@fpt.edu.vn", DepartmentId = department.DepartmentId };

            uow.StudentRepository.Insert(newStudent);
            uow.Save();

            Assert.That(context.Students.Count(), Is.EqualTo(1));
            Assert.That(context.Students.First().StudentCode, Is.EqualTo("SE99"));
        }

        [Test]
        public void GetById_ShouldReturnCorrectStudent()
        {
            using var context = CreateInMemoryContext();

            // Tạo Department trước
            var department = new Department { Name = "SE", Description = "Software Engineering" };
            context.Departments.Add(department);
            context.SaveChanges();

            var student = new Student { StudentCode = "SE01", FullName = "Target", Email = "t@fpt.edu.vn", DepartmentId = department.DepartmentId };
            context.Students.Add(student);
            context.SaveChanges();

            using var uow = new UnitOfWork(context);
            var result = uow.StudentRepository.GetById(student.StudentId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FullName, Is.EqualTo("Target"));
        }

        [Test]
        public void Delete_ShouldRemoveStudent()
        {
            using var context = CreateInMemoryContext();

            // Tạo Department trước
            var department = new Department { Name = "SE", Description = "Software Engineering" };
            context.Departments.Add(department);
            context.SaveChanges();

            var student = new Student { StudentCode = "DEL", FullName = "Delete Me", Email = "d@fpt.edu.vn", DepartmentId = department.DepartmentId };
            context.Students.Add(student);
            context.SaveChanges();

            using var uow = new UnitOfWork(context);
            uow.StudentRepository.Delete(student.StudentId);
            uow.Save();

            Assert.That(context.Students.Count(), Is.EqualTo(0));
        }

        [Test]
        public void UnitOfWork_Save_ShouldPersistMultipleEntities()
        {
            using var context = CreateInMemoryContext();
            using var uow = new UnitOfWork(context);

            // Tạo Department trước khi tạo Student
            var department = new Department { Name = "AI" };
            uow.DepartmentRepository.Insert(department);
            uow.Save(); // Lưu Department trước để có DepartmentId

            var student = new Student { StudentCode = "AI1", FullName = "AI Student", Email = "ai@fpt.edu.vn", DepartmentId = department.DepartmentId };
            uow.StudentRepository.Insert(student);
            uow.Save();

            Assert.That(context.Departments.Count(), Is.EqualTo(1));
            Assert.That(context.Students.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateEnrollment_ShouldLinkStudentAndCourse()
        {
            using var context = CreateInMemoryContext();

            // Tạo Department trước
            var department = new Department { Name = "SE", Description = "Software Engineering" };
            context.Departments.Add(department);
            context.SaveChanges();

            var s = new Student { StudentCode = "S1", FullName = "S", Email = "e@fpt.edu.vn", DepartmentId = department.DepartmentId };
            var c = new Course { CourseCode = "C1", Title = "Java", Credits = 3, DepartmentId = department.DepartmentId };
            context.Students.Add(s);
            context.Courses.Add(c);
            context.SaveChanges();

            using var uow = new UnitOfWork(context);
            var enroll = new Enrollment { StudentId = s.StudentId, CourseId = c.CourseId, EnrollDate = DateTime.Now, Grade = 9.0m };

            uow.EnrollmentRepository.Insert(enroll);
            uow.Save();

            var result = context.Enrollments.First();
            Assert.That(result.StudentId, Is.EqualTo(s.StudentId));
            Assert.That(result.Grade, Is.EqualTo(9.0m));
        }
    }
}
