using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    [Route("sadmin")]
    [Authorize]
    public class DynamicFormSectionController : BaseController
    {
        private readonly IDynamicFormSectionService _dynamicFormSectionService;

        public DynamicFormSectionController(IDynamicFormService dynamicFormService, IDynamicFormSectionService dynamicFormSectionService,
            ICytAdminService cytAdminService) : base(dynamicFormService, cytAdminService)
        {
            _dynamicFormSectionService = dynamicFormSectionService;
        }

        [Route("dynamic-form-section")]
        [HttpGet]
        public async Task<IActionResult> IndexSection(int id)
        {
            var forms = await _dynamicFormSectionService.GetAllAsync(x => x.FormId == id);
            forms = forms.OrderBy(x => x.SortOrder).ToList();
            return Ok(forms);
        }

        [Route("dynamic-form-section-add-save")]
        [HttpPost]
        public async Task<IActionResult> AddSectionSubmit(FormsSection form)
        {
            var section = await _dynamicFormSectionService.GetAllAsync(x => x.FormId == form.FormId && x.SectionName.ToLower().Trim() == form.SectionName.ToLower().Trim());
            if (section != null && section.Count() > 0 && section.Any())
            {
                return this.NotFound("Section name already found.");
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
            await _dynamicFormSectionService.AddAsync(form);
            return this.Ok();
        }

        [Route("dynamic-form-section-edit-save")]
        [HttpPut]
        public async Task<IActionResult> UpdateSectionSubmit(int id, FormsSection form)
        {
            var checksection = await _dynamicFormSectionService.GetAllAsync(x => x.FormId == form.FormId && x.Id != id && x.SectionName.ToLower().Trim() == form.SectionName.ToLower().Trim());
            if (checksection != null && checksection.Count() > 0 && checksection.Any())
            {
                return this.NotFound("Section name already found.");
            }
            var section = await _dynamicFormSectionService.GetByIdAsync(id);
            if (section == null)
            {
                return NotFound("Section not found.");
            }
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                section.UpdatedBy = userIdAsInt;
            }
            section.SectionName = form.SectionName;
            section.SectionDescription = form.SectionDescription;
            section.SortOrder = form.SortOrder;
            section.IsActive = true;
            section.IsDelete = false;
            section.UpdatedAt = DateTime.Now;
            await _dynamicFormSectionService.UpdateAsync(section);
            return this.Ok();
        }

        [Route("dynamic-form-section-edit")]
        [HttpGet]
        public async Task<IActionResult> EditSection(int id)
        {
            var forms = await _dynamicFormSectionService.GetByIdAsync(id);
            return Ok(forms);
        }

        // Toggle Active/Inactive Status
        [HttpGet("dynamic-form-section-toggle-active/{id}")]
        public async Task<IActionResult> ToggleFieldTypeStatus(int id)
        {
            var section = await _dynamicFormSectionService.GetByIdAsync(id);
            if (section == null)
            {
                return NotFound(new { message = "Section not found." });
            }
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                section.UpdatedBy = userIdAsInt;
            }
            section.IsActive = !section.IsActive;
            section.UpdatedAt = DateTime.Now;
            await _dynamicFormSectionService.UpdateAsync(section);
            return this.Ok();
        }

        [Route("dynamic-form-section-toggle-delete/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> ToggleDeleteSection(int id)
        {
            var section = await _dynamicFormSectionService.GetByIdAsync(id);
            if (section == null)
            {
                return NotFound(new { message = "Section not found." });
            }
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                section.UpdatedBy = userIdAsInt;
            }
            // Toggle the IsDelete flag
            section.IsDelete = !section.IsDelete;
            section.UpdatedAt = DateTime.Now;

            await _dynamicFormSectionService.UpdateAsync(section);

            return Ok(new { message = section.IsDelete ? "Section deleted." : "Section restored." });
        }
    }
}