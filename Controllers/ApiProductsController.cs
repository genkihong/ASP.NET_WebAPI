using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Rocket.Filter;
using Rocket.Models;

namespace Rocket.Controllers
{
  [EnableCors(origins: "http://localhost:8080", headers: "*", methods: "*")]
  public class ApiProductsController : ApiController
  {
    private FirstModel db = new FirstModel();

    /// <summary>
    /// 所有商品(會員)
    /// </summary>
    /// <returns></returns>
    [Route("api/product/admin/all")]
    [JwtAuthFilter]
    public IHttpActionResult GetAdminProducts()
    {
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);
      Member member = db.Members.Find(id);

      var products = db.Products
        .Where(p => p.MemberId == member.Id)
        .Select(p => new
        {
          p.Id,
          Member = member.Name,
          p.Name,
          p.OriginPrice,
          p.Price,
          p.Deposit,
          p.Quantity,
          p.City,
          p.Zone,
          p.Store,
          Status = p.Status.ToString(),
          Category = p.Category.ToString(),
          p.Period,
          p.Description,
          p.PublishDate,
          Images = p.ProductImages
            .Where(pImg => pImg.ProductId == p.Id)
            .Select(pImg => pImg.Image)
        });

      return Ok(new
      {
        result = true,
        products
      });
    }

    /// <summary>
    /// 所有商品(免登入)
    /// </summary>
    /// <returns></returns>
    [Route("api/product/all")]
    public IHttpActionResult GetProducts()
    {
      var products = db.Products
        .Select(p => new
        {
          p.Id,
          MemberName = db.Members.FirstOrDefault(m => m.Id == p.MemberId).Name,
          p.Name,
          p.OriginPrice,
          p.Price,
          p.Deposit,
          p.Quantity,
          p.City,
          p.Zone,
          p.Store,
          Status = p.Status.ToString(),
          Category = p.Category.ToString(),
          p.Period,
          p.Description,
          p.PublishDate,
          Images = p.ProductImages
            .Where(pImg => pImg.ProductId == p.Id)
            .Select(pImg => pImg.Image)
        });

      return Ok(new
      {
        result = true,
        products
      });
    }

    /// <summary>
    /// 單一商品(免登入)
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns></returns>
    [Route("api/product/{productId}")]
    public IHttpActionResult GetProduct(int productId)
    {
      Product product = db.Products.Find(productId);

      if (product == null)
      {
        return NotFound();
      }

      //賣家
      var seller = db.Members.FirstOrDefault(m => m.Id == product.MemberId);
      var member = new
      {
        seller.Id,
        seller.Name,
        seller.Line,
        seller.Reply,
        seller.StoreDescription,
        seller.Photo,
        seller.StoreImage,
        Category = db.Products
          .Where(p => p.MemberId == seller.Id)
          .Select(p => p.Category.ToString())
          .Distinct()
      };

      bool isOroder = db.OrderDetails.Any(od => od.ProductId == productId);

      if (!isOroder)
      {
        return Ok(new
        {
          result = true,
          average = 0,
          evaluation = new object[0],
          member ,
          product = new
          {
            product.Id,
            product.Name,
            product.OriginPrice,
            product.Price,
            product.Deposit,
            product.Quantity,
            product.City,
            product.Zone,
            product.Store,
            Status = product.Status.ToString(),
            Category = product.Category.ToString(),
            product.Period,
            product.Description,
            product.PublishDate,
            Images = product.ProductImages
              .Where(pImg => pImg.ProductId == product.Id)
              .Select(pImg => pImg.Image)
          }
        });
      }

      //評論
      var evaluation = db.Orders
        .Join(db.OrderDetails,
          o => o.Id,
          od => od.OrderId,
          (o, od) => new { o, od })
        .Where(order => order.od.ProductId == productId)
        .Select(order => new
        {
          Buyer = order.o.Name,
          order.od.Star,
          order.od.Content,
          order.od.CreateDate,
          db.Members.FirstOrDefault(m => m.Id == order.o.MemberId).Photo
        });

      #region 評論 JOIN 寫法二(Where在後面)
      //var evaluation = db.Orders
      //  .Join(db.OrderDetails,
      //    o => o.Id,
      //    od => od.OrderId,
      //    (o, od) => new
      //    {
      //      od.ProductId,
      //      Buyer = o.Name,
      //      od.Star,
      //      od.Content,
      //      od.CreateDate,
      //      db.Members.FirstOrDefault(m => m.Id == o.MemberId).Photo
      //    }).Where(temp => temp.ProductId == productId);
      #endregion

      //平均評價
      double average = db.OrderDetails
        .Where(od => od.ProductId == productId)
        .Average(od => (int)od.Star);

      return Ok(new
      {
        result = true,
        average,
        evaluation,
        member,
        product = new
        {
          product.Id,
          product.Name,
          product.OriginPrice,
          product.Price,
          product.Deposit,
          product.Quantity,
          product.City,
          product.Zone,
          product.Store,
          Status = product.Status.ToString(),
          Category = product.Category.ToString(),
          product.Period,
          product.Description,
          product.PublishDate,
          Images = product.ProductImages
            .Where(pImg => pImg.ProductId == product.Id)
            .Select(pImg => pImg.Image)
        }
        #region Evaluation
        //Evaluation = db.OrderDetails
        //.Where(od => od.ProductId == product.Id)
        //.Select(od => new
        //{
        ////BuyerId = db.Orders.FirstOrDefault(o => o.Id == od.OrderId).Id,
        //  Buyer = db.Orders.FirstOrDefault(o => o.Id == od.OrderId).Name,
        //  od.Star,
        //  od.Content,
        //  od.CreateDate,
        //  db.Members
        //  .FirstOrDefault(m => m.Id == m.Orders.FirstOrDefault(o => o.Id == od.OrderId).MemberId)
        ////.FirstOrDefault(m => m.Id == db.Orders.FirstOrDefault(o => o.Id == od.OrderId).MemberId)
        //  .Photo
        //}),
        #endregion
      });
    }

