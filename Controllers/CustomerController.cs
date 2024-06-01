using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingHub.Data;
using MarketingHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize(Roles = MarketingHub.Utility.SD.Role_Cust)]


public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        MarketingAgency marketingAgency = await _context.MarketingAgency.FindAsync(id);

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


    public async Task<IActionResult> Register(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        MarketingAgency marketingAgency = await _context.MarketingAgency.FindAsync(id);

        if (marketingAgency == null)
        {
            return NotFound();
        }

        // Get the current customer ID from the claims
        string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
        {
            // Redirect to login or handle unauthorized access
            return RedirectToAction("Login", "Account");
        }

        // Check if the customer is already registered with the agency
        //bool isAlreadyRegistered = _context.MarketingAgencyRegistrations
        //    .Any(r => r.CustomerId == currentUserId && r.MarketingAgencyId == marketingAgency.MarketingAgencyId);

        //if (isAlreadyRegistered)
        //{
        //    // Handle case where customer is already registered
        //    // You might want to redirect with a message or show a message on the current view
        //    // Example: return RedirectToAction("Details", new { id = marketingAgency.MarketingAgencyId });
        //}

        // Create a new MarketingAgencyRegistration instance
        MarketingAgencyRegistration registration = new MarketingAgencyRegistration
        {
            RegistrationDate = DateTime.Now,
            CustomerId = "6",
            MarketingAgencyId = marketingAgency.MarketingAgencyId
        };

        // Save the registration to the database
        _context.MarketingAgencyRegistrations.Add(registration);
        await _context.SaveChangesAsync();

        // Redirect to the agency details page
        return RedirectToAction("Details", new { id = marketingAgency.MarketingAgencyId });
    }

    public async Task<IActionResult> SubmitReview()
    {

        Review review = new Review();
        review.CustomerId= "6";
        return View(review);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReviewAsync(Review review)
    {
        

        if (ModelState.IsValid)
        {
            // Add the review to the context and save changes
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Redirect to a thank you page or customer details page
            return RedirectToAction("ThankYou");
        }

        // If the model is not valid or user not found, return to the form with validation errors
        return View(review);
    }



    public IActionResult ThankYou()
    {
        return View();
    }

    public async Task<IActionResult> Portfolio(int? id)
    {
        var posts = await _context.Posts.Where(p => p.MarketingAgencyId == id).ToListAsync();
        return View(posts);
    }

}
