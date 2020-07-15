using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocket.Models
{
  [DisplayName("會員資料")]
  public class Member
  {
    [Key]
    public int Id { get; set; }

    [Display(Name = "姓名")]
    [Required(ErrorMessage = "{0}必填")]
    [MaxLength(50)]
    public string Name { get; set; }

    [Display(Name = "密碼")]
    [Required(ErrorMessage = "{0}必填")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "{0}長度最少要{2}個字")]
    public string Password { get; set; }

    [Display(Name = "密碼鹽")]
    public string PasswordSalt { get; set; }

    [Display(Name = "Email帳號")]
    [Required(ErrorMessage = "{0}必填")]
    [MaxLength(50)]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "格式錯誤")]
    public string Email { get; set; }

    [Display(Name = "身份證字號")]
    [Required(ErrorMessage = "{0}必填")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "{0}長度最少要{2}個字")]//{0} = Property Name,{1} = Max Length,{2} = Min Length
    public string Identity { get; set; }

    [Display(Name = "手機")]
    [Required(ErrorMessage = "{0}必填")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "{0}長度最少要{2}個字")]
    [DataType(DataType.PhoneNumber)]
    public string Phone { get; set; }

    [Display(Name = "住址")]
    [Required(ErrorMessage = "{0}必填")]
    [MaxLength(100)]
    public string Address { get; set; }

    [Display(Name = "個人圖片")]
    [MaxLength(100)]
    public string Photo { get; set; }

    [Display(Name = "賣場圖片")]
    [MaxLength(100)]
    public string StoreImage { get; set; }

    [Display(Name = "賣場描述")]
    public string StoreDescription { get; set; }

    [Display(Name = "回應時間")]
    [MaxLength(10)]
    public string Reply { get; set; }

    [Display(Name = "Line")]
    public string Line { get; set; }

    [Display(Name = "建立日期")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreateDate { get; set; }
    
    public virtual ICollection<Product> Products { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
    
    //[Display(Name = "會員啟用驗證碼")]
    //[MaxLength(36)]
    //[Description("AuthCode 等於 null 代表此會員已經通過 Email 有效驗證")]
    //public string AuthCode { get; set; }
  }
}