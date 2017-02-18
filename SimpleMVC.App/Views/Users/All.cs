namespace SimpleMVC.App.Views.Users
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SimpleMVC.App.MVC.Interfaces.Generic;
    using SimpleMVC.App.ViewModels;

    public class All : IRenderable<IEnumerable<AllUsernamesViewModel>>
    {
        public AllUsernamesViewModel Model { get; set; }

        IEnumerable<AllUsernamesViewModel> IRenderable<IEnumerable<AllUsernamesViewModel>>.Model { get; set; }

        public string Render()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<h2>All Users</h2>");
            sb.AppendLine("<a href=\"/home/index\">Home<a/>");
            sb.AppendLine("<ul>");
            foreach (AllUsernamesViewModel model in ((IRenderable<IEnumerable<AllUsernamesViewModel>>)this).Model)
            {
                sb.AppendLine($"<li><a href=\"/users/profile?id={model.UserId}\">{model.Username}</a></li>");
            }
            sb.AppendLine("</ul");

            return sb.ToString();
        }

    }
}
