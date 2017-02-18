namespace SimpleMVC.App
{
    using SimpleHttpServer;
    using SimpleMVC.App.MVC;

    public class AppStart
    {
        public static void Main(string[] args)
        {
            HttpServer server = new HttpServer(8181, RouteTable.Routes);
            MvcEngine.Run(server);
        }
    }
}
