using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Rocket.Filter;
using Rocket.Models;

namespace Rocket.Controllers
{
  [EnableCors(origins: "http://localhost:8080", headers: "*", methods: "*")]
  public class ApiOrdersController : ApiController
  {
    private FirstModel db = new FirstModel();

    //買家所有訂單
    [Route("api/orders/buyer")]
    [JwtAuthFilter]
    public IHttpActionResult GetBuyerOrders()
    {
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);
      bool isBuyer = db.Orders.Any(o => o.MemberId == id);

      if (!isBuyer)
      {
        return Ok(new
        {
          result = false,
          message = "無訂單記錄"
        });
      }

      var orders = db.Orders
        .Where(o => o.MemberId == id)
        .Select(o => new
        {
          o.Id,
          o.FinalTotal,
          o.FinalDeposit,
          o.OrderDate,
          Status = o.Status.ToString(),
          User = new
          {
            o.MemberId,
            o.Name,
            o.Phone,
            o.Email
          },
          Product = o.OrderDetailItems
            .Select(od => new
            {
              OrderDetailId = od.Id,
              od.ProductId,
              od.ProductName,
              od.Price,
              od.SellerId,
              od.Seller,
              od.Quantity,
              od.Deposit,
              od.Total,
              od.Store,
              od.Period,
              od.StartDate,
              od.EndDate
            })
        });

      return Ok(new
      {
        result = true,
        orders
      });
    }

    //賣家所有訂單
    [Route("api/orders/seller")]
    [JwtAuthFilter]
    public IHttpActionResult GetSellerOrders()
    {
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);
      bool isSeller = db.OrderDetails.Any(od => od.SellerId == id);

      if (!isSeller)
      {
        return Ok(new
        {
          result = false,
          message = "無訂單記錄"
        });
      }

      #region Join 寫法一

      var orders = db.Orders
        .Join(db.OrderDetails,
          o => o.Id,
          od => od.OrderId,
          (o, od) => new
          {
            o.Id,
            o.FinalTotal,
            o.FinalDeposit,
            o.OrderDate,
            Status = o.Status.ToString(),
            User = new
            {
              o.MemberId,
              o.Name,
              o.Phone,
              o.Email
            },
            Product = new
            {
              OrderDetailId = od.Id,
              od.ProductId,
              od.ProductName,
              od.Price,
              od.SellerId,
              od.Seller,
              od.Quantity,
              od.Deposit,
              od.Total,
              od.Store,
              od.Period,
              od.StartDate,
              od.EndDate
            }
          }).Where(order => order.Product.SellerId == id);

      #endregion

      #region Join 寫法二

      //var orders = db.Orders
      //  .Join(db.OrderDetails,
      //    o => o.Id,
      //    od => od.OrderId,
      //    (o, od) => new { o, od })
      //  .Where(order => order.od.SellerId == id)
      //  .Select(order => new
      //  {
      //    order.o.Id,
      //    order.o.FinalTotal,
      //    order.o.FinalDeposit,
      //    order.o.OrderDate,
      //    Status = order.o.Status.ToString(),
      //    User = new
      //    {
      //      order.o.MemberId,
      //      order.o.Name,
      //      order.o.Phone,
      //      order.o.Email
      //    },
      //    Product = new
      //    {
      //      order.od.ProductId,
      //      order.od.ProductName,
      //      order.od.SellerId,
      //      order.od.Seller,
      //      order.od.Quantity,
      //      order.od.Deposit,
      //      order.od.Total,
      //      order.od.Store,
      //      order.od.Period,
      //      order.od.StartDate,
      //      order.od.EndDate
      //    }
      //  });

      #endregion

