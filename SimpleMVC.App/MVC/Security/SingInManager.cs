namespace SimpleMVC.App.MVC.Security
{
    using System.Collections.Generic;
    using System.Linq;
    using SimpleHttpServer.Models;
    using SimpleMVC.App.Data;
    using SimpleMVC.App.MVC.Interfaces;

    public class SingInManager
    {
        private IDbIdentityContext dbContext;

        public SingInManager(NotesApplicationContext context)
        {
            this.dbContext = context;
        }

        public bool IsAuthenticated(HttpSession session)
        {
            List<string> sessionIds = this.dbContext.Logins.Select(l => l.SessionId).ToList();
            if (sessionIds.Contains(session.Id))
            {
                return true;
            }

            return false;
        }
    }
}
