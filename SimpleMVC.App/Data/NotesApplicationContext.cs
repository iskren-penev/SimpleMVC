namespace SimpleMVC.App.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using SimpleMVC.App.Models;

    public class NotesApplicationContext : DbContext
    {
        public NotesApplicationContext()
            : base("name=NotesApplicationContext")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Note> Notes { get; set; }
    }
}