using System;
using System.Collections.Generic;

namespace PRN222.CourseManagement.Repository.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int DepartmentId { get; set; }

    public DateTime DateOfBirth { get; set; } // Dùng cho BR26
    public bool IsActive { get; set; } = true; // Dùng cho BR29

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
