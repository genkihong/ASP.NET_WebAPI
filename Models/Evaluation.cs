using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocket.Models
{
  public class Evaluation
  {
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public int BuyerId { get; set; }

    [Display(Name = "買家")]
    public string Buyer { get; set; }

    [Display(Name = "星星數")]
    public Star Star { get; set; }

    [Display(Name = "評論")]
    public string Content { get; set; }

    [Display(Name = "建立日期")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreateDate { get; set; }
  }
}