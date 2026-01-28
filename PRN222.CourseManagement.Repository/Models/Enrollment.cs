using System;
using System.Collections.Generic;

namespace PRN222.CourseManagement.Repository.Models;

public partial class Enrollment
{
    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateTime EnrollDate { get; set; }

    public decimal? Grade { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
