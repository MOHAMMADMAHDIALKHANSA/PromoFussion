using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingHub.Data;
using MarketingHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = MarketingHub.Utility.SD.Role_Agency)]

public class MarketingAgencyController : Controller
{
    private readonly ApplicationDbContext _context;

    public MarketingAgencyController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string search)
    {
        // Retrieve all agencies from your data source
        IQueryable<MarketingAgency> allAgencies = _context.MarketingAgency;

        // If search parameter is provided, filter the list of agencies by username
        if (!string.IsNullOrEmpty(search))
        {
            allAgencies = allAgencies.Where(a => a.Username.ToLower().Contains(search.ToLower()));
        }

        // Convert to a list to execute the query
        List<MarketingAgency> marketingAgencies = await allAgencies.ToListAsync();

        return View(marketingAgencies);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        MarketingAgency marketingAgency = await _context.MarketingAgency.FirstOrDefaultAsync(a => a.MarketingAgencyId == id);

        if (marketingAgency == null)
        {
            return NotFound();
        }

        return View(marketingAgency);
    }

    public async Task<IActionResult> ViewLocation(int? id)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(a => a.LocationId == id);

        if (location == null)
        {
            return NotFound();
        }

        return View(location);
    }
    public async Task<IActionResult> Portfolio(int? id)
    {
        var posts = await _context.Posts.Where(p => p.MarketingAgencyId == id).ToListAsync();
        return View(posts);
    }

}
