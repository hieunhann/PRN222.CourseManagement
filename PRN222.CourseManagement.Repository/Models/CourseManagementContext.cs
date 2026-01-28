using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PRN222.CourseManagement.Repository.Models;

public partial class CourseManagementContext : DbContext
{
    public CourseManagementContext()
    {
    }

    public CourseManagementContext(DbContextOptions<CourseManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Enrollment> Enrollments { get; set; }
    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server(local);uid=sa;pwd=12345;database=CourseManagementDB;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Bảng COURSE
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Department).WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Course_Department");
        });

        // 2. Bảng DEPARTMENT
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        // 3. Bảng ENROLLMENT (SỬA LỖI TẠI ĐÂY)
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollment");

            // QUAN TRỌNG: Cấu hình khóa chính là Cặp (StudentId + CourseId)
            // Thay vì tìm EnrollmentId (không tồn tại), ta dùng dòng này:
            entity.HasKey(e => new { e.StudentId, e.CourseId });

            // Các cột khác
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.EnrollDate).HasColumnType("datetime");
            entity.Property(e => e.Grade).HasColumnType("decimal(4, 2)");

            // Khóa ngoại
            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Enrollment_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Enrollment_Student");
        });

        // 4. Bảng STUDENT
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Student");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.StudentCode).HasMaxLength(20);

            entity.HasOne(d => d.Department).WithMany(p => p.Students)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Student_Department");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}