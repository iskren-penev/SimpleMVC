namespace SimpleMVC.App.Controllers
{
    using SimpleMVC.App.BindingModels;
    using SimpleMVC.App.Data;
    using SimpleMVC.App.Models;
    using SimpleMVC.App.MVC.Attributes.Methods;
    using SimpleMVC.App.MVC.Controllers;
    using SimpleMVC.App.MVC.Interfaces;

    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterUserBindingModel model)
        {
            User user = new User()
            {
                Username = model.Username,
                Password = model.Password
            };
            using (var context = new NotesApplicationContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            return View();
        }
    }
}
