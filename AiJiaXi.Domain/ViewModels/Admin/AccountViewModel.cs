using System.ComponentModel.DataAnnotations;

namespace AiJiaXi.Domain.ViewModels.Admin
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [StringLength(5)]
        public string VerificationCode { get; set; }
    }
}