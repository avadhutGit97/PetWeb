using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetWeb.Data;
using PetWeb.Models;
using PetWeb.Services;

namespace PetWeb.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IImageStorageService _imageStorageService;

        public PetsController(ApplicationDbContext db, IImageStorageService imageStorageService)
        {
            _db = db;
            _imageStorageService = imageStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var pets = await _db.Pets.OrderByDescending(p => p.Id).ToListAsync();
            return View(pets);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return View(pet);
            }
            _db.Add(pet);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
            if (pet == null) return NotFound();
            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pet pet)
        {
            if (id != pet.Id) return BadRequest();
            if (!ModelState.IsValid) return View(pet);

            _db.Update(pet);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
            if (pet == null) return NotFound();
            return View(pet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
            if (pet != null)
            {
                if (!string.IsNullOrWhiteSpace(pet.ImageUrl))
                {
                    await _imageStorageService.DeleteAsync(pet.ImageUrl);
                }
                _db.Pets.Remove(pet);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            var file = Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                var newUrl = await _imageStorageService.UploadAsync(file, pet.Name);
                if (!string.IsNullOrWhiteSpace(newUrl))
                {
                    if (!string.IsNullOrWhiteSpace(pet.ImageUrl))
                    {
                        await _imageStorageService.DeleteAsync(pet.ImageUrl);
                    }
                    pet.ImageUrl = newUrl;
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
            if (pet == null) return NotFound();
            return View(pet);
        }
    }
}