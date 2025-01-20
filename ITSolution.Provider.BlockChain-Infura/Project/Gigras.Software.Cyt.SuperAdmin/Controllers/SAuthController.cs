using DNTCaptcha.Core;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Cyt.ViewModel;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.General.Helper;
using Gigras.Software.General.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers
{
    public class SAuthController : Controller
    {
        private readonly IDNTCaptchaValidatorService _validatorService;
        private readonly DNTCaptchaOptions _captchaOptions;
        private readonly ICytAdminService _cytAdminService;
        private readonly ILogger<SAuthController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public SAuthController(ILogger<SAuthController> logger,
            IDNTCaptchaValidatorService validatorService, IOptions<DNTCaptchaOptions> options,
            ICytAdminService cytAdminService, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
            )
        {
            _logger = logger;
            _validatorService = validatorService;
            _cytAdminService = cytAdminService;
            _webHostEnvironment = webHostEnvironment;
            _captchaOptions = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
            _configuration = configuration;
        }

        public IActionResult Login(string returnUrl = "/")
        {
            try
            {
                _logger.LogInformation("Enter Login Action");
                if (User.Identity.IsAuthenticated)
                {
                    return Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var field = ModelState[key];
                    if (field.Errors.Count > 0)
                    {
                        foreach (var error in field.Errors)
                        {
                            Console.WriteLine($"Error on field {key}: {error.ErrorMessage}");
                        }
                    }
                }
                return View(model);
            }
            if (!_validatorService.HasRequestValidCaptchaEntry())
            {
                this.ModelState.AddModelError(_captchaOptions.CaptchaComponent.CaptchaInputName, "Please enter valid captcha.");
                return View(model);
            }
            var user = await _cytAdminService.FindByUsernameAndPasswordAsync(model.Username, model.Password);
            await _cytAdminService.SignInUserAsync(user);

            // If the return URL is empty or invalid, redirect to the default page (Home/Index)
            if (string.IsNullOrEmpty(ReturnUrl) || !Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect("/sadmin");
            }

            // Redirect to the ReturnUrl if it's valid
            return Redirect(ReturnUrl);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ITAdmin model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var field = ModelState[key];
                    if (field.Errors.Count > 0)
                    {
                        foreach (var error in field.Errors)
                        {
                            Console.WriteLine($"Error on field {key}: {error.ErrorMessage}");
                        }
                    }
                }
                return View(model);
            }

            var obj = await _cytAdminService.GetAllAsync(a => a.Email == model.Email || a.UserName == model.UserName);

            // Check if Email or Username already exists
            if (obj != null && obj.Count() > 0)
            {
                ModelState.AddModelError("Email", "This email/username is already registered.");
                return View(model);
            }

            // Save data to database
            model.IsActive = false;
            model.Phone = model.Phone;
            model.CreatedDate = DateTime.Now;
            model.Password = PasswordHelper.HashPassword(model.Password!);
            await _cytAdminService.AddAsync(model);

            var verificationToken = PasswordHelper.GenerateVerificationToken();
            var verificationLink = Url.Action("VerifyEmail", "SAuth", new { token = verificationToken, email = model.Email }, Request.Scheme);

            string rootpath = _webHostEnvironment.ContentRootPath + "//wwwroot";
            string template = await EmailHelper.ReadEmailTemplate(rootpath, "RegisterUser.html", verificationLink, "", "Registration Verification Link");
            await EmailHelper.SendEmailAsync(_configuration, model.Name!, model.Email!, "Registration Verification Link", template);

            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Login", "SAuth");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid verification link.");
            }

            var user = await _cytAdminService.GetAllAsync(u => u.Email == email);
            if (user == null || user.Count() == 0)
            {
                return NotFound("User not found.");
            }

            // Here, you can compare the token from the URL with a stored token
            // and mark the email as verified in the database

            // For now, we'll just mark the user as verified
            var objuser = await _cytAdminService.GetByIdAsync(user!.FirstOrDefault()!.Id);

            objuser.IsActive = true; // Assuming you have an IsActive field for verification
            await _cytAdminService.UpdateAsync(objuser);

            return RedirectToAction("Login", "SAuth");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ITAdmin model)
        {
            try
            {
                // Simulate database lookup for user
                var user = await _cytAdminService.GetAllAsync(u => u.Email == model.Email);
                if (user == null || user!.Count() == 0)
                {
                    ModelState.AddModelError(string.Empty, "No user found with this email.");
                    return View(model);
                }

                var objUser = await _cytAdminService.GetByIdAsync(user!.FirstOrDefault()!.Id);
                // Generate a unique reset token (in real cases, use a secure method)
                var token = Guid.NewGuid().ToString();

                // Store the token and expiration time in the database
                objUser.Token = token;
                objUser.TokenExpiry = DateTime.UtcNow.AddHours(1);
                await _cytAdminService.UpdateAsync(objUser);

                // Generate reset link
                var resetLink = Url.Action("ResetPassword", "SAuth", new { token }, Request.Scheme);
                string rootpath = _webHostEnvironment.ContentRootPath + "//wwwroot";
                string template = await EmailHelper.ReadEmailTemplate(rootpath, "ResetPassword.html", resetLink, "", "Forget Password Link");
                await EmailHelper.SendEmailAsync(_configuration, objUser.Name!, objUser.Email!, "Forget Password Link", template);

                ViewBag.Message = "Reset link sent! Please check your email.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error sending email: {ex.Message}");
            }

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate the token and find the user
            var user = await _cytAdminService.GetAllAsync(u => u.Token == model.Token
                && u.TokenExpiry > DateTime.UtcNow);
            if (user == null || user!.Count() == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired token.");
                return View(model);
            }

            var objUser = await _cytAdminService.GetByIdAsync(user!.FirstOrDefault()!.Id);
            // Update the password
            objUser.Password = PasswordHelper.HashPassword(model.NewPassword); // Use your existing HashPassword method
            objUser.Token = null;
            objUser.TokenExpiry = null;
            await _cytAdminService.UpdateAsync(objUser);

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> LogOut()
        {
            // Sign out the user and remove authentication cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the home page or login page after logout
            return Redirect("/sadmin");
        }
    }
}