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
    public class DynamicFormSectionFieldController : BaseController
    {
        private readonly IDynamicFormFieldService _dynamicFormFieldService;

        public DynamicFormSectionFieldController(IDynamicFormService dynamicFormService, IDynamicFormFieldService dynamicFormFieldService,
            ICytAdminService cytAdminService) : base(dynamicFormService, cytAdminService)
        {
            _dynamicFormFieldService = dynamicFormFieldService;
        }

        [Route("dynamic-form-section-field")]
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var forms = await _dynamicFormFieldService.GetAllAsync(x => x.SectionId == id,
                new Expression<Func<FormField, object?>>[] { x => x.FieldType!, y => y.FormSection });
            forms = forms.OrderBy(x => x.FieldOrder).ToList();
            return Ok(forms);
        }

        [Route("dynamic-form-section-field-add-save")]
        [HttpPost]
        public async Task<IActionResult> AddFormFieldSubmit(FormField form)
        {
            var section = await _dynamicFormFieldService.GetAllAsync(x => x.FieldType == form.FieldType && x.SectionId == form.SectionId);
            if (section != null && section.Count() > 0 && section.Any())
            {
                return this.NotFound("Form Field already found.");
            }
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
            await _dynamicFormFieldService.AddAsync(form);
            return this.Ok();
        }

        [Route("dynamic-form-section-field-edit-save")]
        [HttpPut]
        public async Task<IActionResult> UpdateFieldSubmit(int id, FormField form)
        {
            var checksection = await _dynamicFormFieldService.GetAllAsync(x => x.Id != id && x.SectionId == form.SectionId && x.FieldTypeId == form.FieldTypeId);
            if (checksection != null && checksection.Count() > 0 && checksection.Any())
            {
                return this.NotFound("Field Field already found.");
            }
            var section = await _dynamicFormFieldService.GetByIdAsync(id);
            if (section == null)
            {
                return NotFound("Form Field not found.");
            }

            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                section.UpdatedBy = userIdAsInt;
            }
            section.FieldOrder = form.FieldOrder;
            section.FieldTypeId = form.FieldTypeId;
            section.SectionId = form.SectionId;
            section.CssClass = form.CssClass;
            section.JavaScript = form.JavaScript;
            section.IsActive = true;
            section.IsDelete = false;
            section.UpdatedAt = DateTime.Now;
            await _dynamicFormFieldService.UpdateAsync(section);
            return this.Ok();
        }

        [Route("dynamic-form-section-field-edit")]
        [HttpGet]
        public async Task<IActionResult> EditFormField(int id)
        {
            var forms = await _dynamicFormFieldService.GetByIdAsync(id);
            return Ok(forms);
        }

        // Toggle Active/Inactive Status
        [HttpGet("dynamic-form-section-field-toggle-active/{id}")]
        public async Task<IActionResult> ToggleFieldTypeStatus(int id)
        {
            var section = await _dynamicFormFieldService.GetByIdAsync(id);
            if (section == null)
            {
                return NotFound(new { message = "Form Field not found." });
            }
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                section.UpdatedBy = userIdAsInt;
            }
            section.IsActive = !section.IsActive;
            section.UpdatedAt = DateTime.Now;
            await _dynamicFormFieldService.UpdateAsync(section);
            return this.Ok();
        }

        [Route("dynamic-form-section-field-toggle-delete/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> ToggleDeleteSection(int id)
        {
            var section = await _dynamicFormFieldService.GetByIdAsync(id);
            if (section == null)
            {
                return NotFound(new { message = "Form Field not found." });
            }
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                section.UpdatedBy = userIdAsInt;
            }
            // Toggle the IsDelete flag
            section.IsDelete = !section.IsDelete;
            section.UpdatedAt = DateTime.Now;

            await _dynamicFormFieldService.UpdateAsync(section);

            return Ok(new { message = section.IsDelete ? "Form Field deleted." : "Form Field restored." });
        }
    }
}