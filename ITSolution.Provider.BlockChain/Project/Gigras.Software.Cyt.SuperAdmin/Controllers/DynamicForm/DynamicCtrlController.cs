using Gigras.Software.Cyt.Services.CytServcies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    [Route("sadmin")]
    [Authorize]
    [Authorize(Roles = "Admin")] // Specify multiple roles here
    public class DynamicCtrlController : Controller
    {
        private readonly IDynamicCtrlService _dynamicCtrlService;

        public DynamicCtrlController(IDynamicCtrlService dynamicCtrlService)
        {
            _dynamicCtrlService = dynamicCtrlService;
        }

        [Route("dynamic-ctrl/list")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var forms = await _dynamicCtrlService.GetAllAsync(x => x.IsActive && !x.IsActive);
            return View(forms);
        }
    }
}