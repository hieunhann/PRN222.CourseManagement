using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repositories;
using PRN222.CourseManagement.Service.Common;
using PRN222.CourseManagement.Service.Interfaces;
using System;
using System.Linq;

namespace PRN222.CourseManagement.Service.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Constants for business rules
        private const int MIN_STUDENT_AGE = 18;
        private const int MIN_CREDITS = 1;
        private const int GRADING_PERIOD_DAYS = 30;
        private const int MAX_ENROLLMENT_COUNT = 5;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResult Enroll(int studentId, int courseId, DateTime? enrollDate = null)
        {
            try
            {
                DateTime dateToEnroll = enrollDate ?? DateTime.Now;

                // BR18: Check ngày quá khứ
                if (enrollDate.HasValue && dateToEnroll < DateTime.Now.AddMinutes(-1))
                {
                    return ServiceResult.Fail("Enrollment date cannot be in the past.");
                }

                var student = _unitOfWork.StudentRepository.GetById(studentId);
                var course = _unitOfWork.CourseRepository.GetById(courseId);

                // BR20: Tồn tại
                if (student == null || course == null)
                    return ServiceResult.Fail("Student or Course does not exist.");

                // --- START NEW BUSINESS RULES (Step 2: Green) ---

                // BR29: Student Active
                if (!student.IsActive)
                    return ServiceResult.Fail("Student is not active.");

                // BR28: Course Active
                if (!course.IsActive)
                    return ServiceResult.Fail("Course is not active.");

                // BR27: Course Credits
                if (course.Credits < MIN_CREDITS)
                    return ServiceResult.Fail($"Course must have at least {MIN_CREDITS} credit.");

                // BR26: Age >= 18
                if (!ValidateStudentAge(student))
                    return ServiceResult.Fail($"Student must be at least {MIN_STUDENT_AGE} years old.");

                // --- END NEW BUSINESS RULES ---

                // BR19: Cùng khoa
                if (student.DepartmentId != course.DepartmentId)
                    return ServiceResult.Fail("Student can only enroll in courses of their department.");

                // BR16: Không trùng
                var existingEnrollment = _unitOfWork.EnrollmentRepository.Get()
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

                if (existingEnrollment != null)
                    return ServiceResult.Fail("Student is already enrolled in this course.");

                // BR17: Max 5 môn
                var currentCount = _unitOfWork.EnrollmentRepository.Get().Count(e => e.StudentId == studentId);
                if (currentCount >= MAX_ENROLLMENT_COUNT)
                    return ServiceResult.Fail($"Student cannot enroll in more than {MAX_ENROLLMENT_COUNT} courses.");

                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollDate = dateToEnroll,
                    Grade = null
                };

                _unitOfWork.EnrollmentRepository.Insert(enrollment);
                _unitOfWork.Save();

                return ServiceResult.Success("Enrolled successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Enrollment Failed: {ex.Message}");
            }
        }

        public ServiceResult UpdateGrade(int studentId, int courseId, decimal grade)
        {
            try
            {
                // BR22: Valid Range
                if (grade < 0 || grade > 10)
                    return ServiceResult.Fail("Grade must be between 0 and 10.");

                // BR21: Enrollment exist
                var enrollment = _unitOfWork.EnrollmentRepository.Get()
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment == null)
                    return ServiceResult.Fail("Enrollment does not exist. Cannot assign grade.");

                // BR30: Grading Period (30 days)
                if (!ValidateGradingPeriod(enrollment.EnrollDate))
                    return ServiceResult.Fail($"Grading period has expired ({GRADING_PERIOD_DAYS} days limit).");

                // BR23: Finalized
                if (enrollment.Grade.HasValue)
                    return ServiceResult.Fail("Grade is already finalized and cannot be updated.");

                enrollment.Grade = grade;
                _unitOfWork.EnrollmentRepository.Update(enrollment);
                _unitOfWork.Save();

                return ServiceResult.Success("Grade assigned successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        // Helper method: Validate student age with precise calculation
        private bool ValidateStudentAge(Student student)
        {
            var now = DateTime.Now;
            int age = now.Year - student.DateOfBirth.Year;

            // Fix: If birthday hasn't occurred this year yet, subtract 1
            if (student.DateOfBirth.AddYears(age) > now)
                age--;

            return age >= MIN_STUDENT_AGE;
        }

        // Helper method: Validate grading period - thêm static
        private static bool ValidateGradingPeriod(DateTime enrollDate)
        {
            //                       DateTime.Now
            var daysSinceEnrollment = (DateTime.Now - enrollDate).TotalDays;
            return daysSinceEnrollment <= GRADING_PERIOD_DAYS;
        }
    }
}