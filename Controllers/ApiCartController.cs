using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Rocket.Filter;
using Rocket.Models;

namespace Rocket.Controllers
{
  [EnableCors(origins: "http://localhost:8080", headers: "*", methods: "*")]
  public class ApiCartController : ApiController
  {
    private FirstModel db = new FirstModel();

    #region Carts
    //private List<Cart> Carts;

    //public ApiCartController()
    //{
    //  Carts = new List<Cart>();
    //}
    #endregion

    #region CartList
    //List<ViewModelCart> CartList
    //{
    //  get
    //  {
    //    if (HttpContext.Current.Session["Carts"] == null)
    //    {
    //      HttpContext.Current.Session["Carts"] = new List<ViewModelCart>();
    //    }
    //    return (List<ViewModelCart>)HttpContext.Current.Session["Carts"];
    //  }
    //  set { HttpContext.Current.Session["Carts"] = value; }
    //}
    #endregion

    //取得購物車
    [Route("api/cart")]
    [JwtAuthFilter]
    public IHttpActionResult GetCart()
    {
      #region Session
      //if (HttpContext.Current.Session["CartsCounter"] == null)
      //{
      //  return NotFound();
      //}

      //Carts = (List<Cart>)HttpContext.Current.Session["Carts"];
      //Carts = HttpContext.Current.Session["Carts"] as List<Cart>;
      #endregion

      //if (!db.Carts.Any())
      //{
      //  return Request.CreateResponse(HttpStatusCode.NotFound, new
      //  {
      //    result = false,
      //    message = "購物車內無商品喔!"
      //  });
      //}

      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);

      //買家購物車
      var carts = db.Carts
        .Where(cart => cart.BuyerId == id)
        .Select(cart => new
        {
          cart.Id,
          cart.BuyerId,
          cart.SellerId,
          cart.Seller,
          Product = new
          {
            cart.ProductId,
            cart.Name,
            Category = cart.Category.ToString(),
            Status = cart.Status.ToString(),
            cart.Quantity,
            cart.Price,
            cart.Total,
            cart.Deposit,
            cart.Store,
            cart.StartDate,
            cart.EndDate,
            cart.Period,
            cart.ImageUrl
          }
        });

      return Ok(new
      {
        result = true,
        carts
      });
    }

    //新增購物車
    [Route("api/cart")]
    [JwtAuthFilter]
    public IHttpActionResult PostCart(ViewModelCart viewModelCart)
    {
      var product = db.Products.Find(viewModelCart.ProductId);

      if (product == null)
      {
        return NotFound();
      }

      //買家
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);

      #region Session
      //if (HttpContext.Current.Session["CartsCounter"] != null)
      //{
      //  Carts = HttpContext.Current.Session["Carts"] as List<Cart>;//轉型為 List
      //}
      #endregion

      //var currentCart = db.Carts.FirstOrDefault(c => c.BuyerId == id && c.ProductId == viewModelCart.ProductId);
      bool isCart = db.Carts
        .Where(c => c.BuyerId == id)
        .Any(c => c.ProductId == viewModelCart.ProductId);

      if (isCart)//購物車有相同的商品
      {
        //Cart cart = new Cart();
        //找出購物車內同一個商品
        Cart cart = db.Carts.First(c => c.BuyerId == id && c.ProductId == viewModelCart.ProductId);
        cart.Quantity += 1;
        cart.Total = cart.Price * cart.Quantity * Convert.ToInt32(cart.Period);
      }
      else//新增商品至購物車
      {
        //賣家
        var seller = db.Members.FirstOrDefault(m => m.Id == product.MemberId);

        var firstImage = db.ProductImages
          .OrderByDescending(pImg => pImg.CreateDate)
          .FirstOrDefault(pImg => pImg.ProductId == viewModelCart.ProductId);

        string productImage = firstImage != null ? firstImage.Image : "";

        db.Carts.Add(new Cart
        {
          ProductId = viewModelCart.ProductId,
          Quantity = viewModelCart.Quantity,
          BuyerId = id,
          SellerId = seller.Id,
          Seller = seller.Name,
          Name = product.Name,
          Price = product.Price,
          Total = product.Price * viewModelCart.Quantity * Convert.ToInt32(viewModelCart.Period),
          Deposit = product.Deposit,
          Store = product.Store,
          Category = product.Category,
          Status = product.Status,
          ImageUrl = productImage,
          Period = viewModelCart.Period,
          StartDate = viewModelCart.StartDate,
          EndDate = viewModelCart.EndDate
        });
      }
      db.SaveChanges();

      //HttpContext.Current.Session["CartsCounter"] = Carts.Count;
      //HttpContext.Current.Session["Carts"] = Carts;

      return Ok(new
      {
        result = true,
        message = "已加入購物車",
        carts = db.Carts
          .Where(cart => cart.BuyerId == id)
          .Select(cart => new
          {
            cart.Id,
            cart.BuyerId,
            cart.SellerId,
            cart.Seller,
            Product = new
            {
              cart.ProductId,
              cart.Name,
              Category = cart.Category.ToString(),
              Status = cart.Status.ToString(),
              cart.Quantity,
              cart.Price,
              cart.Total,
              cart.Deposit,
              cart.Store,
              cart.StartDate,
              cart.EndDate,
              cart.Period,
              cart.ImageUrl
            }
          })
      });
    }

    //刪除購物車
    [Route("api/cart/{CartId}")]
    [JwtAuthFilter]
    public IHttpActionResult DeleteCart(int CartId)
    {
      #region Session
      //if (HttpContext.Current.Session["CartsCounter"] == null)
      //{
      //  return NotFound();
      //}
      //Carts = HttpContext.Current.Session["Carts"] as List<Cart>;
      #endregion

      var currentCart = db.Carts.FirstOrDefault(c => c.Id == CartId);

      if (currentCart == null)
      {
        return NotFound();
      }

      db.Carts.Remove(currentCart);
      db.SaveChanges();

      return Ok(new
      {
        result = true,
        message = "已刪除",
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
  }
}
