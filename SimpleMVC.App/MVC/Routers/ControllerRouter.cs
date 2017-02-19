namespace SimpleMVC.App.MVC.Routers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using SimpleHttpServer.Enums;
    using SimpleHttpServer.Models;
    using SimpleMVC.App.MVC.Attributes.Methods;
    using SimpleMVC.App.MVC.Controllers;
    using SimpleMVC.App.MVC.Helpers;
    using SimpleMVC.App.MVC.Interfaces;

    public class ControllerRouter : IHandleable
    {
        private IDictionary<string, string> getParams;
        private IDictionary<string, string> postParams;
        private string requestMethod;
        private string controllerName;
        private string actionName;
        private object[] methodParams;

        private string[] controllerActionParams;
        private string[] controllerAction;

        public ControllerRouter()
        {
            this.getParams = new Dictionary<string, string>();
            this.postParams = new Dictionary<string, string>();
        }

        public HttpResponse Handle(HttpRequest request)
        {
            ParseInput(request);


            MethodInfo method = this.GetMethod();
            Controller controller = this.GetController();
            IInvocable actionResult = (IInvocable)method
                .Invoke(controller, this.methodParams);

            string content = actionResult.Invoke();
            HttpResponse response = new HttpResponse()
            {
                StatusCode = ResponseStatusCode.Ok,
                ContentAsUTF8 = content
            };

            return response;
        }

        private void ParseInput(HttpRequest request)
        {
            string uri = WebUtility.UrlDecode(request.Url);
            string queryString = string.Empty;
            if (request.Url.Contains('?'))
            {
                queryString = request.Url.Split('?')[1];
            }

            this.controllerActionParams = uri.Split('?');
            this.controllerAction = controllerActionParams[0].Split(new char[] { '/' },
                StringSplitOptions.RemoveEmptyEntries);

            this.RetrieveGetParams(queryString);
            this.RetrievePostParams(request.Content);
            this.RetrieveMethodName(request);
            this.RetrieveControllerName();
            this.RetrieveActionName();

            MethodInfo method = this.GetMethod();
            if (method==null)
            {
                throw new NotSupportedException("No such method");
            }

            IEnumerable<ParameterInfo> parameters = method.GetParameters();

            this.methodParams = new object[parameters.Count()];
            int index = 0;
            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.ParameterType.IsPrimitive)
                {
                    object value = this.getParams[parameter.Name];
                    this.methodParams[index] = Convert.ChangeType(value, parameter.ParameterType);
                    index++;
                }
                else if (parameter.ParameterType == typeof(HttpRequest))
                {
                    this.methodParams[index] = request;
                    index++;
                }
                else if (parameter.ParameterType == typeof(HttpSession))
                {
                    this.methodParams[index] = request.Session;
                    index++;
                }
                else
                {
                    Type bindingModelType = parameter.ParameterType;
                    object bindingModel = Activator.CreateInstance(bindingModelType);
                    IEnumerable<PropertyInfo> properties = bindingModel.GetType().GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        property.SetValue(bindingModel,
                            Convert.ChangeType(this.postParams[property.Name], property.PropertyType));
                    }

                    this.methodParams[index] = Convert.ChangeType(bindingModel, bindingModelType);
                    index++;
                }
            }
        }
        
        private void RetrieveGetParams(string queryString)
        {
            string[] paramTokens = queryString.Split('&');
            if (paramTokens.Length >= 1)
            {
                foreach (string token in paramTokens)
                {
                    if (token.Contains('='))
                    {
                        string[] innerTokens = token.Split('=');
                        this.getParams.Add(innerTokens[0], innerTokens[1]);
                    }
                }
            }
        }

        private void RetrievePostParams(string requestContent)
        {
            if (requestContent != null)
            {
                requestContent = WebUtility.UrlDecode(requestContent);
                string[] paramTokens = requestContent.Split('&');
                foreach (string token in paramTokens)
                {
                    string[] innerTokens = token.Split('=');
                    this.postParams.Add(innerTokens[0], innerTokens[1]);
                }
            }
        }

        private void RetrieveControllerName()
        {
            this.controllerName = this.controllerAction[this.controllerAction.Length - 2].ToUpperFirst()
                                  + MvcContext.Current.ControllersSuffix;
        }

        private void RetrieveActionName()
        {
            this.actionName = this.controllerAction[this.controllerAction.Length - 1].ToUpperFirst();
        }

        private void RetrieveMethodName(HttpRequest request)
        {
            this.requestMethod = request.Method.ToString();
        }

        private Controller GetController()
        {
            string controllerType = string.Format(
                "{0}.{1}.{2}",
                MvcContext.Current.AssemblyName,
                MvcContext.Current.ControllersFolder,
                this.controllerName);

            Controller controller = (Controller)Activator.CreateInstance(Type.GetType(controllerType));

            return controller;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods()
        {
            return this.GetController()
                .GetType()
                .GetMethods()
                .Where(m => m.Name == this.actionName);
        }

        private MethodInfo GetMethod()
        {
            MethodInfo method = null;
            foreach (MethodInfo methodInfo in this.GetSuitableMethods())
            {
                IEnumerable<Attribute> attributes = methodInfo
                    .GetCustomAttributes()
                    .Where(a => a is HttpMethodAttribute);
                if (!attributes.Any())
                {
                    return methodInfo;
                }

                foreach (HttpMethodAttribute attribute in attributes)
                {
                    if (attribute.IsValid(this.requestMethod))
                    {
                        return methodInfo;
                    }
                }
            }

            return method;
        }
    }
}
