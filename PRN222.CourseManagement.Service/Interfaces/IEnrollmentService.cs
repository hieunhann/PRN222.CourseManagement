using PRN222.CourseManagement.Service.Common;
using System;

namespace PRN222.CourseManagement.Service.Interfaces
{
    public interface IEnrollmentService
    {
        ServiceResult Enroll(int studentId, int courseId, DateTime? enrollDate = null);

      
        ServiceResult UpdateGrade(int studentId, int courseId, decimal grade);
    }
}