      return Ok(new
      {
        result = true,
        orders
      });
    }

    //單一訂單
    [Route("api/order/{OrderId}")]
    [JwtAuthFilter]
    public IHttpActionResult GetOrder(int OrderId)
    {
      if (db.Orders.Find(OrderId) == null)
      {
        return NotFound();
      }

      var order = db.Orders
        .Where(o => o.Id == OrderId)
        .Select(o => new
        {
          o.Id,
          o.FinalTotal,
          o.FinalDeposit,
          o.OrderDate,
          Status = o.Status.ToString(),
          User = new
          {
            o.MemberId,
            o.Name,
            o.Phone,
            o.Email,
          },
          Product = o.OrderDetailItems
            .Select(od => new
            {
              OrderDetailId = od.Id,
              od.ProductId,
              od.ProductName,
              od.Price,
              od.SellerId,
              od.Seller,
              od.Quantity,
              od.Deposit,
              od.Total,
              od.Store,
              od.Period,
              od.StartDate,
              od.EndDate,
              ImageUrl = db.Members
                .FirstOrDefault(m => m.Id == od.SellerId)
                .Photo
            })
        });

      return Ok(new
      {
        result = true,
        order
      });
    }

    //新增訂單
    [Route("api/order")]
    [JwtAuthFilter]
    public IHttpActionResult PostOrder(ViewModelOrder viewModelOrder)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //買家
      string token = Request.Headers.Authorization.Parameter;
      int id = Utility.GetParameter(token);
      Member member = db.Members.Find(id);

      if (viewModelOrder.Email != member.Email)
      {
        return Ok(new
        {
          result = false,
          message = "Email 與登入帳號不同"
        });
      }
      //判斷購物車內是否有買家商品
      if (!db.Carts.Any(c => c.BuyerId == id))
      {
        return Ok(new
        {
          result = false,
          message = "購物車內無商品!"
        });
      }

      Order order = new Order
      {
        MemberId = id,
        Name = viewModelOrder.Name,
        Phone = viewModelOrder.Phone,
        Email = viewModelOrder.Email,
        OrderDate = viewModelOrder.OrderDate,
        Status = 0
      };

      int? finalTotal = 0;
      int? finalDeposit = 0;
      var carts = db.Carts.Where(c => c.BuyerId == id);

      foreach (var cart in carts)
      {
        finalDeposit += cart.Deposit;
        finalTotal += cart.Total;

        OrderDetail orderDetail = new OrderDetail
        {
          OrderId = order.Id,
          Seller = viewModelOrder.Seller,
          SellerId = cart.SellerId,
          ProductId = cart.ProductId,
          ProductName = cart.Name,
          Price = cart.Price,
          Deposit = cart.Deposit,
          Total = cart.Total,
          Quantity = cart.Quantity,
          Store = cart.Store,
          Period = cart.Period,
          StartDate = cart.StartDate,
          EndDate = cart.EndDate
        };
        db.OrderDetails.Add(orderDetail);
      }

      order.FinalDeposit = finalDeposit;
      order.FinalTotal = finalTotal;

      db.Orders.Add(order);

      if (carts.Any())
      {
        db.Carts.RemoveRange(carts);
      }

      db.SaveChanges();

      return Ok(new
      {
        result = true,
        message = "已新增訂單"
      });
    }

    //更改訂單狀態
    [Route("api/order-status")]
    [JwtAuthFilter]
    public IHttpActionResult PostOrderStatus(ViewModelOrderStatus orderStatus)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var order = db.Orders.FirstOrDefault(o => o.Id == orderStatus.OrderId);

      if (order == null)
      {
        return Ok(new
        {
          result = false,
          message = "無此訂單"
        });
      }

      order.Status = orderStatus.Status;

      db.SaveChanges();

      return Ok(new
      {
        result = true,
        message = "已改更訂單狀態"
      });
    }

    //新增評論
    [Route("api/evaluation")]
    [JwtAuthFilter]
    public IHttpActionResult PostEvaluation(ViewModelStar star)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      #region
      //Order order = db.Orders.FirstOrDefault(o => o.Id == star.OrderId);

      //if (order == null)
      //{
      //  return NotFound();
      //}

      //var orderDetails = db.OrderDetails
      //  .Where(od => od.OrderId == star.OrderId);

      //foreach (var itemDetail in orderDetails)
      //{
      //  Evaluation evaluation = new Evaluation
      //  {
      //    ProductId = itemDetail.ProductId,
      //    OrderId = star.OrderId,
      //    BuyerId = order.MemberId,
      //    Buyer = order.Name,
      //    Star = star.Star,
      //    Content = star.Content
      //  };
      //  db.Evaluations.Add(evaluation);
      //}
      #endregion

      var orderDetail = db.OrderDetails
        .FirstOrDefault(od => od.Id == star.OrderDetailId);

      if (orderDetail == null)
      {
        return NotFound();
      }

      orderDetail.Star = star.Star;
      orderDetail.Content = star.Content;
      orderDetail.CreateDate = DateTime.Now;

      db.SaveChanges();

      return Ok(new
      {
        result = true,
        message = "已新增評論",
        evaluation = new
        {
          orderDetail.ProductId,
          orderDetail.Star,
          orderDetail.Content,
          orderDetail.CreateDate
        }
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