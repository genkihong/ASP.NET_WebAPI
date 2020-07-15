using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Rocket.Filter;
using Rocket.Models;
using System.Web.Http.Cors;
using System.Text.RegularExpressions;

namespace Rocket.Controllers
{
  [EnableCors(origins: "http://localhost:8080", headers: "*", methods: "*")]
  public class ApiMembersController : ApiController
  {
    private FirstModel db = new FirstModel();
    //註冊
    [Route("register")]
    public IHttpActionResult PostRegister(Member member)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //檢查 Email 是否重複
      var checkEmail = db.Members.FirstOrDefault(m => m.Email == member.Email);

      if (checkEmail != null)
      {
        return Ok(new
        {
          result = false,
          message = "此 Email 已經註冊!"
        });
      }

      member.PasswordSalt = Utility.CreateSalt();//建立鹽
      member.Password = Utility.GenerateHashWithSalt(member.Password, member.PasswordSalt);//密碼+鹽後加密
      //member.CreateDate = DateTime.Now;//會員註冊時間
      //member.AuthCode = Guid.NewGuid().ToString();//會員驗證碼，採用 Guid 當成驗證碼內容，避免有會員使用到重覆的驗證碼

      db.Members.Add(member);
      db.SaveChanges();
      //SendAuthCodeToMember(member);//發送註冊驗證信給會員
      return Ok(new
      {
        result = true,
        message = "註冊成功"
      });
    }

