using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetWeb.Data;
using PetWeb.Models;

namespace PetWeb.Controllers
{
    public class AdoptionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdoptionsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
            if (pet == null) return NotFound();
            ViewBag.Pet = pet;
            return View(new AdoptionRequest { PetId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdoptionRequest request)
        {
            var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == request.PetId);
            if (pet == null) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewBag.Pet = pet;
                return View(request);
            }
            _db.AdoptionRequests.Add(request);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Success), new { id = request.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            var req = await _db.AdoptionRequests.FindAsync(id);
            if (req == null) return NotFound();
            var pet = await _db.Pets.FindAsync(req.PetId);
            ViewBag.Pet = pet;
            return View(req);
        }
    }
}