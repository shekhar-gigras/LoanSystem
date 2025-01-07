using Gigras.Software.Cyt.Services.CytServcies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    [Route("sadmin")]
    [Authorize]
    public class DynamicValidationController : Controller
    {
        private readonly IDynamicValidationService _dynamicValidationService;

        public DynamicValidationController(IDynamicValidationService dynamicValidationService)
        {
            _dynamicValidationService = dynamicValidationService;
        }

        [Route("dynamic-validation/list")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var forms = await _dynamicValidationService.GetAllAsync(x => x.IsActive && !x.IsActive);
            return View(forms);
        }
    }
}