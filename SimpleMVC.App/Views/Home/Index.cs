namespace SimpleMVC.App.Views.Home
{
    using System.Text;
    using SimpleMVC.App.MVC.Interfaces;

    public class Index : IRenderable
    {
        public string Render()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<h1>Notes Application</h1>");
            sb.AppendLine("<a href=\"/users/all\">All users<a/>");
            sb.AppendLine("<a href=\"/users/register\">Register<a/>");
            return sb.ToString();
        }
    }
}
