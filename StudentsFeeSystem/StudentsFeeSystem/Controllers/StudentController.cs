using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentsFeeSystem.Data;
using StudentsFeeSystem.Helpers;
using StudentsFeeSystem.Models;
using StudentsFeeSystem.Services;
using StudentsFeeSystem.ViewModel;

namespace StudentsFeeSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            BindSelectList();
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,FathersName,Roll,Class,Date,Department")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            BindSelectList();
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            BindSelectList();
            ViewBag.Fees = student.Fee;
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FathersName,Roll,Class,Date,Department,HasPaid")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            var existingStudent = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            student.HasPaid = existingStudent.HasPaid;
            student.Fee = existingStudent.Fee;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            BindSelectList();
            return View(student);
        }


        // GET: MakePayment
        public ActionResult MakePayment()
        {
            var model = new ItemListViewModel
            {
                Items = new List<Item>
            {
                new Item { Id = 1, Name = "Admission Fee", Value =  100 },
                new Item { Id = 2, Name = "Yearly Admission Fee",   Value = 200 },
                new Item { Id = 3, Name = "Collateral Fee", Value =     50 },
                new Item { Id = 4, Name = "Internal Exam Fee", Value =  30 },
                new Item { Id = 5, Name = "Magazine Fee (Yearly)",  Value = 25 },
                new Item { Id = 6, Name = "Religious Fund", Value = 10 },
                new Item { Id = 7, Name = "Rover Scout / Girl Guide     Fees", Value = 15 },
                new Item { Id = 8, Name = "Registration Fees", Value =  20 },
                new Item { Id = 9, Name = "Students Medical Test    Fees", Value = 40 },
                new Item { Id = 10, Name = "Identity Fees", Value =     5 },
                new Item { Id = 11, Name = "Red Crescent Fees",     Value = 8 },
                new Item { Id = 12, Name = "Mosque Development Fees",   Value = 12 },
                new Item { Id = 13, Name = "Games and Cultural Fees",   Value = 18 },
                new Item { Id = 14, Name = "Credential Fees", Value =   10 },
                new Item { Id = 15, Name = "Enrolled Student    Certificate Fees", Value = 6 },
                new Item { Id = 16, Name = "Poor Fund Fees", Value = 5 },
                new Item { Id = 17, Name = "Night Guard Fees", Value =  3 },
                new Item { Id = 18, Name = "ICT Fees", Value = 20 },
                new Item { Id = 19, Name = "Cycle Garage Fees",     Value = 7 },
                new Item { Id = 20, Name = "Parents Day Fees", Value =  15 },
                new Item { Id = 21, Name = "Science & Technology    Fees", Value = 10 },
                new Item { Id = 22, Name = "Education Week Fees",   Value = 8 },
                new Item { Id = 23, Name = "Literature & Culture    Fees", Value = 12 },
                new Item { Id = 24, Name = "Others", Value = 5 }
                }
            };
            return View(model);
        }

        // POST: MakePayment
        [HttpPost]
        public async Task<IActionResult> MakePayment(int id, ItemListViewModel model)
        {
            if (model.Items == null || !model.Items.Any(x => x.IsSelected))
            {
                ModelState.AddModelError("", "Please select at least one fee item.");
                ViewBag.StudentId = id;
                return View(model);
            }

            var selectedFees = model.Items
                .Where(x => x.IsSelected)
                .Select(x => new { x.Name, x.Value })
                .ToList();

            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                decimal totalAmount = selectedFees.Sum(x => x.Value);
                student.HasPaid = true;
                student.Fee = totalAmount;
                _context.Entry(student).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Print Receipt (Asynchronous)
        public async Task<IActionResult> Print(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var pdfBytes = PdfService.GenerateStudentReceipt(student);

            Response.Headers.Add("Content-Disposition", $"inline; filename=Receipt_{student.Name}.pdf");

            return File(pdfBytes, "application/pdf");
        }

        // Reset all fees to zero asynchronously
        public async Task<IActionResult> ResetFees()
        {
            var students = await _context.Students.ToListAsync();

            foreach (var student in students)
            {
                student.Fee = 0;
                student.HasPaid = false;
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "All fees have been reset to 0 and payment status is set to false.";
            return RedirectToAction("Index");
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();  

            return RedirectToAction(nameof(Index));  
        }



        [HttpGet]
        public async Task<IActionResult> ResetFee(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            student.HasPaid = false;
            student.Fee = 0;

            _context.Update(student);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Student fee reset successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<JsonResult> CheckRoll(bool isEdit, int roll)
        {
            if (isEdit)
            {
                return Json(true);
            }

            bool isRollTaken = await _context.Students.AnyAsync(s => s.Roll == roll);
            if (isRollTaken)
            {
                return Json($"Roll number is already taken!");
            }

            return Json(true);
        }

        private void BindSelectList()
        {
            var departmentList = Enum.GetValues(typeof(Department))
                         .Cast<Department>()
                         .Select(d => new
                         {
                             Value = d.ToString(),
                             Text = EnumHelper.GetDescription(d)
                         });
            ViewBag.Departments = new SelectList(departmentList, "Value", "Text");
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
