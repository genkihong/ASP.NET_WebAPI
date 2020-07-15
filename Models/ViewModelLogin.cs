using System.ComponentModel.DataAnnotations;

namespace Rocket.Models
{
  public class ViewModelLogin
  {
    [Display(Name = "Email")]
    [Required(ErrorMessage = "{0}必填")]
    [MaxLength(100)]
    public string Email { get; set; }

    [Display(Name = "密碼")]
    [Required(ErrorMessage = "{0}必填")]
    [DataType(DataType.Password)]
    [StringLength(6, ErrorMessage = "{0}長度最少要{2}個字", MinimumLength = 6)]
    public string Password { get; set; }
  }
}