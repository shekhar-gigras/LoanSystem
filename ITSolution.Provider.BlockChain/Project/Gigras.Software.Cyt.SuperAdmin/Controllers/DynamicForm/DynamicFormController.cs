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
    public class DynamicFormController : BaseController
    {
        private readonly IDynamicFormSectionService _dynamicFormSectionService;
        private readonly IDynamicFormFieldService _dynamicFormFieldService;

        public DynamicFormController(IDynamicFormService dynamicFormService, IDynamicFormSectionService dynamicFormSectionService,
            IDynamicFormFieldService dynamicFormFieldService, ICytAdminService cytAdminService) : base(dynamicFormService, cytAdminService)
        {
            _dynamicFormSectionService = dynamicFormSectionService;
            _dynamicFormFieldService = dynamicFormFieldService;
        }

        [Route("dynamic-form")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var forms = await _dynamicFormService.GetAllAsync(null,
                new Expression<Func<Form, object?>>[] { x => x.State!, y => y.Country, y => y.City }
                );
            return View(forms);
        }

        [Route("dynamic-form-add")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(new Form());
        }

        [Route("dynamic-form-add")]
        [HttpPost]
        public async Task<IActionResult> AddSubmit([FromForm] Form form)
        {
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                form.CreatedBy = userIdAsInt;
                form.UpdatedBy = userIdAsInt;
            }
            form.IsActive = true;
            form.IsDelete = false;
            form.CreatedAt = DateTime.Now;
            form.UpdatedAt = DateTime.Now;
            await _dynamicFormService.AddAsync(form);
            return Redirect("/sadmin/dynamic-form");
        }

        [Route("dynamic-form-edit")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var obj = await _dynamicFormService.GetByIdAsync(id);
            return View(obj);
        }

        [Route("dynamic-form-edit-submit")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditSubmit(int id, [FromForm] Form form)
        {
            if (id != form.Id)
                return BadRequest("ID mismatch.");
            var existingForm = await _dynamicFormService.GetByIdAsync(id);

            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                existingForm.UpdatedBy = userIdAsInt;
            }
            existingForm.FormName = form.FormName;
            existingForm.FormDescription = form.FormDescription;
            existingForm.EntityName = form.EntityName;
            existingForm.NavigationGroup = form.NavigationGroup;
            existingForm.CountryId = form.CountryId;
            existingForm.StateId = form.StateId;
            existingForm.CityId = form.CityId;
            existingForm.IsActive = true;
            existingForm.IsDelete = false;
            existingForm.UpdatedAt = DateTime.Now;
            await _dynamicFormService.UpdateAsync(existingForm);
            return Redirect("/sadmin/dynamic-form");
        }

        // Toggle Active/Inactive Status
        [HttpGet("dynamic-form-toggle-status/{id}")]
        public async Task<IActionResult> ToggleFieldTypeStatus(int id, bool isActive)
        {
            var existingForm = await _dynamicFormService.GetByIdAsync(id);
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                existingForm.UpdatedBy = userIdAsInt;
                existingForm.UpdatedAt = DateTime.Now;
            }
            existingForm.IsActive = isActive;
            await _dynamicFormService.UpdateAsync(existingForm);
            return Redirect("/sadmin/dynamic-form");
        }

        [HttpGet("dynamic-form-delete/{id}")]
        public async Task<IActionResult> Delete(int id, bool isDelete)
        {
            var existingForm = await _dynamicFormService.GetByIdAsync(id);
            existingForm.IsDelete = isDelete;
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                existingForm.UpdatedBy = userIdAsInt;
                existingForm.UpdatedAt = DateTime.Now;
            }
            await _dynamicFormService.UpdateAsync(existingForm);
            return Redirect("/sadmin/dynamic-form");
        }
    }
}