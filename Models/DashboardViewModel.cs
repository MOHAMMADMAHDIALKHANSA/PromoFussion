using MarketingHub.Models;

public class DashboardViewModel
{
    public int CustomerCount { get; set; }
    public int AgencyCount { get; set; }
    public int AdminCount { get; set; }

    public List<Customer> CustomerList { get; set; }
    public List<MarketingAgency> AgencyList { get; set; }
    // Add other properties if needed
}
