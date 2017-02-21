namespace SimpleMVC.App.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using SimpleHttpServer.Models;
    using SimpleMVC.App.BindingModels;
    using SimpleMVC.App.Data;
    using SimpleMVC.App.Models;
    using SimpleMVC.App.MVC.Attributes.Methods;
    using SimpleMVC.App.MVC.Controllers;
    using SimpleMVC.App.MVC.Interfaces;
    using SimpleMVC.App.MVC.Interfaces.Generic;
    using SimpleMVC.App.MVC.Security;
    using SimpleMVC.App.ViewModels;

    public class UsersController : Controller
    {
        private SingInManager singInManager;

        public UsersController()
        {
            this.singInManager = new SingInManager(new NotesApplicationContext());
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult<UserViewModel> Register(RegisterUserBindingModel model)
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
            UserViewModel viewModel = new UserViewModel()
            {
                Username = model.Username
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult<IEnumerable<AllUsernamesViewModel>> All(HttpSession session, HttpResponse response)
        {
            if (!this.singInManager.IsAuthenticated(session))
            {
                Redirect(response, "/users/login");
                return null;
            }
            List<User> users = null;
            using (var context = new NotesApplicationContext())
            {
                users = context.Users.ToList();
            }

            List<AllUsernamesViewModel> models = new List<AllUsernamesViewModel>();
            foreach (User user in users)
            {
                models.Add(new AllUsernamesViewModel()
                {
                    UserId = user.Id,
                    Username = user.Username
                });
            }

            return View(models.AsEnumerable());
        }

        [HttpGet]
        public IActionResult<UserProfileViewModel> Profile(int id)
        {
            using (var context = new NotesApplicationContext())
            {
                User user = context.Users.Find(id);
                UserProfileViewModel model = new UserProfileViewModel()
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Notes = user.Notes.Select(x => new NoteViewModel()
                    {
                        Title = x.Title,
                        Content = x.Content
                    })
                };
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult<UserProfileViewModel> Profile(AddNoteBindingModel model)
        {
            using (var context = new NotesApplicationContext())
            {
                User user = context.Users.Find(model.UserId);
                Note note = new Note()
                {
                    Title = model.Title,
                    Content = model.Content
                };
                user.Notes.Add(note);
                context.SaveChanges();
            }
            return Profile(model.UserId);
        }

        [HttpGet]
        public IActionResult Login(LoginUserBindingModel model, HttpSession session, HttpResponse response)
        {
            string username = model.Username;
            string password = model.Password;
            string sessionId = session.Id;

            using (var context = new NotesApplicationContext())
            {
                User user = context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    Login login = new Login()
                    {
                        User = user, SessionId = sessionId
                    };
                    context.Logins.Add(login);
                    context.SaveChanges();
                    Redirect(response, "/home/index");
                    return null;
                }
            }

            return this.View();
        }
    }
}
