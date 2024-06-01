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
        // Create a new ApplicationUser based on the MarketingAgency data
        var user = new ApplicationUser { UserName = marketingAgency.Username, Email = marketingAgency.Email, Name = "ali" };

        // Attempt to create the ApplicationUser
        var result = await _userManager.CreateAsync(user, "Mm_123");

        // Set the UserId property of the MarketingAgency to the Id of the newly created user
        marketingAgency.UserId = user.Id;


        // Check if the ModelState is valid before proceeding
        
        
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

    // [HttpPost]    
    //[ValidateAntiForgeryToken]
    // public async Task<IActionResult> ProcessEditAgency(MarketingAgency marketingAgency)
    // {

    //         try
    //         {
    //         // Find the marketing agency including its associated user and location
    //         var agencyToUpdate = await _context.MarketingAgency.FindAsync(marketingAgency.MarketingAgencyId);

    //             if (agencyToUpdate == null)
    //             {
    //                 return NotFound(); // MarketingAgency with given ID not found
    //             }

    //             // Update marketing agency details
    //             agencyToUpdate.Username = marketingAgency.Username;
    //             agencyToUpdate.Salary = marketingAgency.Salary;
    //             agencyToUpdate.Email = marketingAgency.Email;
    //             agencyToUpdate.PhoneNumber = marketingAgency.PhoneNumber;
    //             agencyToUpdate.Description = marketingAgency.Description;
    //             agencyToUpdate.Rating = marketingAgency.Rating;
    //             agencyToUpdate.ImageUrl = marketingAgency.ImageUrl;
    //             agencyToUpdate.Instagram = marketingAgency.Instagram;
    //             agencyToUpdate.Facebook = marketingAgency.Facebook;
    //             agencyToUpdate.Twitter = marketingAgency.Twitter;
    //             agencyToUpdate.LinkedIn = marketingAgency.LinkedIn;
    //             agencyToUpdate.Location = marketingAgency.Location;

    //             //// Update associated application user's email if changed
    //             //if (agencyToUpdate.applicationUser != null && agencyToUpdate.applicationUser.Email != marketingAgency.Email)
    //             //{
    //             //    agencyToUpdate.applicationUser.Email = marketingAgency.Email;
    //             //}

    //             //// Update associated location if changed
    //             //if (agencyToUpdate.Location != null && agencyToUpdate.Location.LocationId != marketingAgency.Location?.LocationId)
    //             //{
    //             //    agencyToUpdate.Location = marketingAgency.Location;
    //             //}

    //             await _context.SaveChangesAsync();

    //             return RedirectToAction("Index");
    //         }
    //         catch (DbUpdateConcurrencyException)
    //         {
    //             return NotFound(); // MarketingAgency with given ID not found
    //         }


    //     // If ModelState is not valid, return to the EditAgency view with the model to show validation errors
    //     return View("EditAgency", marketingAgency);
    // }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessEditAgency(MarketingAgency marketingAgency)
    {

            try
            {
            // Update marketing agency details

            // Find associated application user
            var user = await _userManager.FindByIdAsync(marketingAgency.UserId);

                if (user != null)
                {
                    // Update user email if changed
                        user.Email = marketingAgency.Email;
                        user.UserName = marketingAgency.Username;

                        await _userManager.UpdateAsync(user);
                    
                }

            marketingAgency.UserId = user.Id;

            marketingAgency.Location.LocationId = marketingAgency.LocationId;

            _context.MarketingAgency.Update(marketingAgency);

            var existingLocation = await _context.Locations.FindAsync(marketingAgency.LocationId);
            if (existingLocation != null)
            {
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
        

        // If ModelState is not valid, return to the EditAgency view with the model to show validation errors
        return View("EditAgency", marketingAgency);
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
