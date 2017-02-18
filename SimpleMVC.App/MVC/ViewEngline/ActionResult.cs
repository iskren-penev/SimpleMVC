namespace SimpleMVC.App.MVC.ViewEngline
{
    using System;
    using SimpleMVC.App.MVC.Interfaces;
    public class ActionResult : IActionResult
    {
        public ActionResult(string viewFullQualifiedName)
        {
            this.Action = (IRenderable)Activator.CreateInstance(Type.GetType(viewFullQualifiedName));
        }

        public IRenderable Action { get; set; }

        public string Invoke()
        {
            return this.Action.Render();
        }

    }
}
