namespace SimpleMVC.App.MVC.Interfaces
{
    using System.Data.Entity;
    using SimpleMVC.App.Models;

    public interface IDbIdentityContext
    {
        DbSet<User> Users { get; }
        DbSet<Login> Logins { get; }

        void SaveChanges();
    }
}
