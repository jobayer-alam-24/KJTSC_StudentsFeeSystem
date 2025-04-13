using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsFeeSystem.Data;
using StudentsFeeSystem.Models;
using StudentsFeeSystem.ViewModel;

namespace StudentsFeeSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Reports()
        {
            var students = await _context.Students.ToListAsync();

            var report = new ReportViewModel
            {
                Overall = BuildReport(students),

                ClassReports = Enumerable.Range(6, 7)
                    .ToDictionary(
                        classNumber => classNumber,
                        classNumber => new ClassReport
                        {
                            Report = BuildReport(students.Where(s => s.Class == classNumber)),
                            DepartmentReports = Enum.GetValues(typeof(Department))
                                .Cast<Department>()
                                .ToDictionary(
                                    dept => dept,
                                    dept => BuildReport(students.Where(s => s.Class == classNumber && s.Department == dept))
                                )
                        }
                    )
            };

            return View(report);
        }

        private ReportSection BuildReport(IEnumerable<Student> students)
        {
            return new ReportSection
            {
                StudentCount = students.Count(),
                PaidCount = students.Count(s => s.HasPaid),
                NotPaidCount = students.Count(s => !s.HasPaid),
                TotalCollected = students
                    .Where(s => s.HasPaid && s.Fee.HasValue)
                    .Sum(s => s.Fee.Value)
            };
        }
    }
}
