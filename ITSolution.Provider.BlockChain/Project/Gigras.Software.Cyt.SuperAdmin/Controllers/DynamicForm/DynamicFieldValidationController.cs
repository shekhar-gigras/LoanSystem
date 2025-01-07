using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    [Route("sadmin")]
    [Authorize]
    public class DynamicFieldValidationController : BaseController
    {
        private readonly IDynamicFieldValidationService _dynamicFieldValidationService;
        private readonly IDynamicValidationService _dynamicValidationService;

        public DynamicFieldValidationController(IDynamicFormService dynamicFormService, ICytAdminService cytAdminService, IDynamicFieldValidationService dynamicFieldValidationService,
            IDynamicValidationService dynamicValidationService):base(dynamicFormService, cytAdminService)
        {
            _dynamicFieldValidationService = dynamicFieldValidationService;
            _dynamicValidationService = dynamicValidationService;
        }

        [Route("dynamic-fieldvalidation")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var fieldTypes = await _dynamicFieldValidationService.GetAllAsync();
            return View(fieldTypes);
        }

        [Route("api/getvalidations")]
        [HttpGet]
        public async Task<IActionResult> GetValidations()
        {
            var fieldTypes = await _dynamicFieldValidationService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            return Ok(fieldTypes);
        }

        [Route("dynamic-fieldvalidation-add")]
        [HttpGet]
        public async Task<IActionResult> Add(FieldValidation fieldType)
        {
            ViewBag.ValidationList = await _dynamicValidationService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            return View();
        }

        [Route("dynamic-fieldvalidation-add")]
        [HttpPost]
        public async Task<IActionResult> AddSubmit([FromForm] FieldValidation fieldType)
        {
            await _dynamicFieldValidationService.AddFieldValidationAsync(fieldType);
            return Redirect("/sadmin/dynamic-fieldvalidation");
        }

        [Route("dynamic-fieldvalidation-edit")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.ValidationList = await _dynamicValidationService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            var obj = await _dynamicFieldValidationService.GetByIdAsync(id);
            return View(obj);
        }

        [Route("dynamic-fieldvalidation-edit-submit")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditSubmit(int id, [FromForm] FieldValidation fieldType)
        {
            if (id != fieldType.Id)
                return BadRequest("ID mismatch.");

            await _dynamicFieldValidationService.EditFieldValidationAsync(fieldType);
            return Redirect("/sadmin/dynamic-fieldvalidation");
        }

        // Toggle Active/Inactive Status
        [HttpGet("dynamic-fieldvalidation-toggle-status/{id}")]
        public async Task<IActionResult> ToggleFieldTypeStatus(int id, bool isActive)
        {
            int updatedBy = 1;
            await _dynamicFieldValidationService.ToggleFieldValidationStatusAsync(id, isActive, updatedBy);
            return Redirect("/sadmin/dynamic-fieldvalidation");
        }

        [HttpGet("dynamic-fieldvalidation-delete/{id}")]
        public async Task<IActionResult> Delete(int id, bool isDelete)
        {
            int updatedBy = 1;
            await _dynamicFieldValidationService.DeleteFieldValidationAsync(id, isDelete, updatedBy);
            return Redirect("/sadmin/dynamic-fieldvalidation");
        }
    }
}