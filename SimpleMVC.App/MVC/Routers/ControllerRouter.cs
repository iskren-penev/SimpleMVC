namespace SimpleMVC.App.MVC.Routers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SimpleHttpServer.Enums;
    using SimpleHttpServer.Models;
    using SimpleMVC.App.MVC.Attributes.Methods;
    using SimpleMVC.App.MVC.Controllers;
    using SimpleMVC.App.MVC.Interfaces;
    using static System.Type;

    public class ControllerRouter : IHandleable
    {
        private IDictionary<string, string> getParams;
        private IDictionary<string, string> postParams;
        private string requestMethod;
        private string controllerName;
        private string actionName;
        private object[] methodParams;


        public HttpResponse Handle(HttpRequest request)
        {
            ParseInput(request);

            IInvocable actionResult = (IInvocable)this.GetMethod()
                .Invoke(this.GetController(), this.methodParams);

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
            this.getParams = RetrieveGetParams(request.Url);
            this.postParams = RetrievePostParams(request.Content);
            this.requestMethod = request.Method.ToString();
            RetrieveControllerAndActionName(request.Url);

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


        private IDictionary<string, string> RetrieveGetParams(string requestUrl)
        {
            IDictionary<string, string> temp = new Dictionary<string, string>();
            int indexOfQuestion = requestUrl.IndexOf('?');
            if (indexOfQuestion == -1)
            {
                return temp;
            }
            string parametersString = requestUrl.Substring(indexOfQuestion);
            string[] paramTokens = parametersString.Split('&');
            foreach (string token in paramTokens)
            {
                string[] innerTokens = token.Split('=');
                temp.Add(innerTokens[0], innerTokens[1]);
            }

            return temp;
        }

        private IDictionary<string, string> RetrievePostParams(string requestContent)
        {
            IDictionary<string, string> temp = new Dictionary<string, string>();
            if (String.IsNullOrEmpty(requestContent))
            {
                return temp;
            }
            string[] paramTokens = requestContent.Split('&');
            foreach (string token in paramTokens)
            {
                string[] innerTokens = token.Split('=');
                temp.Add(innerTokens[0], innerTokens[1]);
            }

            return temp;
        }

        private void RetrieveControllerAndActionName(string requestUrl)
        {
            int indexOfFirstSlash = requestUrl.IndexOf('/');
            int indexOfSecondSlash = requestUrl.Substring(indexOfFirstSlash + 1).IndexOf('/');
            if (indexOfSecondSlash == -1)
            {
                throw new Exception("Invalid URL");
            }
            int cotrollerNameLength = indexOfSecondSlash - (indexOfFirstSlash + 1);
            string tempControllerName = requestUrl.Substring(indexOfFirstSlash + 1,
                cotrollerNameLength);
            string tempActionName;
            int indexOfQuestion = requestUrl.IndexOf('?');
            if (indexOfQuestion == -1)
            {
                tempActionName = requestUrl.Substring(indexOfSecondSlash + 1);
            }
            else
            {
                int actionNameLength = indexOfQuestion - (indexOfSecondSlash + 1);
                tempActionName = requestUrl.Substring(indexOfSecondSlash + 1, actionNameLength);
            }

            this.controllerName = tempControllerName[0].ToString().ToUpper()
                                 + tempControllerName.Substring(1)
                                 +"Controller";
            this.actionName = tempActionName[0].ToString().ToUpper()
                              + tempActionName.Substring(1);
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
