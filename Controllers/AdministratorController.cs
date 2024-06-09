using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingHub.Data;
using MarketingHub.Models;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

[Authorize(Roles =MarketingHub.Utility.SD.Role_Admin)]

public class AdministratorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdministratorController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;

    }

    public async Task<IActionResult> Index(string search)
    {
        IQueryable<MarketingAgency> allAgencies = _context.MarketingAgency;

        if (!string.IsNullOrEmpty(search))
        {
            allAgencies = allAgencies.Where(a => a.Username.ToLower().Contains(search.ToLower()));
        }

        List<MarketingAgency> marketingAgencies = await allAgencies.ToListAsync();

        return View(marketingAgencies);
    }

    public async Task<IActionResult> DisplayAdmin()
    {
        List<Administrator> administrators= await _context.Administrators.ToListAsync();
        return View(administrators);
    }
    [HttpGet]

    public async Task<IActionResult> SearchAdmin(string search)
    {
        var administrators = await _context.Administrators
            .Where(a => a.FirstName.Contains(search) || a.LastName.Contains(search))
            .ToListAsync();

        return View("DisplayAdmin", administrators);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var marketingAgency = await _context.MarketingAgency
            .Include(m => m.Location)
            .FirstOrDefaultAsync(m => m.MarketingAgencyId == id);

        if (marketingAgency == null)
        {
            return NotFound();
        }

        // Delete associated locations
        if (marketingAgency.Location != null)
        {
            _context.Remove(marketingAgency.Location);
        }

        // Delete marketing agency
        _context.Remove(marketingAgency);

        await _context.SaveChangesAsync();

        // Delete associated user
        var user = await _userManager.FindByNameAsync(marketingAgency.Username);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                // Handle the case where user deletion fails
                // You might want to log or display an error message
            }
        }

        return RedirectToAction("Index");
    }

    //public async Task<IActionResult> Delete(int id)
    //{
    //    var marketingAgency = await _context.MarketingAgency
    //        .FirstOrDefaultAsync(m => m.MarketingAgencyId == id);

    //    if (marketingAgency == null)
    //    {
    //        return NotFound();
    //    }

    //    if (marketingAgency.Location != null)
    //    {
    //        _context.Remove(marketingAgency.Location);
    //    }

    //    _context.Remove(marketingAgency);

    //    await _context.SaveChangesAsync();

    //    var user = await _userManager.FindByNameAsync(marketingAgency.Username);
    //    if (user != null)
    //    {
    //        var result = await _userManager.DeleteAsync(user);


    //    }

    //    return RedirectToAction("Index");
    //}


    public async Task<IActionResult> Details(int? id)
    {
        var marketingAgency = await _context.MarketingAgency.FirstOrDefaultAsync(a => a.MarketingAgencyId == id);

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



    [HttpGet]
    public IActionResult AddAgency()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessAddAgency(MarketingAgency marketingAgency)
    {
        // Check if the email already exists in the ApplicationUser table
        var existingUser = await _userManager.FindByEmailAsync(marketingAgency.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError(string.Empty, "Email is already in use.");
            return View("AddAgency", marketingAgency);
        }

        // Create a new ApplicationUser based on the MarketingAgency data
        var user = new ApplicationUser
        {
            UserName = marketingAgency.Username,
            Email = marketingAgency.Email,
            Name = marketingAgency.Username
        };

        // Attempt to create the ApplicationUser
        var result = await _userManager.CreateAsync(user, "Mm_123");

        // Set the UserId property of the MarketingAgency to the Id of the newly created user
        marketingAgency.UserId = user.Id;

        // Check if the ApplicationUser creation was successful
        if (result.Succeeded)
        {
            // Add the MarketingAgency to the context and save changes
            _context.MarketingAgency.Add(marketingAgency);
            await _context.SaveChangesAsync();

            // Redirect to the Index action if successful
            return RedirectToAction("Index");
        }
        else
        {
            // If ApplicationUser creation failed, add errors to ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If ModelState is not valid or ApplicationUser creation failed, return to the view with the model
        return View("AddAgency", marketingAgency);
    }






    public async Task<IActionResult> EditAgency(int? id)
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
    public async Task<IActionResult> ProcessEditAgency(MarketingAgency marketingAgency)
    {
        if (!ModelState.IsValid)
        {
            // If ModelState is not valid, return to the EditAgency view with the model to show validation errors
            return View("EditAgency", marketingAgency);
        }

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

    public async Task<IActionResult> Portfolio(int? id)
    {
        var posts = await _context.Posts.Where(p => p.MarketingAgencyId == id).ToListAsync();
        return View(posts);
    }

}
