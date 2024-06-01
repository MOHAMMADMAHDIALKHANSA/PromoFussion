using MarketingHub.Data;
using MarketingHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Your asynchronous logic goes here if needed
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            // Your asynchronous logic goes here if needed
            return View();
        }
        public async Task<IActionResult> About()
        {
            List<Administrator> administrators = await _context.Administrators.ToListAsync();
            return View(administrators);
        }


        public async Task<IActionResult> ContactUs()
        {
            // Your asynchronous logic goes here if needed
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            // Your asynchronous logic goes here if needed
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ViewDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Administrators.FirstOrDefaultAsync(a => a.AdministratorId == id);

            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

    }


}
