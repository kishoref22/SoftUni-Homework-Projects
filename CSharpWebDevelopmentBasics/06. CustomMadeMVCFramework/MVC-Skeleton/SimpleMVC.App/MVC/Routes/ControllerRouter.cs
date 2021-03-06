﻿namespace SimpleMVC.App.MVC.Routes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Tracing;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Attributes.Methods;
    using Controllers;
    using Interfaces;
    using SimpleHttpServer.Enums;
    using SimpleHttpServer.Models;
    using SimpleHttpServer.Utilities;

    public class ControllerRouter : IHandleable
    {
        private IDictionary<string, string> requestParameters;
        private string requestMethodType;
        private string controllerName;
        private string actionName;
        private object[] methodParameters;

        public HttpResponse Handle(HttpRequest request)
        {
            this.requestParameters = new Dictionary<string, string>();
            string requestMethod = null;
            if (request.Method == RequestMethod.GET)
            {

                this.requestParameters = DecodeParams(request.Url);
            }
            else
            {
                this.requestParameters = DecodeParams(request.Content);
            }

            this.requestMethodType = request.Method.ToString();
            this.MapUrlToControllerAndAction(request.Url);

            MethodInfo method = this.GetMethod();
            if (method == null)
            {
                throw new NotSupportedException("Action is not supported");
            }
            IEnumerable<ParameterInfo> parameters = method.GetParameters();
            this.methodParameters = new object[parameters.Count()];
            int index = 0;
            foreach (ParameterInfo pamfo in parameters)

            {
                if (pamfo.ParameterType.IsPrimitive)
                {
                    object value = this.requestParameters[pamfo.Name];
                    this.methodParameters[index] = Convert.ChangeType(value, pamfo.ParameterType);
                    index++;
                }
                else if (pamfo.ParameterType == typeof(HttpRequest))
                {
                    this.methodParameters[index] = pamfo;
                    index++;
                }
                else if (pamfo.ParameterType == typeof(HttpSession))
                {
                    this.methodParameters[index] = request.HttpSession;
                   index++;
                }
                else
                {
                    Type bindingModelType = pamfo.ParameterType;
                    object bindingModel = Activator.CreateInstance(bindingModelType);
                    foreach (var requestParam in this.requestParameters)
                    {
                        PropertyInfo pinfo =
                            bindingModelType.GetProperties()
                                .FirstOrDefault(p => p.Name.ToLower() == requestParam.Key.ToLower());
                        if(pinfo!=null)
                            pinfo.SetValue(bindingModel, Convert.ChangeType(this.requestParameters[pinfo.Name], pinfo.PropertyType));
                    }
                    this.methodParameters[index] = Convert.ChangeType(bindingModel, bindingModelType);
                    index++;
                }
            }
            Controller controller = this.GetController();

            IInvocable actionResult = (IInvocable)method.Invoke(controller, this.methodParameters);
            string content = actionResult.Invoke();
            var response = new HttpResponse();
            if (!string.IsNullOrEmpty(actionResult.Location))
            {
                response.StatusCode = ResponseStatusCode.Found;
                response.Header.OtherParameters.Add("Location", actionResult.Location);
            }
            else
            {
                response.StatusCode = ResponseStatusCode.Ok;
                response.ContentAsUTF8 = content;
            }         
            return response;
        }

        private MethodInfo GetMethod()
        {
            MethodInfo method = null;
            foreach (MethodInfo suitableMethod in this.GetSuitableMethods())
            {
                IEnumerable<Attribute> attributes =
                    suitableMethod.GetCustomAttributes().Where(a => a is HttpMethodAttribute);

                if (!attributes.Any() && this.requestMethodType == "GET")
                {
                    return suitableMethod;
                }
                foreach (HttpMethodAttribute attribute in attributes)
                {
                    if (attribute.isValid(this.requestMethodType))
                    {
                        return suitableMethod;
                    }
                }
            }
            return method;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods()
        {
            return this.GetController()
                .GetType()
                .GetMethods()
                .Where(m => m.Name == this.actionName);
        }

        private Controller GetController()
        {
            var controllerType = string.Format(
                "{0}.{1}.{2}",
                MvcContext.Current.AssemblyName,
                MvcContext.Current.ControllersFolder,
                this.controllerName);
            var controller = (Controller)Activator.CreateInstance(Type.GetType(controllerType));
            return controller;
        }

        private void MapUrlToControllerAndAction(string requestUrl)
        {
            Regex urlMatcher = new Regex("/([\\w\\d]+)\\/([\\w\\d]+)");
            Match match = urlMatcher.Match(requestUrl);
            if (!match.Success)
            {
                throw new InvalidOperationException("Invalid URL, how did you get here?");
            }
            this.controllerName = UppercaseFirstLetter(match.Groups[1].Value) + "Controller";
            this.actionName = UppercaseFirstLetter(match.Groups[2].Value);
        }

        private static string UppercaseFirstLetter(string componentName)
        {
            char[] controllerNameRaw = componentName.ToCharArray();
            char firstUpper = Char.ToUpper(controllerNameRaw[0]);
            string controllerName = firstUpper.ToString();
            for (int i = 1; i < controllerNameRaw.Length; i++)
            {
                controllerName += controllerNameRaw[i];
            }

            return controllerName;
        }

        private Dictionary<string, string> DecodeParams(string incomingParamString)
        {
            Dictionary<string, string> nameValueParams = new Dictionary<string, string>();
            int indexOfQuestionMark = incomingParamString.IndexOf('?');
            if (indexOfQuestionMark == -1 && !incomingParamString.Contains('&'))
            {
                return nameValueParams;
            }
            else
            {
                incomingParamString = incomingParamString.Substring(indexOfQuestionMark + 1);
                this.SplitNameValuePairs(nameValueParams, incomingParamString);
            }
            return nameValueParams;
        }

        private void SplitNameValuePairs(Dictionary<string, string> nameValueParams, string incomingParamString)
        {
            string[] incomingParams = incomingParamString.Split('&');
            foreach (var incomingParam in incomingParams)
            {
                string[] paramsSplitByEqual = incomingParam.Split('=');
                string paramName = paramsSplitByEqual[0];
                string paramValue = paramsSplitByEqual[1];
                nameValueParams.Add(paramName, paramValue);
            }
        }

    }



}
