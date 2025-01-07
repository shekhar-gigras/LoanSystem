using Gigras.Software.Cyt.Services.CytServcies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    [Route("sadmin")]
    [Authorize(Roles = "Admin")]
    public class DynamicBlockChainController : Controller
    {
        private readonly IDynamicBlockChainService _dynamicBlockChainService;

        public DynamicBlockChainController(IDynamicBlockChainService dynamicBlockChainService)
        {
            _dynamicBlockChainService = dynamicBlockChainService;
        }

        [Route("dynamic-blockchain/list")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var forms = await _dynamicBlockChainService.GetAllAsync(x => x.IsActive && !x.IsActive);
            return View(forms);
        }
    }
}