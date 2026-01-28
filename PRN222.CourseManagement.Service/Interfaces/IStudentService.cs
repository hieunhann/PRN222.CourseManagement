using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.Common;
using System.Collections.Generic;

namespace PRN222.CourseManagement.Service.Interfaces
{
    public interface IStudentService
    {
        ServiceResult GetAll(); // <-- Sửa thành ServiceResult
        ServiceResult GetById(int id); // <-- Sửa thành ServiceResult
        ServiceResult Create(Student student);
        ServiceResult Update(Student student);
        ServiceResult Delete(int id);
    }
}