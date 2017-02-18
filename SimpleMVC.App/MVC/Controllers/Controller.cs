namespace SimpleMVC.App.MVC.Controllers
{
    using System.Runtime.CompilerServices;
    using SimpleMVC.App.MVC.Interfaces;
    using SimpleMVC.App.MVC.Interfaces.Generic;
    using SimpleMVC.App.MVC.ViewEngline;
    using SimpleMVC.App.MVC.ViewEngline.Generic;

    public class Controller
    {
        protected IActionResult View([CallerMemberName]string callee = "")
        {
            string controllerName = this.GetType()
                .Name
                .Replace(MvcContext.Current.ControllersSuffix, string.Empty);

            string qualifiedName = string.Format(
                "{0}.{1}.{2}.{3}",
                MvcContext.Current.AssemblyName,
                MvcContext.Current.ViewsFolder,
                controllerName,
                callee);

            return new ActionResult(qualifiedName);
        }

        protected IActionResult View(string controller, string action)
        {
            string qualifiedName = string.Format(
                "{0}.{1}.{2}.{3}",
                MvcContext.Current.AssemblyName,
                MvcContext.Current.ViewsFolder,
                controller,
                action);

            return new ActionResult(qualifiedName);
        }

        protected IActionResult<T> View<T>(T model, [CallerMemberName] string callee = "")
        {
            string controllerName = this.GetType()
                .Name
                .Replace(MvcContext.Current.ControllersSuffix, string.Empty);

            string qualifiedName = string.Format(
                "{0}.{1}.{2}.{3}",
                MvcContext.Current.AssemblyName,
                MvcContext.Current.ViewsFolder,
                controllerName,
                callee);

            return new ActionResult<T>(qualifiedName, model);
        }

        protected IActionResult<T> View<T>(T model, string controller, string action)
        {
            string qualifiedName = string.Format(
                "{0}.{1}.{2}.{3}",
                MvcContext.Current.AssemblyName,
                MvcContext.Current.ViewsFolder,
                controller,
                action);

            return new ActionResult<T>(qualifiedName, model);
        }
    }
}
