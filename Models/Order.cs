using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocket.Models
{
  public class Order
  {
    [Key]
    public int Id { get; set; }

    [Display(Name = "買家Id")]
    //[ForeignKey("Member")]
    public int MemberId { get; set; }

    [Display(Name = "買家姓名")]
    [Required(ErrorMessage = "{0}必填")]
    [MaxLength(50)]
    public string Name { get; set; }

    [Display(Name = "買家手機")]
    [Required(ErrorMessage = "{0}必填")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "{0}長度最少要{2}個字")]
    public string Phone { get; set; }

    [Display(Name = "買家Email帳號")]
    [Required(ErrorMessage = "{0}必填")]
    [MaxLength(50)]
    public string Email { get; set; }

    [Display(Name = "訂單狀態")]
    public OrderStatus? Status { get; set; }

    [Display(Name = "全部商品總租金")]
    public int? FinalTotal { get; set; }

    [Display(Name = "全部商品總押金")]
    public int? FinalDeposit { get; set; }

    [Display(Name = "下訂日期")]
    public string OrderDate { get; set; }

    [Display(Name = "建立日期")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreateDate { get; set; }

    [ForeignKey("MemberId")]
    public virtual Member Member { get; set; }
    public virtual ICollection<OrderDetail> OrderDetailItems { get; set; }
  }
}