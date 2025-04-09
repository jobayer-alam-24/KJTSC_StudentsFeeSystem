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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FathersName,Roll,Class,Date,Department")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

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
        public ActionResult MakePayment()
        {
            var model = new ItemListViewModel
            {
                Items = new List<Item>
                {
                new Item { Id = 1, Name = "Admission Fee", Value = 100 },
                 new Item { Id = 1, Name = "Tution Fee", Value = 100 },
                  new Item { Id = 1, Name = "Health Fee", Value = 200 }
                }
            };
            return View(model);
        }
        public IActionResult Print(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var pdfBytes = PdfService.GenerateStudentReceipt(student);

            Response.Headers.Add("Content-Disposition", $"inline; filename=Receipt_{student.Name}.pdf");

            return File(pdfBytes, "application/pdf");
        }

        [HttpPost]
        public ActionResult MakePayment(int id, ItemListViewModel model)
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
            var student = _context.Students.Find(id);
            decimal totalAmount = selectedFees.Sum(x => x.Value);
            if (student != null)
            {
                student.HasPaid = true;
                student.Fee = totalAmount;
                _context.Entry(student).State = EntityState.Modified;
                _context.SaveChanges(); 
            }

            return RedirectToAction(nameof(Index));
        }
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

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public JsonResult CheckRoll(bool isEdit, int roll)
        {
            if (isEdit)
            { 
                return Json(true);
            }
            bool isRollTaken = _context.Students.Any(s => s.Roll == roll);

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
