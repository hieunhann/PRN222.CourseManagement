using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repositories;
using PRN222.CourseManagement.Service.Common;
using PRN222.CourseManagement.Service.Interfaces;
using System;
using System.Linq;

namespace PRN222.CourseManagement.Service.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResult Create(Department department)
        {
            try
            {
                // BR02: Tên không rỗng và dài hơn 3 ký tự
                if (string.IsNullOrWhiteSpace(department.Name) || department.Name.Length < 3)
                    return ServiceResult.Fail("Department name must be at least 3 characters.");

                // BR01: Tên khoa phải là duy nhất
                var existingDept = _unitOfWork.DepartmentRepository.Get()
                    .FirstOrDefault(d => d.Name.ToLower() == department.Name.ToLower());

                if (existingDept != null)
                    return ServiceResult.Fail("Department name already exists.");

                _unitOfWork.DepartmentRepository.Insert(department);
                _unitOfWork.Save();

                return ServiceResult.Success("Department created successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        public ServiceResult Delete(int departmentId)
        {
            try
            {
                var dept = _unitOfWork.DepartmentRepository.GetById(departmentId);
                if (dept == null) return ServiceResult.Fail("Department not found.");

                // BR03: Không xóa nếu đang có sinh viên
                var hasStudents = _unitOfWork.StudentRepository.Get().Any(s => s.DepartmentId == departmentId);
                if (hasStudents)
                    return ServiceResult.Fail("Cannot delete department that has students.");

                // BR04: Không xóa nếu đang có môn học
                var hasCourses = _unitOfWork.CourseRepository.Get().Any(c => c.DepartmentId == departmentId);
                if (hasCourses)
                    return ServiceResult.Fail("Cannot delete department that has courses.");

                _unitOfWork.DepartmentRepository.Delete(departmentId);
                _unitOfWork.Save();
                return ServiceResult.Success("Department deleted successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }
    }
}