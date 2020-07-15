using Jose;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Rocket.Filter
{
  public class JwtAuthFilter : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      string secret = ConfigurationManager.AppSettings["secret"];//加解密的key,如果不一樣會無法成功解密
      var request = actionContext.Request;

      if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != "Bearer")
      {
        setErrorResponse(actionContext, "驗證錯誤");
        //throw new Exception("Lost Token");
      }
      else
      {
        try
        {
          //解密後會回傳Json格式的物件(即加密前的資料)
          var jwtObject = JWT.Decode<Dictionary<string, object>>(
            request.Headers.Authorization.Parameter,
            Encoding.UTF8.GetBytes(secret),
            JwsAlgorithm.HS256);
          
          if (IsTokenExpired(jwtObject["Exp"].ToString()))
          {
            setErrorResponse(actionContext, "Token 時效已過期!");
            //throw new Exception("Token Expired");
          }
        }
        catch (Exception ex)
        {
          setErrorResponse(actionContext, ex.Message);
        }
      }
      base.OnActionExecuting(actionContext);
    }
    /// <summary>
    /// 錯誤訊息
    /// </summary>
    /// <param name="actionContext"></param>
    /// <param name="message"></param>
    private void setErrorResponse(HttpActionContext actionContext, string message)
    {
      actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
    }

    /// <summary>
    /// 驗證token時效
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>bool</returns>
    public bool IsTokenExpired(string dateTime)
    {
      return Convert.ToDateTime(dateTime) < DateTime.Now;
    }

    //Login不需要驗證因為還沒有token
    public bool WithoutVerifyToken(string requestUri)
    {
      if (requestUri.EndsWith("/login")) return true;
      return false;
    }
  }
}