using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace Rocket
{
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
        new Newtonsoft.Json.Converters.IsoDateTimeConverter()
        {
          DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
        }
      );
      GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
    }

    protected void Application_PostAuthorizeRequest()
    {
      if (IsWebApiRequest())
      {
        HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
      }
    }

    private bool IsWebApiRequest()
    {
      return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
    }
  }
}
