using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingHub.Data;
using MarketingHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

[Authorize(Roles = MarketingHub.Utility.SD.Role_Cust)]


public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CustomerController(ApplicationDbContext context,UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Inside CustomerController

    public async Task<IActionResult> Index(string search)
    {
        IQueryable<MarketingAgency> allAgencies = _context.MarketingAgency.Include(a => a.Feedbacks);

        if (!string.IsNullOrEmpty(search))
        {
            allAgencies = allAgencies.Where(a => a.Username.ToLower().Contains(search.ToLower()));
        }

        List<MarketingAgency> marketingAgencies = await allAgencies.ToListAsync();

        // Calculate average rating for each agency
        foreach (var agency in marketingAgencies)
        {
            if (agency.Feedbacks.Any())
            {
                agency.Rating = agency.Feedbacks.Average(f => f.Rating);
            }
            else
            {
                agency.Rating = 0; // Or any default value you prefer
            }
        }

        return View(marketingAgencies);
    }


    public IActionResult Details(int id)
    {
        // Retrieve the marketing agency including its associated feedbacks and customers
        var agency = _context.MarketingAgency
                             .Include(a => a.Feedbacks) // Include feedbacks
                                 .ThenInclude(f => f.Customer) // Include customers of feedbacks
                             .FirstOrDefault(a => a.MarketingAgencyId == id);

        if (agency == null)
        {
            return NotFound();
        }

        return View(agency);
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


    [HttpPost]
    public IActionResult Book(int MarketingAgencyId, DateTime PreferredDate)
    {
        // Get current user ID
        var userId = _userManager.GetUserId(User);

        // Find the customer associated with the user ID
        var customer = _context.Customers.SingleOrDefault(c => c.applicationUser.Id == userId);
        if (customer == null)
        {
            // Handle the case where the customer is not found
            return NotFound("Customer not found.");
        }

        // Create the appointment
        var appointment = new Appointment
        {
            MarketingAgencyId = MarketingAgencyId,
            CustomerId = customer.CustomerId,
            RequestedDate = PreferredDate,
            Status = "Pending"
        };

        // Add and save the appointment
        _context.Appointments.Add(appointment);
        _context.SaveChanges();

        // Redirect to a confirmation page or home
        return RedirectToAction("Index");
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

    public async Task<IActionResult> MyRequests()
    {
        var userId = _userManager.GetUserId(User);
        var customer = await _context.Customers.Include(c => c.Appointments)
                                               .ThenInclude(a => a.MarketingAgency)
                                               .SingleOrDefaultAsync(c => c.applicationUser.Id == userId);
        if (customer == null)
        {
            return NotFound("Customer not found.");
        }

        return View(customer.Appointments);
    }

    [HttpPost]
    public async Task<IActionResult> DeclineRequest(int appointmentId)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment != null)
        {
            appointment.Status = "Rejected";
            _context.SaveChanges();
        }
        return RedirectToAction("MyRequests");
    }
    [HttpPost]
    public async Task<IActionResult> SubmitFeedback(Feedback feedback)
    {
        
            var userId = _userManager.GetUserId(User);

            // Find the CustomerId associated with the UserId
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.applicationUser.Id == userId);

            if (customer != null)
            {
                // Check if the customer has already submitted feedback for this agency
                var existingFeedback = await _context.Feedbacks.FirstOrDefaultAsync(f =>
                    f.CustomerId == customer.CustomerId && f.MarketingAgencyId == feedback.MarketingAgencyId);

                if (existingFeedback != null)
                {
                    // Update existing feedback
                    existingFeedback.Rating = feedback.Rating;
                    existingFeedback.Comment = feedback.Comment;
                    existingFeedback.Date = DateTime.Now;

                    _context.Feedbacks.Update(existingFeedback);
                }
                else
                {
                    // If no existing feedback, add new feedback
                    feedback.CustomerId = customer.CustomerId;
                    feedback.Date = DateTime.Now;
                    _context.Feedbacks.Add(feedback);
                }

                await _context.SaveChangesAsync();

                // Redirect to the details view of the marketing agency
                return RedirectToAction("Details", new { id = feedback.MarketingAgencyId });
            }
        
        // If ModelState is not valid or customer not found, return to the previous page
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Profile()
    {
        // Retrieve the current user
        ApplicationUser currentUser = (ApplicationUser)await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            // Handle the case where the current user is not found
            return NotFound();
        }

        // Find the customer associated with the current user
        Customer customer = await _context.Customers.Include(c => c.Reviews).FirstOrDefaultAsync(c => c.applicationUser.Id == currentUser.Id);

        if (customer == null)
        {
            // Handle the case where the customer for the current user is not found
            return NotFound();
        }

        return View(customer);
    }

    // GET: Customer/EditProfile
    public async Task<IActionResult> EditProfile()
    {
        // Retrieve the current user
        ApplicationUser currentUser = (ApplicationUser)await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            // Handle the case where the current user is not found
            return NotFound();
        }

        // Find the customer associated with the current user
        Customer customer = await _context.Customers
                                            .FirstOrDefaultAsync(c => c.applicationUser.Id == currentUser.Id);

        if (customer == null)
        {
            // Handle the case where the customer for the current user is not found
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(Customer customer)
    {
        // Retrieve the current user
        var currentUser = await _userManager.GetUserAsync(User);
        customer.applicationUser = (ApplicationUser)currentUser;
       
        if (ModelState.IsValid)
        {
            try
            {
                // Update the customer profile
                // Set the ApplicationUser reference to the current user
                

                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile");
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound(); // Handle the case where the customer is not found
            }
        }
        return View(customer);
    }


}
