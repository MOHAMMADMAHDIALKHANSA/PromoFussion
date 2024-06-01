using Microsoft.AspNetCore.Mvc;
using MarketingHub.Data;
using MarketingHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketingHub.Controllers
{
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Location> locations = await _context.Locations.ToListAsync();
            return View(locations);
        }
    }
}
