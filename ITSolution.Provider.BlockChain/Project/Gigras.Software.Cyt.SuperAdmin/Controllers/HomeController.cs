using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Cyt.SuperAdmin.Models;
using Gigras.Software.Cyt.ViewModel;
using Gigras.Software.General.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers
{
    [Route("sadmin")]
    [Authorize(Roles = "Admin,Lender")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDynamicFormService _dynamicFormService;
        private readonly ILoanDetailsService _loanDetailsService;
        private readonly ICytAdminService _cytAdminService;

        public HomeController(IDynamicFormService dynamicFormService, ILogger<HomeController> logger, ILoanDetailsService loanDetailsService, ICytAdminService cytAdminService)
        {
            _dynamicFormService = dynamicFormService;
            _logger = logger;
            _loanDetailsService = loanDetailsService;
            _cytAdminService = cytAdminService;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var data = await _loanDetailsService.GetInfo();
            ViewBag.Forms = await _dynamicFormService.GetAllAsync(x => x.IsActive && !x.IsDelete, y => y.Country!, y => y.State!, y => y.City!);
            return View(data);
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword()
        {
            ViewBag.Forms = await _dynamicFormService.GetAllAsync(x => x.IsActive && !x.IsDelete, y => y.Country!, y => y.State!, y => y.City!);
            var model = new ChangePasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [Route("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "User session expired. Please login again.";
                return RedirectToAction("Login");
            }

            var user = await _cytAdminService.FindByUsernameAsync(username);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Login");
            }

            if (!PasswordHelper.VerifyPassword(model.CurrentPassword, user.Password))
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View(model);
            }

            user.Password = PasswordHelper.HashPassword(model.NewPassword);
            await _cytAdminService.UpdateAsync(user);

            TempData["SuccessMessage"] = "Password changed successfully.";
            ViewBag.Forms = await _dynamicFormService.GetAllAsync(x => x.IsActive && !x.IsDelete, y => y.Country!, y => y.State!, y => y.City!);
            return RedirectToAction("ChangePassword");
        }
    }
}