using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.Common;

namespace PRN222.CourseManagement.Service.Interfaces
{
    public interface IDepartmentService
    {
        ServiceResult Create(Department department);
        ServiceResult Delete(int departmentId);
    }
}