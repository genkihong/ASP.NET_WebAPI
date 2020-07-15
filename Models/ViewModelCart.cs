using System.ComponentModel.DataAnnotations;

namespace Rocket.Models
{
  public class ViewModelCart
  {
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Display(Name = "租借時間-開始")]
    public string StartDate { get; set; }

    [Display(Name = "租借時間-結束")]
    public string EndDate { get; set; }

    [Display(Name = "租借天數")]
    public string Period { get; set; }
  }
}