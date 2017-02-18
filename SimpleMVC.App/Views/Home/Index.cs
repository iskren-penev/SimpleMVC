namespace SimpleMVC.App.Views.Home
{
    using SimpleMVC.App.MVC.Interfaces;

    public class Index : IRenderable
    {
        public string Render()
        {
            return "<h3>Hello MVC</h3>";
        }
    }
}
