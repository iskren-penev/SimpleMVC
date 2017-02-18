namespace SimpleMVC.App.Controllers
{
    using SimpleMVC.App.MVC.Controllers;
    using SimpleMVC.App.MVC.Interfaces;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
