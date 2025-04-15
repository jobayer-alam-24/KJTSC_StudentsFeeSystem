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
        [Route("Student/List")]
        [HttpGet]
        public async Task<IActionResult> List(string searchInput, int? classFilter, string paymentStatusFilter, string genderFilter)
        {
            var students = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(searchInput))
            {
                students = students.Where(s => s.Name.ToLower().Contains(searchInput.ToLower()) || s.Roll.ToString().Contains(searchInput.ToLower()));
            }

            if (classFilter.HasValue)
            {
                students = students.Where(s => s.Class == classFilter.Value);
            }

            if (!string.IsNullOrEmpty(paymentStatusFilter))
            {
                if (paymentStatusFilter == "Paid")
                {
                    students = students.Where(s => s.HasPaid);
                }
                else if (paymentStatusFilter == "NotPaid")
                {
                    students = students.Where(s => !s.HasPaid);
                }
            }
            if (!string.IsNullOrEmpty(genderFilter))
            {
                students = students.Where(s => s.Gender.ToString() == genderFilter);
            }
            await CountTotalFee();
            return View(await students.ToListAsync());
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

        public IActionResult Create()
        {
            BindSelectList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,FathersName,Roll,Class,Date,Department,Gender")] Student student)
        {
            if (ModelState.IsValid)
            {
                bool isInSameRoll = _context.Students
                .Any(s => s.Class == student.Class
                && s.Roll == student.Roll
                && s.Department == student.Department
                && s.Id != student.Id);
                if (isInSameRoll)
                {
                    TempData["RoleExistsError"] = "Roll already taken in this class & department.";
                    BindSelectList();
                    return View(student);
                }
                _context.Add(student);
                await _context.SaveChangesAsync();
                await CountTotalFee();
                return RedirectToAction(nameof(List));
            }
            BindSelectList();
            return View(student);
        }

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FathersName,Roll,Class,Date,Department,HasPaid,Gender")] Student student)
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
            bool isInSameRoll = _context.Students
                .Any(s => s.Class == student.Class
                       && s.Roll == student.Roll
                       && s.Department == student.Department
                       && s.Id != student.Id);

            if (isInSameRoll)
            {
                TempData["RoleExistsError"] = "Roll already taken in this class & department.";
                BindSelectList();
                return View(student);
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
                await CountTotalFee();
                return RedirectToAction(nameof(List));
            }

            BindSelectList();
            return View(student);
        }


        // GET: MakePayment
        [Route("Student/MakePayment/{id}")]
        [HttpGet]
        public async Task<IActionResult> MakePayment(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            List<FeeItem> feeItems = await _context.FeeItems.ToListAsync();

            if (student.Gender.ToString() == "Male")
            {
                feeItems = feeItems
                    .Where(f => f.AssignedToMale)
                    .Where(f => IsFeeItemAssignedToClass(f, student.Class))
                    .ToList();
            }
            else if (student.Gender.ToString() == "Female")
            {
                feeItems = feeItems
                    .Where(f => f.AssignedToFemale)
                    .Where(f => IsFeeItemAssignedToClass(f, student.Class))
                    .ToList();
            }
            else
            {
                feeItems = feeItems
                    .Where(f => IsFeeItemAssignedToClass(f, student.Class))
                    .ToList();
            }

            var viewModel = new ItemListViewModel
            {
                Id = student.Id,
                Items = feeItems.Select(f => new FeeItemViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value,
                    IsSelected = false
                }).ToList()
            };

            return View(viewModel);
        }
        private bool IsFeeItemAssignedToClass(FeeItem feeItem, int studentClass)
        {
            // Dynamically generate the property name based on the student's class
            var propertyName = $"AssignedToClass{studentClass}";

            // Get the property using reflection
            var property = feeItem.GetType().GetProperty(propertyName);

            if (property != null)
            {
                // Return the value of the property (true or false) for the given class
                return (bool)property.GetValue(feeItem);
            }

            // If the property does not exist, return false
            return false;
        }

        // POST: MakePayment
        [Route("Student/MakePayment/{id}")]
        [HttpPost]
        public async Task<IActionResult> MakePayment(ItemListViewModel model)
        {
            var student = await _context.Students.FindAsync(model.Id);
            if (student == null)
            {
                return NotFound();
            }

            decimal totalAmount = 0;
            var selectedItems = new List<FeeItem>();

            foreach (var item in model.Items)
            {
                if (item.IsSelected)
                {
                    var feeItem = await _context.FeeItems.FindAsync(item.Id);
                    if (feeItem != null)
                    {
                        selectedItems.Add(feeItem);
                        totalAmount += feeItem.Value;
                    }
                }
            }

            if (selectedItems.Count == 0)
            {
                ModelState.AddModelError("", "Please select at least one item.");
                ViewBag.StudentId = model.Id;
                return View(model);
            }

            student.HasPaid = true;
            student.Fee = totalAmount;
            student.Date = DateTime.UtcNow;

            _context.Update(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("List");
        }

        // Print Receipt (Asynchronous)

        public async Task<IActionResult> Print(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var pdfBytes = PdfService.GenerateReceipts(student);

            Response.Headers.Add("Content-Disposition", $"inline; filename=Receipt_{student.Name}.pdf");

            return File(pdfBytes, "application/pdf");
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
            await CountTotalFee();
            return RedirectToAction("List");
        }

    
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
            await CountTotalFee();
            return RedirectToAction(nameof(List));  
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
            await CountTotalFee();
            return RedirectToAction(nameof(List));
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
       

        private async Task CountTotalFee()
        {
            decimal? totalFee = await _context.Students.SumAsync(x => x.Fee);
            ViewBag.TotalFee = totalFee ?? 0;
        }
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
