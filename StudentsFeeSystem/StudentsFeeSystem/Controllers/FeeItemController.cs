using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsFeeSystem.Data;
using StudentsFeeSystem.Models;
using StudentsFeeSystem.ViewModel;

namespace StudentsFeeSystem.Controllers
{
    public class FeeItemController : Controller
    {
        private readonly AppDbContext _context;

        public FeeItemController(AppDbContext context)
        {
            _context = context;
        }

        // GET: FeeItem
        public async Task<IActionResult> Index()
        {
            var feeItems = await _context.FeeItems.ToListAsync();
            return View(feeItems);
        }

        // GET: FeeItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var feeItem = await _context.FeeItems
                .FirstOrDefaultAsync(m => m.Id == id);

            if (feeItem == null) return NotFound();

            return View(feeItem);
        }

        // GET: FeeItem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FeeItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Value")] FeeItem feeItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feeItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feeItem);
        }

        [HttpPost]
        public async Task<IActionResult> ResetGenderAssignment(int id)
        {
            var item = await _context.FeeItems.FindAsync(id);
            if (item == null) return NotFound();

            item.AssignedToMale = false;
            item.AssignedToFemale = false;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: FeeItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var feeItem = await _context.FeeItems.FindAsync(id);
            if (feeItem == null) return NotFound();

            return View(feeItem);
        }

        // POST: FeeItem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Value")] FeeItem feeItem)
        {
            if (id != feeItem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feeItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeeItemExists(feeItem.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(feeItem);
        }

        // POST: FeeItem/AssignToClass
        [HttpPost]
        public async Task<IActionResult> AssignToClass(int id, int classNumber)
        {
            var item = await _context.FeeItems.FindAsync(id);
            if (item == null) return NotFound();

            var propertyName = $"AssignedToClass{classNumber}";
            var property = item.GetType().GetProperty(propertyName);
            if (property != null)
            {
                property.SetValue(item, true);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: FeeItem/AssignToGender
        [HttpPost]
        public async Task<IActionResult> AssignToGender(int id, string gender)
        {
            var item = await _context.FeeItems.FindAsync(id);
            if (item == null) return NotFound();

            switch (gender)
            {
                case "Male":
                    item.AssignedToMale = true;
                    item.AssignedToFemale = false;
                    break;
                case "Female":
                    item.AssignedToMale = false;
                    item.AssignedToFemale = true;
                    break;
                case "Both":
                    item.AssignedToMale = true;
                    item.AssignedToFemale = true;
                    break;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ResetClassAssignment(int id)
        {
            var item = await _context.FeeItems.FindAsync(id);
            if (item == null) return NotFound();

            for (int i = 6; i <= 12; i++)
            {
                var propertyName = $"AssignedToClass{i}";
                var property = item.GetType().GetProperty(propertyName);
                if (property != null && property.PropertyType == typeof(bool))
                {
                    property.SetValue(item, false);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: FeeItem/Delete/5
        [Route("/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var feeItem = await _context.FeeItems.FindAsync(id);
            if (feeItem != null)
            {
                _context.FeeItems.Remove(feeItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FeeItemExists(int id)
        {
            return _context.FeeItems.Any(e => e.Id == id);
        }

        // GET: MakePayment
        [HttpGet]
        public IActionResult MakePayment(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

          
            List<FeeItem> feeItems = _context.FeeItems.ToList();

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

            // Create ViewModel
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

        // POST: MakePayment
      
        [HttpPost]
        public IActionResult MakePayment(ItemListViewModel model)
        {
            var student = _context.Students.Find(model.Id);
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
                    var feeItem = _context.FeeItems.Find(item.Id);
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
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        private bool IsFeeItemAssignedToClass(FeeItem feeItem, int studentClass)
        {
            var propertyName = $"AssignedToClass{studentClass}";
            var property = feeItem.GetType().GetProperty(propertyName);

            if (property != null)
            {
                return (bool)property.GetValue(feeItem);
            }

            return false;
        }
    }
}
