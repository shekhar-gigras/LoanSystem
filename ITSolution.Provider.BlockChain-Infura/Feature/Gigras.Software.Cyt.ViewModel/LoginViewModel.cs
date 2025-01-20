using System.ComponentModel.DataAnnotations;

namespace Gigras.Software.Cyt.ViewModel
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Username or Email is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Security code is required")]
        public string DNTCaptchaInputText { get; set; }

        public string? ReturnUrl { get; set; }
    }
}