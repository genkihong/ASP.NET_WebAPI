using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using Rocket.Common;

namespace Rocket
{
  public static class WebApiConfig
  {
    public static string UrlPrefix { get { return "api"; } }
    public static string UrlPrefixRelative { get { return "~/api"; } }

    public static void Register(HttpConfiguration config)
    {
      // Allow Cors
      config.EnableCors();
      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional }
      );

      config.ParameterBindingRules.Insert(0, descriptor =>
        typeof(IPatchState).IsAssignableFrom(descriptor.ParameterType)
          ? new PatchBinding(descriptor)
          : null);
    }
  }
}