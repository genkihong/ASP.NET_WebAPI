using System.ComponentModel.DataAnnotations;

namespace Rocket.Models
{
  public class Cart
  {
    [Key]
    public int Id { get; set; }

    [Display(Name = "訂購商品Id")]
    public int ProductId { get; set; }

    [Display(Name = "買家Id")]
    public int BuyerId { get; set; }

    [Display(Name = "賣家Id")]
    public int SellerId { get; set; }

    [Display(Name = "賣家")]
    public string Seller { get; set; }

    [Display(Name = "商品名稱")]
    public string Name { get; set; }

    [Display(Name = "特價")]
    public int? Price { get; set; }

    [Display(Name = "押金")]
    public int? Deposit { get; set; }

    [Display(Name = "總租金")]
    public int? Total { get; set; }

    [Display(Name = "數量")]
    public int? Quantity { get; set; }

    [Display(Name = "門市")]
    public string Store { get; set; }

    [Display(Name = "狀態")]
    public ProductStatus? Status { get; set; }

    [Display(Name = "類別")]
    public ProductCategory? Category { get; set; }

    [Display(Name = "買家租借天數")]
    public string Period { get; set; }

    [Display(Name = "租借時間-開始")]
    public string StartDate { get; set; }

    [Display(Name = "租借時間-結束")]
    public string EndDate { get; set; }

    [Display(Name = "商品圖片")]
    public string ImageUrl { get; set; }
  }
}