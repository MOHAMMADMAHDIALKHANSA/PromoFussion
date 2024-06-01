using MarketingHub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = MarketingHub.Utility.SD.Role_Admin)]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var customerCount = _context.Customers.Count();
        var agencyCount = _context.MarketingAgency.Count();
        var adminCount = _context.Administrators.Count();

        var customerList = _context.Customers.ToList();
        var agencyList = _context.MarketingAgency.ToList();

        var dashboardModel = new DashboardViewModel
        {
            CustomerCount = customerCount,
            AgencyCount = agencyCount,
            AdminCount = adminCount,
            CustomerList = customerList,
            AgencyList = agencyList
        };

        return View(dashboardModel);
    }
}
