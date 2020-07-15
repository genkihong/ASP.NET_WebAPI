using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocket.Models
{
  public class ProductImage
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }

    [Display(Name = "圖片檔名")]
    [MaxLength(100)]
    public string Image { get; set; }

    [Display(Name = "建立日期")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreateDate { get; set; }

    //[ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
  }
}