    //登入
    [Route("login")]
    public HttpResponseMessage PostLogin(ViewModelLogin viewModelLogin)
    {
      if (!ModelState.IsValid)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, new
        {
          result = false,
          message = "登入失敗!"
        });
      }

      Member member = ValidateUser(viewModelLogin.Email, viewModelLogin.Password);//檢查會員登入密碼

      if (member == null)
      {
        return Request.CreateResponse(HttpStatusCode.NotFound, new
        {
          result = false,
          message = "帳號或密碼錯誤!"
        });
      }

      string jwtToken = Utility.GenerateToken(member);//產生 Token

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        result = true,
        message = "登入成功",
        id = member.Id,
        token = jwtToken
      });
    }

    //登入驗證
    private Member ValidateUser(string userName, string password)
    {
      Member member = db.Members.SingleOrDefault(o => o.Email == userName);

      //if (member == null || member.AuthCode != null)
      if (member == null)
      {
        //ModelState.AddModelError("", "尚未通過會員驗證，請收信並點擊會員驗證連結!");
        return null;
      }
      string saltPassword = Utility.GenerateHashWithSalt(password, member.PasswordSalt);
      return saltPassword == member.Password ? member : null;
    }

    //重設密碼
    [Route("forget")]
    public IHttpActionResult PostForget([FromBody] string email)
    {
      Member member = db.Members.FirstOrDefault(m => m.Email == email);

      if (member == null)
      {
        return Ok(new
        {
          result = false,
          message = "Email 錯誤"
        });
      }

      SendNewPwdToMember(member);

      return Ok(new
      {
        result = true,
        message = "已寄送 Email"
      });
    }

    /// <summary>
    /// 發送重設密碼信件
    /// </summary>
    /// <param name="member"></param>
    private void SendNewPwdToMember(Member member)
    {
      string template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/RestPassword.html"));
      string newPwd = Membership.GeneratePassword(6, 0);//產生6位數密碼
      newPwd = Regex.Replace(newPwd, @"[^a-zA-Z0-9]", m => "9");//非字母或數字就換成9

      db.Members.Attach(member);//State = Unchanged => db.Entry(member).State = EntityState.Unchanged
      member.Password = Utility.GenerateHashWithSalt(newPwd, member.PasswordSalt); //State = Modified,只有 Password 更新.
      db.SaveChanges();

      template = template.Replace("@Password", newPwd);
      Utility.SendEmail("重設密碼", template, member.Email);
    }

    //所有賣家
    [Route("api/members")]
    //[JwtAuthFilter]
    public IHttpActionResult GetMembers()
    {
      var sellerId = db.Products
        .Select(p => p.MemberId)
        .Distinct();

      var members = db.Members
        .Where(m => sellerId.Contains(m.Id))
        .Select(m => new
        {
          m.Id,
          m.Name,
          m.Photo,
          Category = db.Products
            .Where(p => p.MemberId == m.Id)
            .Select(p => p.Category.ToString())
            .Distinct()
        });

      return Ok(new
      {
        result = true,
        members
      });
    }

    //單一會員
    [Route("api/member/{id}")]
    //[JwtAuthFilter]
    public IHttpActionResult GetMember(int id)
    {
      //string token = Request.Headers.Authorization.Parameter;
      //int id = Utility.GetParameter(token);
      Member member = db.Members.Find(id);

      if (member == null)
      {
        return NotFound();
      }

      return Ok(new
      {
        result = true,
        member = new
        {
          member.Id,
          member.Name,
          member.Line,
          member.Email,
          member.Phone,
          member.Identity,
          member.Address,
          member.StoreDescription,
          member.Reply,
          member.Photo,
          member.StoreImage,
          Category = db.Products
            .Where(p => p.MemberId == member.Id)
            .Select(p => p.Category.ToString())
            .Distinct()
        }
      });

      #region Join

      //var result = db.Members
      //  .Join(
      //    db.Products,
      //    m => m.Id,
      //    p => p.MemberId,
      //    (m, p) => new { m, p })
      //  .Join(
      //    db.ProductImages,
      //    mp => mp.p.Id,
      //    pImg => pImg.ProductId,
      //    (mp, pImg) => new { mp, pImg })
      //  .Where(temp => temp.mp.m.Id == id)
      //  .Select(temp => new
      //  {
      //    temp.mp.m.Name,
      //    temp.mp.m.Email,
      //    temp.mp.m.Phone,
      //    temp.mp.m.Identity,
      //    temp.mp.m.Password,
      //    temp.mp.m.Address,
      //    temp.mp.p.Description,
      //    imgUrl = temp.pImg.Image
      //  });

      #endregion
    }

    //修改會員
    [Route("api/member")]
    [JwtAuthFilter]
    public IHttpActionResult PatchMember(PatchMember patchMember)
    {
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);
      Member member = db.Members.Find(id);

      if (member == null)
      {
        return NotFound();
      }

      patchMember.Password = Utility.GenerateHashWithSalt(patchMember.Password, member.PasswordSalt);

      patchMember.Patch(member);

      db.Entry(member).State = EntityState.Modified;
      db.SaveChanges();

      return Ok(new
      {
        result = true,
        message = "已更新會員"
      });
    }

    //上傳個人圖片
    [Route("api/member/upload/user")]
    [JwtAuthFilter]
    public IHttpActionResult PostUploadImage()
    {
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);
      Member member = db.Members.Find(id);

      var file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

      if (file.ContentType.IndexOf("image") == -1)
      {
        return Ok(new
        {
          result = false,
          message = "非圖片格式!"
        });
      }

      if (file != null && file.ContentLength > 0)
      {
        string fileName = Utility.UploadImage(file);

        //產生圖片連結
        UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url)
        {
          Path = $"/upload/images/{fileName}"
        };
        Uri imageUrl = uriBuilder.Uri;

        member.Photo = imageUrl.ToString();

        db.Entry(member).State = EntityState.Modified;
        db.SaveChanges();

        return Ok(new
        {
          result = true,
          message = "已上傳個人圖片",
          imageUrl
        });
      }

      return Ok(new
      {
        result = false,
        message = "無法上傳圖片!"
      });
    }

    //上傳賣場圖片
    [Route("api/member/upload/store")]
    [JwtAuthFilter]
    public IHttpActionResult PostUploadStoreImage()
    {
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);
      Member member = db.Members.Find(id);

      HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

      if (file.ContentType.IndexOf("image") == -1)
      {
        return Ok(new
        {
          result = false,
          message = "非圖片格式!"
        });
      }

      if (file != null && file.ContentLength > 0)
      {
        string fileName = Utility.UploadImage(file);

        //產生圖片連結
        UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url)
        {
          Path = $"/upload/images/{fileName}"
        };
        Uri imageUrl = uriBuilder.Uri;

        db.Members.Attach(member);
        member.StoreImage = imageUrl.ToString();

        //db.Entry(member).State = EntityState.Modified;
        db.SaveChanges();

        return Ok(new
        {
          result = true,
          message = "已上傳賣場圖片",
          imageUrl
        });
      }

      return Ok(new
      {
        result = false,
        message = "無法上傳圖片!"
      });
    }

    //登出
    [Route("~/logout")]
    public IHttpActionResult PostLogOut()
    {
      return Ok(new
      {
        result = true,
        message = "已登出"
      });
    }

    // DELETE: api/members/5
    [Route("api/member/{id}")]
    [JwtAuthFilter]
    public HttpResponseMessage DeleteCustomer(int id)
    {
      Member member = db.Members.Find(id);

      if (member == null)
      {
        return Request.CreateResponse(HttpStatusCode.NotFound, new
        {
          result = false,
          message = "無此會員資料"
        });
      }

      db.Members.Remove(member);
      db.SaveChanges();

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        result = true,
        message = "已刪除會員資料"
      });
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool MemberExists(int id)
    {
      return db.Members.Count(e => e.Id == id) > 0;
    }

    #region 修改個人資料
    //[Route("api/member/update")]
    //[JwtAuthFilter]
    //public IHttpActionResult PutMember(Member member)
    //{
    //  string token = Request.Headers.Authorization.Parameter;
    //  int id = Utility.GetParameter(token);

    //  if (!ModelState.IsValid)
    //  {
    //    return BadRequest(ModelState);
    //  }

    //  if (id != member.Id)
    //  {
    //    return BadRequest();
    //  }

    //  member.PasswordSalt = Utility.CreateSalt();
    //  member.Password = Utility.GenerateHashWithSalt(member.Password, member.PasswordSalt);

    //  db.Entry(member).State = EntityState.Modified;

    //  try
    //  {
    //    db.SaveChanges();
    //  }
    //  catch (DbUpdateConcurrencyException)
    //  {
    //    if (!MemberExists(id))
    //    {
    //      return NotFound();
    //    }
    //    throw;
    //  }
    //  return Ok(new
    //  {
    //    result = true,
    //    message = "已更新會員資料"
    //  });
    //}
    #endregion

    #region 寄送註冊驗證信
    //private void SendAuthCodeToMember(Member member)
    //{
    //  string template = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/EmailTemplate.html"));
    //  template = template.Replace("@Name", member.Name)
    //    .Replace("@CreateDate", member.CreateDate.ToString());

    //  UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url)//產生驗證碼網址
    //  {
    //    //Path = @Url.Action("ValidateRegister", new { code = member.AuthCode }),
    //    Path = $"ValidateRegister/{member.AuthCode}"
    //  };

    //  Uri authUrl = uriBuilder.Uri;
    //  template = template.Replace("@AuthUrl", authUrl.ToString());
    //  Utility.SendEmail("會員註冊確認信", template, member.Email);
    //}
    #endregion

    #region 驗證會員 Email
    //[Route("~/ValidateRegister/{code}")]
    //[HttpGet]
    //public HttpResponseMessage GetValidateRegister(string code)
    //{
    //  if (string.IsNullOrEmpty(code))
    //  {
    //    return Request.CreateResponse(HttpStatusCode.NotFound);
    //  }

    //  Member member = db.Members.FirstOrDefault(p => p.AuthCode == code);

    //  if (member != null)
    //  {
    //    member.AuthCode = null;
    //    db.SaveChanges();
    //    return Request.CreateResponse(HttpStatusCode.OK, "會員驗證成功");
    //  }
    //  return Request.CreateResponse(HttpStatusCode.NotFound, "查無此會員驗證碼，可能已驗證過了");
    //}
    #endregion

    #region 上傳檔案
    //[Route("upload")]
    //[HttpPost]
    //public async Task<IHttpActionResult> UploadFile()
    //{
    //  // 如果 Content-Type 沒有 multipart/form-data 就回傳 415
    //  if (!Request.Content.IsMimeMultipartContent())
    //  {
    //    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
    //  }

    //  //檔案儲存路徑
    //  var root = HttpContext.Current.Server.MapPath("~/App_Data/Images");

    //  //建立 MultipartFormDataStreamProvider instance
    //  var provider = new MultipartFormDataStreamProvider(root);

    //  try
    //  {
    //    //從 request content 中讀取檔案並以 BodyPart_{GUID} 格式寫至上方定義的路徑中
    //    await Request.Content.ReadAsMultipartAsync(provider);

    //    var uploadResponse = new UploadResponse();
    //    uploadResponse.Description = provider.FormData["description"];//    

    //    // 取得實際檔案內容
    //    foreach (MultipartFileData content in provider.FileData)
    //    {
    //      //實際檔案處理
    //      fileName = content.Headers.ContentDisposition.FileName.Trim('\"');//      
    //      uploadResponse.Names.Add(fileName);//檔名
    //      uploadResponse.FileNames.Add(content.LocalFileName);//實體路徑+BodyPart_{GUID}
    //      uploadResponse.ContentTypes.Add(content.Headers.ContentType.MediaType);
    //    }
    //    //return Ok(uploadResponse);
    //  }
    //  catch (Exception e)
    //  {
    //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
    //    {
    //      Content = new StringContent(e.Message)
    //    });
    //    //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
    //  }
    //}
    #endregion
  }
}