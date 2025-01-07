using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    [Route("sadmin")]
    [Authorize]
    public class DynamicFieldOptionController : BaseController
    {
        private readonly IDynamicFieldOptionService _dynamicFieldOptionService;

        public DynamicFieldOptionController(IDynamicFormService dynamicFormService, IDynamicFieldOptionService dynamicFieldOptionService,
            ICytAdminService cytAdminService) : base(dynamicFormService, cytAdminService)
        {
            _dynamicFieldOptionService = dynamicFieldOptionService;
        }

        [Route("dynamic-fieldoption")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var fieldTypes = await _dynamicFieldOptionService.GetAllAsync();
            return View(fieldTypes);
        }

        [Route("api/getoptions")]
        [HttpGet]
        public async Task<IActionResult> GetOptions()
        {
            var fieldTypes = await _dynamicFieldOptionService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            return Ok(fieldTypes);
        }

        [Route("dynamic-fieldoption-add")]
        [HttpGet]
        public async Task<IActionResult> Add(FieldType fieldType)
        {
            GetDbSchema();
            return View();
        }

        [Route("dynamic-fieldoption-add")]
        [HttpPost]
        public async Task<IActionResult> AddSubmit([FromForm] FieldOption fieldType)
        {
            try
            {
                if (fieldType.IsDynamic && string.IsNullOrEmpty(fieldType.SourceTable))
                {
                    GetDbSchema();
                    ViewBag.Error = "If Is Dynamic, then Source Table data is required";
                    return View("~/views/DynamicFieldOption/Add.cshtml"); // Replace "Error" with an actual error view, if available.
                }
                else if (!fieldType.IsDynamic && (fieldType.OptionValue.Count == 0 || fieldType.OptionLabel.Count == 0))
                {
                    GetDbSchema();
                    ViewBag.Error = "If Is not Dynamic, then Option value is required";
                    return View("~/views/DynamicFieldOption/Add.cshtml"); // Replace "Error" with an actual error view, if available.
                }
                var obj = await _dynamicFieldOptionService.GetAllAsync(x => x.OptionName!.ToLower().Trim() == fieldType.OptionName!.ToLower().Trim());
                if (obj.Any())
                {
                    GetDbSchema();
                    ViewBag.Error = "Option Name already exists";
                    return View("~/views/DynamicFieldOption/Add.cshtml"); // Replace "Error" with an actual error view, if available.
                }
                // Save to database
                await _dynamicFieldOptionService.AddFieldOptionAsync(fieldType);

                return Redirect("/sadmin/dynamic-fieldoption");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "An error occurred while saving the data.";
                return View("Error");
            }
        }

        [Route("dynamic-fieldoption-edit")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            GetDbSchema();
            var obj = await _dynamicFieldOptionService.GetByIdAsync(
                 id,
                 include: query => query.Include(x => x.OptionValues) // Eagerly load OptionValues
             );
            obj.OptionValues = obj.OptionValues.Where(cv => cv.IsActive).ToList();
            return View(obj);
        }

        [Route("dynamic-fieldoption-edit-submit")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditSubmit(int id, [FromForm] FieldOption fieldType)
        {
            if (id != fieldType.Id)
                return BadRequest("ID mismatch.");
            if (fieldType.IsDynamic && string.IsNullOrEmpty(fieldType.SourceTable))
            {
                GetDbSchema();
                var objField = await _dynamicFieldOptionService.GetByIdAsync(
                     id,
                     include: query => query.Include(x => x.OptionValues) // Eagerly load OptionValues
                 );
                objField.OptionValues = objField.OptionValues.Where(cv => cv.IsActive).ToList();
                ViewBag.Error = "If Is Dynamic, then Source Table data is required";
                return View("~/views/DynamicFieldOption/Edit.cshtml", objField); // Replace "Error" with an actual error view, if available.
            }
            else if (!fieldType.IsDynamic && (fieldType.OptionValue.Count == 0 || fieldType.OptionLabel.Count == 0))
            {
                GetDbSchema();
                var objField = await _dynamicFieldOptionService.GetByIdAsync(
                   id,
                   include: query => query.Include(x => x.OptionValues) // Eagerly load OptionValues
               );
                objField.OptionValues = objField.OptionValues.Where(cv => cv.IsActive).ToList();
                ViewBag.Error = "If Is not Dynamic, then Option value is required";
                return View("~/views/DynamicFieldOption/Edit.cshtml", objField); // Replace "Error" with an actual error view, if available.
            }
            var obj = await _dynamicFieldOptionService.GetAllAsync(x => x.Id != id && x.OptionName!.ToLower().Trim() == fieldType.OptionName!.ToLower().Trim());
            if (obj.Any())
            {
                GetDbSchema();
                var objField = await _dynamicFieldOptionService.GetByIdAsync(
                   id,
                   include: query => query.Include(x => x.OptionValues) // Eagerly load OptionValues
               );
                objField.OptionValues = objField.OptionValues.Where(cv => cv.IsActive).ToList();
                ViewBag.Error = "Option Name already exists";
                return View("~/views/DynamicFieldOption/Edit.cshtml", objField); // Replace "Error" with an actual error view, if available.
            }
            await _dynamicFieldOptionService.EditFieldOptionAsync(fieldType);
            return Redirect("/sadmin/dynamic-fieldoption");
        }

        // Toggle Active/Inactive Status
        [HttpGet("dynamic-fieldoption-toggle-status/{id}")]
        public async Task<IActionResult> ToggleFieldTypeStatus(int id, bool isActive)
        {
            int updatedBy = 1;
            await _dynamicFieldOptionService.ToggleFieldOptionAsync(id, isActive, updatedBy);
            return Redirect("/sadmin/dynamic-fieldoption");
        }

        [HttpGet("dynamic-fieldoption-delete/{id}")]
        public async Task<IActionResult> Delete(int id, bool isDelete)
        {
            int updatedBy = 1;
            await _dynamicFieldOptionService.DeleteFieldOptionAsync(id, isDelete, updatedBy);
            return Redirect("/sadmin/dynamic-fieldoption");
        }
    }
}