using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repositories;
using PRN222.CourseManagement.Service.Common;
using PRN222.CourseManagement.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PRN222.CourseManagement.Service.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResult GetAll()
        {
            try
            {
                var list = _unitOfWork.StudentRepository.Get().ToList();
                return ServiceResult.Success("Success", list);
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        public ServiceResult GetById(int id)
        {
            var student = _unitOfWork.StudentRepository.GetById(id);
            if (student == null) return ServiceResult.Fail("Student not found");
          
            return ServiceResult.Success(string.Empty, student);
        }
        public ServiceResult Create(Student student)
        {
            try
            {
                // BR07: Không được null
                if (string.IsNullOrWhiteSpace(student.FullName))
                    return ServiceResult.Fail("Name cannot be empty");

                // BR08: Tên phải >= 3 ký tự
                if (student.FullName.Trim().Length < 3)
                    return ServiceResult.Fail("Student name must be at least 3 characters.");

                // BR05: Check trùng mã số sinh viên
                var existsCode = _unitOfWork.StudentRepository.Get()
                    .Any(s => s.StudentCode == student.StudentCode);
                if (existsCode) return ServiceResult.Fail("Student Code already exists.");

                // BR09: Check trùng Email
                if (!string.IsNullOrEmpty(student.Email))
                {
                    var existsEmail = _unitOfWork.StudentRepository.Get()
                        .Any(s => s.Email == student.Email);
                    if (existsEmail) return ServiceResult.Fail("Email already exists.");
                }

                _unitOfWork.StudentRepository.Insert(student);
                _unitOfWork.Save();
                return ServiceResult.Success("Student created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        public ServiceResult Update(Student student)
        {
            try
            {
                _unitOfWork.StudentRepository.Update(student);
                _unitOfWork.Save();
                return ServiceResult.Success("Student updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        public ServiceResult Delete(int id)
        {
            try
            {
                var student = _unitOfWork.StudentRepository.GetById(id);
                if (student == null) return ServiceResult.Fail("Student not found");

                // BR10: Check Enrollment trước khi xóa
                var hasEnrollments = _unitOfWork.EnrollmentRepository.Get()
                    .Any(e => e.StudentId == id);

                if (hasEnrollments)
                {
                    return ServiceResult.Fail("Cannot delete student who has enrollments.");
                }

                _unitOfWork.StudentRepository.Delete(id);
                _unitOfWork.Save();
                return ServiceResult.Success("Student deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }
    }
}