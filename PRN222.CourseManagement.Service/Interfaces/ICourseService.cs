using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Service.Common;

namespace PRN222.CourseManagement.Service.Interfaces
{
    public interface ICourseService
    {
        ServiceResult Create(Course course);
        ServiceResult Update(Course course);
        ServiceResult Delete(int courseId);
    }
}