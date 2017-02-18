﻿namespace SimpleMVC.App.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using SimpleMVC.App.BindingModels;
    using SimpleMVC.App.Data;
    using SimpleMVC.App.Models;
    using SimpleMVC.App.MVC.Attributes.Methods;
    using SimpleMVC.App.MVC.Controllers;
    using SimpleMVC.App.MVC.Interfaces;
    using SimpleMVC.App.MVC.Interfaces.Generic;
    using SimpleMVC.App.ViewModels;

    public class UsersController : Controller
    {
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
        public IActionResult<IEnumerable<AllUsernamesViewModel>> All()
        {
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
    }
}
