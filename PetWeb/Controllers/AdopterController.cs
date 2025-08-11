using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetWeb.Data;
using PetWeb.Models;

namespace PetWeb.Controllers
{
    public class AdopterController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdopterController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string? location, PetType? type)
        {
            var query = _db.Pets.AsQueryable();
            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(p => p.Location != null && EF.Functions.Like(p.Location, $"%{location}%"));
            }
            if (type.HasValue)
            {
                query = query.Where(p => p.Type == type.Value);
            }
            var results = await query.OrderBy(p => p.Name).ToListAsync();
            ViewBag.Location = location;
            ViewBag.Type = type;
            return View(results);
        }
    }
}