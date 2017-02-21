namespace SimpleMVC.App.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using SimpleMVC.App.Models;
    using SimpleMVC.App.MVC.Interfaces;

    public class NotesApplicationContext : DbContext, IDbIdentityContext
    {
        public NotesApplicationContext()
            : base("name=NotesApplicationContext")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Login> Logins { get; }


        public new void SaveChanges()
        {
            base.SaveChanges();
        }
    }
}