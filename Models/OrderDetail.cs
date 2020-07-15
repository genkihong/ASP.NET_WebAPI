using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocket.Models
{
  public class OrderDetail
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    [Display(Name = "商品名稱")]
    public string ProductName { get; set; }

    [Display(Name = "賣家Id")]
    public int SellerId { get; set; }

    [Display(Name = "賣家")]
    [MaxLength(50)]
    public string Seller { get; set; }

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

    [Display(Name = "買家租借天數")]
    public string Period { get; set; }

    [Display(Name = "租借時間-開始")]
    public string StartDate { get; set; }

    [Display(Name = "租借時間-結束")]
    public string EndDate { get; set; }

    [Display(Name = "星星數")]
    public Star Star { get; set; }

    [Display(Name = "評論")]
    public string Content { get; set; }

    [Display(Name = "建立日期")]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreateDate { get; set; }

    //[ForeignKey("OrderId")]
    public virtual Order Order { get; set; }
  }
}