using Microsoft.AspNetCore.Mvc;

namespace SDHRM.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
