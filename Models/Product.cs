using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocket.Models
{
  public class Product
  {
    [Key]
    public int Id { get; set; }

    [Display(Name = "MemberId FK key")]
    public int MemberId { get; set; }

    [Display(Name = "名稱")]
    [MaxLength(50)]
    public string Name { get; set; }

    [Display(Name = "原價")]
    public int? OriginPrice { get; set; }

    [Display(Name = "特價")]
    public int? Price { get; set; }

    [Display(Name = "押金")]
    public int? Deposit { get; set; }

    [Display(Name = "數量")]
    public int? Quantity { get; set; }

    [Display(Name = "城市")]
    [MaxLength(10)]
    public string City { get; set; }

    [Display(Name = "地區")]
    [MaxLength(10)]
    public string Zone { get; set; }

    [Display(Name = "門市")]
    [MaxLength(20)]
    public string Store { get; set; }

    [Display(Name = "狀態")]
    public ProductStatus? Status { get; set; }

    [Display(Name = "類別")]
    public ProductCategory? Category { get; set; }

    [Display(Name = "租借天數")]
    [MaxLength(10)]
    public string Period { get; set; }

    [Display(Name = "內容")]
    public string Description { get; set; }

    [Display(Name = "上架時間")]
    public string PublishDate { get; set; }

    [Display(Name = "建立日期")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreateDate { get; set; }

    [NotMapped]
    public string CategoryList
    {
      get
      {
        switch (Category)
        {
          case ProductCategory.遊戲主機:
            return "遊戲主機";
          case ProductCategory.遊戲配件:
            return "遊戲配件";
          case ProductCategory.遊戲片:
            return "遊戲片";
          default:
            return null;
        }
      }
    }

    [ForeignKey("MemberId")]
    public virtual Member Member { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; }
  }
}