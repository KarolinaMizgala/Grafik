using Microsoft.AspNetCore.Mvc;

namespace ScheduleAppCore.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Schedules");
    }
}
