using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.SuperAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers
{
    [Route("sadmin")]
    [Authorize(Roles = "Admin,User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDynamicFormService _dynamicFormService;

        public HomeController(IDynamicFormService dynamicFormService, ILogger<HomeController> logger)
        {
            _dynamicFormService = dynamicFormService;
            _logger = logger;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Forms = await _dynamicFormService.GetAllAsync(x => x.IsActive && !x.IsDelete, y => y.Country!, y => y.State!, y => y.City!);
            return View();
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
    }
}