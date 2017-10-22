using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MyAspNetMvcApp.Areas.App.Models
{
    public class AppControllerAction
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ReturnType { get; set; }
        public string Attributes { get; set; }

        public static List<AppControllerAction> GetExamples()
        {
            Assembly asm = Assembly.GetAssembly(typeof(MyAspNetMvcApp.MvcApplication));
            string @namespace = "MyAspNetMvcApp.Areas.Examples.Controllers";
            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(System.Web.Mvc.Controller).IsAssignableFrom(type) && type.Namespace == @namespace)
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any(p => p.ToString() == "HttpGet"))
                    .Select(x => new AppControllerAction { Controller = x.DeclaringType.Name, Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

            var ConActList = new List<AppControllerAction>();

            foreach (var i in controlleractionlist)
            {
                if(!i.Attributes.Contains("HttpPost") && i.Action.ToLower() == "index")
                    ConActList.Add(new AppControllerAction { Controller = i.Controller.Replace("Controller", string.Empty), Action = i.Action, ReturnType = i.ReturnType, Attributes = i.Attributes });
            }

            return ConActList;
        }
    }
}