using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    [Route("sadmin")]
    [Authorize]
    public class DynamicFieldTypesController : BaseController
    {
        private readonly IDynamicFieldTypeService _dynamicFieldTypeService;
        private readonly IDynamicFieldOptionService _dynamicFieldOptionService;
        private readonly IDynamicFieldValidationService _dynamicFieldValidationService;
        private readonly IDynamicCtrlService _dynamicCtrlService;
        private readonly IDynamicBlockChainService _dynamicBlockChainService;

        public DynamicFieldTypesController(IDynamicFieldTypeService dynamicFieldTypeService, ICytAdminService cytAdminService,
            IDynamicFieldOptionService dynamicFieldOptionService,
            IDynamicFieldValidationService dynamicFieldValidationService,
            IDynamicCtrlService dynamicCtrlService,
            IDynamicBlockChainService dynamicBlockChainService,
            IDynamicFormService dynamicFormService) : base(dynamicFormService, cytAdminService)
        {
            _dynamicFieldTypeService = dynamicFieldTypeService;
            _dynamicFieldOptionService = dynamicFieldOptionService;
            _dynamicFieldValidationService = dynamicFieldValidationService;
            _dynamicCtrlService = dynamicCtrlService;
            _dynamicBlockChainService = dynamicBlockChainService;
        }

        [Route("dynamic-fieldtype")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var fieldTypes = await _dynamicFieldTypeService.GetAllAsync();
            return View(fieldTypes);
        }

        [Route("dynamic-fieldtype/list")]
        [HttpGet]
        public async Task<IActionResult> GetFieldTypes()
        {
            var fieldTypes = await _dynamicFieldTypeService.GetAllAsync(f => !f.IsDelete && f.IsActive);
            return Json(fieldTypes);
        }

        [Route("dynamic-fieldtype-add")]
        [HttpGet]
        public async Task<IActionResult> Add(FieldType fieldType)
        {
            ViewBag.CtrlList = await _dynamicCtrlService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            ViewBag.Options = await _dynamicFieldOptionService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            ViewBag.Validations = await _dynamicFieldValidationService.GetAllAsync(x => x.IsActive && !x.IsDelete,
                 new Expression<Func<FieldValidation, object?>>[] { x => x.FieldTypeValidaions! });
            ViewBag.BlockChainFields = await _dynamicBlockChainService.GetAllAsync(x => x.IsActive && !x.IsDelete);

            return View();
        }

        [Route("dynamic-fieldtype-add")]
        [HttpPost]
        public async Task<IActionResult> AddSubmit([FromForm] FieldType fieldType)
        {
            var validation = Request.Form["FieldValidations"];
            var validationList = validation.Select(v => Convert.ToInt32(v)).ToList();
            if (validationList != null && validationList.Count > 0)
            {
                fieldType.FieldTypeValidations = new List<FieldTypeValidation>();
                foreach (var item in validationList)
                {
                    fieldType.FieldTypeValidations.Add(new FieldTypeValidation() { FieldValidationId = item });
                }
            }

            await _dynamicFieldTypeService.AddFieldTypeAsync(fieldType);
            return Redirect("/sadmin/dynamic-fieldtype");
        }

        [Route("dynamic-fieldtype-edit")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.CtrlList = await _dynamicCtrlService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            ViewBag.Options = await _dynamicFieldOptionService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            ViewBag.Validations = await _dynamicFieldValidationService.GetAllAsync(x => x.IsActive && !x.IsDelete,
                 new Expression<Func<FieldValidation, object?>>[] { x => x.FieldTypeValidaions!.Where(y => y.IsActive && !y.IsDelete) });
            ViewBag.BlockChainFields = await _dynamicBlockChainService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            var obj = await _dynamicFieldTypeService.GetByIdAsync(id);
            return View(obj);
        }

        [Route("dynamic-fieldtype-edit-submit")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditSubmit(int id, [FromForm] FieldType fieldType)
        {
            if (id != fieldType.Id)
                return BadRequest("ID mismatch.");

            var validation = Request.Form["FieldValidations"];
            var validationList = validation.Select(v => Convert.ToInt32(v)).ToList();
            if (validationList != null && validationList.Count > 0)
            {
                fieldType.FieldTypeValidations = new List<FieldTypeValidation>();
                foreach (var item in validationList)
                {
                    fieldType.FieldTypeValidations.Add(new FieldTypeValidation() { FieldValidationId = item });
                }
            }

            await _dynamicFieldTypeService.EditFieldTypeAsync(fieldType);
            return Redirect("/sadmin/dynamic-fieldtype");
        }

        // Toggle Active/Inactive Status
        [HttpGet("dynamic-fieldtype-toggle-status/{id}")]
        public async Task<IActionResult> ToggleFieldTypeStatus(int id, bool isActive)
        {
            int updatedBy = 1;
            await _dynamicFieldTypeService.ToggleFieldTypeStatusAsync(id, isActive, updatedBy);
            return Redirect("/sadmin/dynamic-fieldtype");
        }

        [HttpGet("dynamic-fieldtype-delete/{id}")]
        public async Task<IActionResult> Delete(int id, bool isDelete)
        {
            int updatedBy = 1;
            await _dynamicFieldTypeService.DeleteFieldTypeAsync(id, isDelete, updatedBy);
            return Redirect("/sadmin/dynamic-fieldtype");
        }
    }
}