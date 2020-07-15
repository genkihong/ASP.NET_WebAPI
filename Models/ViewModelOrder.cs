using System.ComponentModel.DataAnnotations;

namespace Rocket.Models
{
  public class ViewModelOrder
  {
    [Key]
    public int Id { get; set; }

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

    [Display(Name = "賣家")]
    [MaxLength(50)]
    public string Seller { get; set; }

    [Display(Name = "下訂日期")]
    public string OrderDate { get; set; }

    //[Display(Name = "買家")]
    //[MaxLength(50)]
    //public string Buyer { get; set; }

    //[Display(Name = "押金")]
    //public int? Deposit { get; set; }

    //[Display(Name = "全部總租金")]
    //public int? FinalTotal { get; set; }

    //[Display(Name = "數量")]
    //public int? Quantity { get; set; }

    //[Display(Name = "歸還日期")]
    //public string ReturnDate { get; set; }

  }
}