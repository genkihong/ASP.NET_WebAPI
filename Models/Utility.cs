using Jose;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Rocket.Models
{
  public class Utility
  {
    #region JWT 驗證
    /// <summary>
    /// JWT 驗證
    /// </summary>
    /// <param name="id"></param>
    /// <returns>token</returns>
    public static string GenerateToken(Member member)
    {
      string secret = ConfigurationManager.AppSettings["secret"];//加解密的key,如果不一樣會無法成功解密
      Dictionary<string, object> payload = new Dictionary<string, object>//payload 需透過token傳遞的資料
      {
        { "Id", member.Id },
        { "Exp", DateTime.Now.AddSeconds(Convert.ToInt32("86400")).ToString() }//Token 時效=現在時間+秒
      };
      var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS256);//產生token
      return token;
    }
    #endregion

    #region GetParameter
    /// <summary>
    /// GetParameter
    /// </summary>
    /// <param name="token"></param>
    /// <returns>id</returns>
    public static int GetParameter(string token)
    {
      string secret = ConfigurationManager.AppSettings["secret"];
      var jwtObject = JWT.Decode<Dictionary<string, object>>(
        token,
        Encoding.UTF8.GetBytes(secret),
        JwsAlgorithm.HS256);
      int id = (int)jwtObject["Id"];
      return id;
    }
    #endregion

    #region "密碼加密"
    public const int DefaultSaltSize = 5;
    /// <summary>
    /// 產生Salt
    /// </summary>
    /// <returns>Salt</returns>
    public static string CreateSalt()
    {
      RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
      byte[] buffer = new byte[DefaultSaltSize];
      rng.GetBytes(buffer);
      return Convert.ToBase64String(buffer);
    }

    ///// <summary>
    ///// 密碼加密
    ///// </summary>
    ///// <param name="password">密碼明碼</param>
    ///// <returns>Hash後密碼</returns>
    //public static string CreateHash(string password)
    //{
    //    string salt = CreateSalt();
    //    string saltAndPassword = String.Concat(password, salt);
    //    string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPassword, "SHA1");
    //    hashedPassword = string.Concat(hashedPassword, salt);
    //    return hashedPassword;
    //}

    /// <summary>
    /// Computes a salted hash of the password and salt provided and returns as a base64 encoded string.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">The salt to use in the hash.</param>
    public static string GenerateHashWithSalt(string password, string salt)
    {
      // merge password and salt together
      string sHashWithSalt = password + salt;
      // convert this merged value to a byte array
      byte[] saltedHashBytes = Encoding.UTF8.GetBytes(sHashWithSalt);
      // use hash algorithm to compute the hash
      HashAlgorithm algorithm = new SHA256Managed();
      // convert merged bytes to a hash as byte array
      byte[] hash = algorithm.ComputeHash(saltedHashBytes);
      // return the has as a base 64 encoded string
      return Convert.ToBase64String(hash);
    }
    #endregion

    #region "將使用者資料寫入cookie,產生AuthenTicket"
    /// <summary>
    /// 將使用者資料寫入cookie,產生AuthenTicket
    /// </summary>
    /// <param name="userData">使用者資料</param>
    /// <param name="userId">UserAccount</param>
    public static void SetAuthenTicket(string userData, string userId)
    {
      //宣告一個驗證票
      FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userId, DateTime.Now, DateTime.Now.AddHours(3), false, userData);
      //加密驗證票
      string encryptedTicket = FormsAuthentication.Encrypt(ticket);
      //建立Cookie
      HttpCookie authenticationcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
      //將Cookie寫入回應

      HttpContext.Current.Response.Cookies.Add(authenticationcookie);
    }
    #endregion

    #region"儲存上傳圖片"
    /// <summary>
    /// 儲存上傳圖片
    /// </summary>
    /// <param name="uploadFile">HttpPostedFile 物件</param>
    /// <returns>儲存檔名</returns>
    public static string UploadImage(HttpPostedFile uploadFile)
    {
      string fileName = Path.GetFileName(uploadFile.FileName);
      //取得副檔名
      string extension = fileName.Split('.')[fileName.Split('.').Length - 1];
      //新檔案名稱
      fileName = $"{DateTime.Now.ToString("yyyyMMddhhmmss")}.{extension}";
      string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/images"), fileName);
      uploadFile.SaveAs(path);
      return fileName;
    }
    #endregion

    #region"儲存上傳圖片"
    /// <summary>
    /// 儲存上傳圖片
    /// </summary>
    /// <param name="upfile">HttpPostedFile 物件</param>
    /// <returns>儲存檔名</returns>
    public static string UploadImage(HttpPostedFileBase upfile)
    {
      //取得副檔名
      string extension = upfile.FileName.Split('.')[upfile.FileName.Split('.').Length - 1];
      //新檔案名稱
      //string fileName = String.Format("{0:yyyyMMddhhmmss}.{1}", DateTime.Now, extension);
      string fileName = $"{DateTime.Now.ToString("yyyyMMddhhmmss")}.{extension}";
      string savedName = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/images"), fileName);
      upfile.SaveAs(savedName);
      return fileName;
    }
    #endregion

    #region 寄信函數
    /// <summary>
    /// 寄信函數
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <param name="receiveMail"></param>
    public static void SendEmail(string subject, string body, string receiveMail)
    {
      // 寄信人Email
      string sendMail = ConfigurationManager.AppSettings["sendMail"].Trim();
      // 收信人Email(多筆用逗號隔開)
      //string receiveMails = ConfigurationManager.AppSettings["receiveMails"].Trim();
      // 寄信smtp server
      string smtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim();
      // 寄信smtp server的Port，預設25
      int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"].Trim());
      // 寄信帳號
      string mailAccount = ConfigurationManager.AppSettings["mailAccount"].Trim();
      // 寄信密碼
      string mailPwd = ConfigurationManager.AppSettings["mailPwd"].Trim();

      MailMessage mail = new MailMessage();
      mail.From = new MailAddress(mailAccount);//設定寄件者Email
      mail.To.Add(receiveMail); //設定收件者Email
      //Mail.Bcc.Add("密件副本的收件者Mail"); //加入密件副本的Mail          
      mail.Subject = subject;
      mail.Body = body; //設定信件內容
      mail.IsBodyHtml = true; //是否使用html格式

      SmtpClient SMTP = new SmtpClient();
      SMTP.Host = smtpServer;
      SMTP.Port = smtpPort;
      SMTP.EnableSsl = true;
      SMTP.Credentials = new NetworkCredential(mailAccount, mailPwd);
      try
      {
        SMTP.Send(mail);
        mail.Dispose(); //釋放資源
      }
      catch (Exception ex)
      {
        ex.ToString();
      }
    }
    #endregion
  }
}