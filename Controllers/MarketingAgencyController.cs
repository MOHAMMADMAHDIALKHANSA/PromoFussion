using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingHub.Data;
using MarketingHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = MarketingHub.Utility.SD.Role_Agency)]

public class MarketingAgencyController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MarketingAgencyController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
    public IActionResult Requests()
    {
        var userId = _userManager.GetUserId(User); // Get current agency ID
        var requests = _context.Appointments
                               .Where(r => r.MarketingAgency.UserId == userId)
                               .Include(r => r.Customer)
                               .ToList();

        return View(requests);
    }

    // Approving an appointment request
    [HttpPost]
    public IActionResult Approve(int appointmentId)
    {
        var appointment = _context.Appointments.Find(appointmentId);
        if (appointment != null)
        {
            appointment.Status = "Accepted";
            _context.SaveChanges();
        }
        return RedirectToAction("Requests");
    }

    // Rejecting an appointment request
    [HttpPost]
    public IActionResult Reject(int appointmentId)
    {
        var appointment = _context.Appointments.Find(appointmentId);
        if (appointment != null)
        {
            appointment.Status = "Rejected";
            _context.SaveChanges();
        }
        return RedirectToAction("Requests");
    }
    public async Task<IActionResult> Profile()
    {
        // Get the current user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Find the associated marketing agency
        var agency = await _context.MarketingAgency
            .Include(a => a.Location)
            .FirstOrDefaultAsync(a => a.UserId == user.Id);

        if (agency == null)
        {
            return NotFound();
        }

        return View(agency);
    }

    public async Task<IActionResult> EditProfile(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var marketingAgency = await _context.MarketingAgency.FindAsync(id);

        if (marketingAgency == null)
        {
            return NotFound();
        }

        var locations = await _context.Locations.ToListAsync();

        ViewBag.Locations = new SelectList(locations, "LocationId", "LocationName");

        return View(marketingAgency);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(MarketingAgency marketingAgency)
    {
      

        try
        {
            // Update marketing agency details

            // Find associated application user
            var user = await _userManager.FindByIdAsync(marketingAgency.UserId);

            if (user != null)
            {
                // Update user email if changed
                if (user.Email != marketingAgency.Email)
                {
                    user.Email = marketingAgency.Email;
                    await _userManager.UpdateAsync(user);
                }

                user.UserName = marketingAgency.Username; // Always update username

                // Update user in the context
                _context.Update(user);
            }

            // Update location ID
            marketingAgency.Location.LocationId = marketingAgency.LocationId;

            // Update marketing agency in the context
            _context.MarketingAgency.Update(marketingAgency);

            // Find existing location
            var existingLocation = await _context.Locations.FindAsync(marketingAgency.LocationId);
            if (existingLocation != null)
            {
                // Update location details
                existingLocation = marketingAgency.Location;

                // Update location in the context
                _context.Locations.Update(existingLocation);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound(); // MarketingAgency with given ID not found
        }
    }
}