    /// <summary>
    /// 修改商品
    /// </summary>
    /// <param name="patchProduct">修改商品</param>
    /// <returns></returns>
    [Route("api/product/{Id}")]
    [JwtAuthFilter]
    public IHttpActionResult PatchProduct(PatchProduct patchProduct)
    {
      Product product = db.Products.Find(patchProduct.Id);

      if (product == null)
      {
        return NotFound();
      }

      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);

      if (product.MemberId != id)
      {
        return BadRequest();
      }

      patchProduct.Patch(product);

      db.Entry(product).State = EntityState.Modified;
      db.SaveChanges();

      return Ok(new
      {
        result = true,
        message = "已更新商品"
      });
    }

    /// <summary>
    /// 新增商品
    /// </summary>
    /// <param name="product">新增商品</param>
    /// <returns></returns>
    [Route("api/product/add")]
    [JwtAuthFilter]
    public IHttpActionResult PostProduct(Product product)
    {
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);
      product.MemberId = id;
      db.Products.Add(product);
      db.SaveChanges();

      return Ok(new
      {
        result = true,
        message = "已建立商品",
        product
      });
    }

    /// <summary>
    /// 上傳商品圖片
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns></returns>
    [Route("api/product/upload/{productId}")]
    [JwtAuthFilter]
    public IHttpActionResult PostUploadProductImage(int productId)
    {
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

        ProductImage productImage = new ProductImage()
        {
          ProductId = productId,
          Image = imageUrl.ToString()
        };

        db.ProductImages.Add(productImage);
        db.SaveChanges();

        return Ok(new
        {
          result = true,
          message = "已上傳商品圖片",
          imageUrl
        });
      }

      return Ok(new
      {
        result = false,
        message = "無法上傳圖片!"
      });
    }

    /// <summary>
    /// 刪除商品
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns></returns>
    [Route("api/product/delete/{productId}")]
    [JwtAuthFilter]
    public IHttpActionResult DeleteProduct(int productId)
    {
      Product product = db.Products.Find(productId);
      if (product == null)
      {
        return NotFound();
      }

      db.Products.Remove(product);
      db.SaveChanges();

      return Ok(new
      {
        result = true,
        message = "已刪除商品"
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

    private bool ProductExists(int id)
    {
      return db.Products.Count(e => e.Id == id) > 0;
    }

    #region 修改商品
    //[Route("update/{productId}")]
    //[JwtAuthFilter]
    //public IHttpActionResult PutProduct(int productId, Product product)
    //{
    //  if (productId != product.Id)
    //  {
    //    return BadRequest();
    //  }

    //  string token = Request.Headers.Authorization.Parameter;
    //  int id = Utility.GetParameter(token);

    //  product.MemberId = id;
    //  db.Entry(product).State = EntityState.Modified;

    //  try
    //  {
    //    db.SaveChanges();
    //  }
    //  catch (DbUpdateConcurrencyException)
    //  {
    //    if (!ProductExists(productId))
    //    {
    //      return NotFound();
    //    }
    //    throw;
    //  }

    //  return Ok(new
    //  {
    //    result = true,
    //    message = "已更新商品資料"
    //  });
    //}
    #endregion
  }
}