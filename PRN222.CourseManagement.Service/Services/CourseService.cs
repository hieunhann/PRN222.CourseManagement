using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repositories;
using PRN222.CourseManagement.Service.Common;
using PRN222.CourseManagement.Service.Interfaces;
using System;
using System.Linq;

namespace PRN222.CourseManagement.Service.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResult Create(Course course)
        {
            try
            {
                // BR13: Credits 1-6
                if (course.Credits < 1 || course.Credits > 6)
                    return ServiceResult.Fail("Course credits must be between 1 and 6.");

                // BR11: Mã môn duy nhất
                var existingCourse = _unitOfWork.CourseRepository.Get()
                    .FirstOrDefault(c => c.CourseCode == course.CourseCode);

                if (existingCourse != null)
                    return ServiceResult.Fail("Course Code already exists.");

                // BR12: Có khoa
                if (course.DepartmentId <= 0)
                    return ServiceResult.Fail("Course must belong to a valid department.");

                _unitOfWork.CourseRepository.Insert(course);
                _unitOfWork.Save();

                return ServiceResult.Success("Course created successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        public ServiceResult Update(Course course)
        {
            try
            {
                var existingCourse = _unitOfWork.CourseRepository.GetById(course.CourseId);
                if (existingCourse == null) return ServiceResult.Fail("Course not found.");

                // BR15: Inactive (Credits = 0 là inactive)
                if (existingCourse.Credits == 0)
                    return ServiceResult.Fail("Cannot update an inactive course.");

                existingCourse.Title = course.Title;
                existingCourse.Credits = course.Credits;
                existingCourse.DepartmentId = course.DepartmentId;

                _unitOfWork.CourseRepository.Update(existingCourse);
                _unitOfWork.Save();
                return ServiceResult.Success("Course updated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        public ServiceResult Delete(int courseId)
        {
            try
            {
                var course = _unitOfWork.CourseRepository.GetById(courseId);
                if (course == null) return ServiceResult.Fail("Course not found.");

                // BR14: Has Enrollments
                var hasEnrollments = _unitOfWork.EnrollmentRepository.Get().Any(e => e.CourseId == courseId);
                if (hasEnrollments)
                    return ServiceResult.Fail("Cannot delete course that has active enrollments.");

                _unitOfWork.CourseRepository.Delete(courseId);
                _unitOfWork.Save();
                return ServiceResult.Success("Course deleted successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }
    }
}