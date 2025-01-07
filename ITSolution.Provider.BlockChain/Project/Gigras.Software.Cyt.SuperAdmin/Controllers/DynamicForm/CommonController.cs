using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.General.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    public class CommonController : Controller
    {
        private readonly IDynamicFormService _dynamicFormService;
        private readonly CytContext _context;
        private readonly ICytAdminService _cytAdminService;

        public CommonController(IDynamicFormService dynamicFormService, CytContext cytContext, ICytAdminService cytAdminService)
        {
            _dynamicFormService = dynamicFormService;
            _context = cytContext;
            _cytAdminService = cytAdminService;
        }

        [HttpPost]
        [Route("File/Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                var uploadsFolder = Path.Combine("wwwroot/uploads/editorImage");

                // Ensure the folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique file name using a GUID
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

                // Combine the folder path and unique file name
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Return or store the relative path for later use
                var relativePath = Path.Combine("/uploads/editorImage", uniqueFileName);

                return Json(new { location = relativePath }); // TinyMCE requires the uploaded file's URL
            }
            return BadRequest("File upload failed.");
        }

        [HttpPost]
        [Route("forms/copy")]
        public async Task<IActionResult> CopyForm(FormCopyRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest("Form name cannot be empty.");
                }

                var originalForm = await _context.DynamicForms.Where(f => f.Id == request.Id)
                    .Include(f => f.FormsSections!.Where(x => !x.IsDelete && x.IsActive))     // Include the sections for the form
                        .ThenInclude(fs => fs.FormFields!.Where(x => !x.IsDelete && x.IsActive)).FirstOrDefaultAsync();  // Include the form fields for each section
                if (originalForm == null)
                {
                    return NotFound("Original form not found.");
                }

                var userid = await _cytAdminService.GetUserId();

                var newForm = new Form
                {
                    FormName = request.Name, // New name for the copied form
                    FormDescription = request.Name,
                    EntityName = originalForm.EntityName,
                    CountryId = originalForm.CountryId,
                    StateId = originalForm.StateId,
                    CityId = originalForm.CityId,
                    IsActive = originalForm.IsActive,
                    IsDelete = originalForm.IsDelete,
                    CreatedBy = Convert.ToInt32(userid),
                    CreatedAt = DateTime.Now,
                    FormsSections = originalForm.FormsSections!.Select(section => new FormsSection
                    {
                        SectionName = section.SectionName,
                        SectionDescription = section.SectionDescription,
                        IsActive = section.IsActive,
                        IsDelete = section.IsDelete,
                        CreatedBy = Convert.ToInt32(userid),
                        CreatedAt = DateTime.Now,
                        FormFields = section.FormFields!.Select(field => new FormField
                        {
                            SectionId = field.SectionId,
                            FieldTypeId = field.FieldTypeId,
                            FieldOrder = field.FieldOrder,
                            CssClass = field.CssClass,
                            JavaScript = field.JavaScript,
                            IsActive = field.IsActive,
                            IsDelete = field.IsDelete,
                            CreatedBy = Convert.ToInt32(userid),
                            CreatedAt = DateTime.Now
                        }).ToList()
                    }).ToList()
                };

                var obj = _dynamicFormService.AddAsync(newForm);
            }
            catch (Exception ex)
            {
            }
            return this.Ok(HttpStatusCode.OK);
        }
    }
}