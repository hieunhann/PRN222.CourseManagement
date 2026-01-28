using System;
using System.Collections.Generic;

namespace PRN222.CourseManagement.Repository.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseCode { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Credits { get; set; }

    public int DepartmentId { get; set; }

    public bool IsActive { get; set; } = true; // Dùng cho BR28

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
