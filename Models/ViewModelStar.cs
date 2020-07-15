using System.ComponentModel.DataAnnotations;

namespace Rocket.Models
{
  public class ViewModelStar
  {
    public int OrderDetailId { get; set; }

    [Display(Name = "星星數")]
    public Star Star { get; set; }

    [Display(Name = "評論")]
    public string Content { get; set; }
  }
}