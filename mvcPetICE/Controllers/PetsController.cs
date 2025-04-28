using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcPetICE.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace mvcPetICE.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Pets = await _context.Pets.ToListAsync();
            return View(Pets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var Pets = await _context.Pets
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Pets == null)
                return NotFound();

            return View(Pets);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(Pets Pets)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Pets);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Pets);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Pets = await _context.Pets.FindAsync(id);

            if (Pets == null)
            {
                return NotFound();
            }
            return View(Pets);
        }

        private bool PetsExists(int id)
        {
            return _context.Pets.Any(c => c.Id == id);
        }
        [HttpPost]

        public async Task<IActionResult> Edit(int id, Pets Pets)
        {
            if (id != Pets.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Pets);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetsExists(Pets.Id))
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
            return View(Pets);
        }
    }
}
