using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repositories;
using PRN222.CourseManagement.Service.Interfaces;
using PRN222.CourseManagement.Service.Services;

static class Program
{
    static void Main(string[] args)
    {
        // ==========================================================
        // 1. CẤU HÌNH HỆ THỐNG (DI CONTAINER)
        // ==========================================================
        var serviceProvider = new ServiceCollection()
            // Kết nối SQL Server
            .AddDbContext<CourseManagementContext>(options =>
                options.UseSqlServer("Server=(local);Database=CourseManagementDB;Trusted_Connection=True;TrustServerCertificate=True"))

            // Đăng ký UnitOfWork & Repository
            .AddScoped<IUnitOfWork, UnitOfWork>()

            // Đăng ký các Service
            .AddScoped<IStudentService, StudentService>()
            .AddScoped<IEnrollmentService, EnrollmentService>()
            .AddScoped<ICourseService, CourseService>()
            .AddScoped<IDepartmentService, DepartmentService>()

            .BuildServiceProvider();

        // Lấy Service ra để dùng
        var studentService = serviceProvider.GetService<IStudentService>();
        // var enrollmentService = serviceProvider.GetService<IEnrollmentService>();

        // Hỗ trợ hiển thị tiếng Việt
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        // ==========================================================
        // 2. MENU GIAO DIỆN (LOOP)
        // ==========================================================
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=========================================");
            Console.WriteLine("    COURSE MANAGEMENT SYSTEM (DEMO)      ");
            Console.WriteLine("=========================================");
            Console.ResetColor();
            Console.WriteLine("1. Tạo sinh viên mới (Nhập tay)");
            Console.WriteLine("2. Xem danh sách sinh viên");
            Console.WriteLine("0. Thoát chương trình");
            Console.WriteLine("=========================================");
            Console.Write("👉 Chọn chức năng: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateStudentManual(studentService);
                    break;
                case "2":
                    ShowListStudents(studentService);
                    break;
                case "0":
                    Console.WriteLine("Tạm biệt!");
                    return; // Thoát
                default:
                    Console.WriteLine("Chọn sai! Nhấn Enter để chọn lại.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    // -----------------------------------------------------------
    // CHỨC NĂNG 1: TẠO SINH VIÊN (NHẬP TAY)
    // -----------------------------------------------------------
    static void CreateStudentManual(IStudentService service)
    {
        Console.WriteLine("\n--- [1] NHẬP THÔNG TIN SINH VIÊN ---");

        Console.Write("Nhập MSSV (ví dụ SE123): ");
        string code = Console.ReadLine();

        Console.Write("Nhập Họ Tên: ");
        string name = Console.ReadLine();

        Console.Write("Nhập Email: ");
        string email = Console.ReadLine();

        // Department ID thường là số, ở đây mình để mặc định là 1 để demo cho nhanh
        // Bạn có thể cho nhập nếu muốn
        int deptId = 1;
        Console.Write($"Nhập Department ID (Mặc định {deptId}): ");
        string deptInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(deptInput))
        {
            int.TryParse(deptInput, out deptId);
        }

        var newStudent = new Student
        {
            StudentCode = code,
            FullName = name,
            Email = email,
            DepartmentId = deptId
        };

        Console.WriteLine("Đang xử lý...");
        var result = service.Create(newStudent);

        if (result.IsSuccess)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✔ THÀNH CÔNG: " + result.Message);
            Console.ResetColor();
            Console.WriteLine($"   + MSSV : {newStudent.StudentCode}");
            Console.WriteLine($"   + Tên  : {newStudent.FullName}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n✘ THẤT BẠI: " + result.Message);
            Console.ResetColor();
            Console.WriteLine("(Gợi ý: Kiểm tra trùng mã, tên rỗng, hoặc Department ID chưa có trong DB)");
        }

        Console.WriteLine("\nNhấn Enter để quay lại Menu...");
        Console.ReadLine();
    }

    // -----------------------------------------------------------
    // CHỨC NĂNG 2: XEM DANH SÁCH
    // -----------------------------------------------------------
    static void ShowListStudents(IStudentService service)
    {
        Console.WriteLine("\n--- [2] DANH SÁCH SINH VIÊN ---");

        var result = service.GetAll();

        if (result.IsSuccess && result.Data is IEnumerable<Student> list)
        {
            // In tiêu đề bảng
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{"MSSV",-10} | {"Họ và Tên",-25} | {"Email"}");
            Console.WriteLine(new string('-', 60));
            Console.ResetColor();

            // In dữ liệu
            foreach (var s in list)
            {
                Console.WriteLine($"{s.StudentCode,-10} | {s.FullName,-25} | {s.Email}");
            }
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"Tổng số: {System.Linq.Enumerable.Count(list)} sinh viên");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Lỗi lấy danh sách: {result.Message}");
            Console.ResetColor();
        }

        Console.WriteLine("\nNhấn Enter để quay lại Menu...");
        Console.ReadLine();
    }
}