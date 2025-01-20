using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.General.Model;
using Gigras.Software.Generic.Services;
using Microsoft.EntityFrameworkCore;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicFormService : IGenericCytService<Form>
    {
        // Additional methods for DynamicFormService
        Task<Form> GetForm(string mode, string formname);

        Task<Form> GetForm(string formname);

        Task<Form> GetForm(int formid);

        Task<List<FieldInfo>> GetFormFieldInfo(string formname);
    }

    public class DynamicFormService : GenericCytService<Form>, IDynamicFormService
    {
        private readonly IDynamicFormRepository _dynamicformRepository;
        private readonly IDynamicFieldValidationService _dynamicfieldValidationService;
        private readonly CytContext _context;

        public DynamicFormService(IDynamicFormRepository dynamicformRepository,
            IDynamicFieldValidationService dynamicfieldValidationService, CytContext cytContext
            ) : base(dynamicformRepository)
        {
            _dynamicformRepository = dynamicformRepository;
            _dynamicfieldValidationService = dynamicfieldValidationService;
            _context = cytContext;
        }

        // Additional methods specific to DynamicForm

        public async Task<Form> GetForm(string mode, string formname)
        {
            var form = await _context.DynamicForms
                        .Where(f =>
                            (mode == "any" && f.Id.ToString() == formname) ||
                            (mode == "form" && f.FormName!.Replace(" ", "-") == formname) ||
                            (mode == "country" && f.Country!.CountryName!.Replace(" ", "-") == formname) ||
                            (mode == "state" && f.State!.StateName!.Replace(" ", "-") == formname) ||
                            (mode == "city" && f.City!.CityName!.Replace(" ", "-") == formname)
                        )  // Apply your filtering criteria
                    .Include(f => f.FormsSections!.Where(x => !x.IsDelete && x.IsActive))     // Include the sections for the form
                        .ThenInclude(fs => fs.FormFields!.Where(x => !x.IsDelete && x.IsActive))  // Include the form fields for each section
                            .ThenInclude(fss => fss.FieldType)
                                .ThenInclude(fsss => fsss!.FieldTypeValidations!.Where(x => !x.IsDelete && x.IsActive))
                                    .ThenInclude(x => x.FieldValidation)
               .FirstOrDefaultAsync();
            if (form != null)
            {
                // Load FieldOption and FieldTypeValidations separately
                foreach (var section in form.FormsSections!)
                {
                    foreach (var fieldType in from formField in section.FormFields
                                              let fieldType = formField.FieldType
                                              select fieldType)
                    {
                        // Load FieldOption
                        await _context.Entry(fieldType)
                                                    .Reference(f => f.FieldOption) // Load FieldOption
                                                    .LoadAsync();// Load the FieldOption and its FieldOptionValues
                    }
                }
            }

            return form;
        }

        public async Task<Form> GetForm(string formname)
        {
            var form = await _context.DynamicForms
                        .Where(f =>
                            (f.Id.ToString() == formname) ||
                            (f.FormName!.Replace(" ", "-") == formname) ||
                            (f.Country!.CountryName!.Replace(" ", "-") == formname) ||
                            (f.State!.StateName!.Replace(" ", "-") == formname) ||
                            (f.City!.CityName!.Replace(" ", "-") == formname)
                        )  // Apply your filtering criteria
                    .Include(f => f.FormsSections!.Where(x => !x.IsDelete && x.IsActive))     // Include the sections for the form
                        .ThenInclude(fs => fs.FormFields!.Where(x => !x.IsDelete && x.IsActive))  // Include the form fields for each section
                            .ThenInclude(fss => fss.FieldType)
                                .ThenInclude(fsss => fsss!.FieldTypeValidations!.Where(x => !x.IsDelete && x.IsActive))
                                    .ThenInclude(x => x.FieldValidation)
               .FirstOrDefaultAsync();
            if (form != null)
            {
                // Load FieldOption and FieldTypeValidations separately
                foreach (var section in form.FormsSections!)
                {
                    foreach (var fieldType in from formField in section.FormFields
                                              let fieldType = formField.FieldType
                                              select fieldType)
                    {
                        // Load FieldOption
                        await _context.Entry(fieldType)
                                                    .Reference(f => f.FieldOption) // Load FieldOption
                                                    .LoadAsync();// Load the FieldOption and its FieldOptionValues
                    }
                }
            }

            return form;
        }

        public async Task<Form> GetForm(int formid)
        {
            var form = await _context.DynamicForms
                        .Where(f => f.Id == formid)  // Apply your filtering criteria
                    .Include(f => f.FormsSections!.Where(x => !x.IsDelete && x.IsActive))     // Include the sections for the form
                        .ThenInclude(fs => fs.FormFields!.Where(x => !x.IsDelete && x.IsActive))  // Include the form fields for each section
                            .ThenInclude(fss => fss.FieldType)
                                .ThenInclude(fsss => fsss!.FieldTypeValidations!.Where(x => !x.IsDelete && x.IsActive))
                                    .ThenInclude(x => x.FieldValidation)
               .FirstOrDefaultAsync();
            if (form != null)
            {
                // Load FieldOption and FieldTypeValidations separately
                foreach (var section in form.FormsSections!)
                {
                    foreach (var fieldType in from formField in section.FormFields
                                              let fieldType = formField.FieldType
                                              select fieldType)
                    {
                        // Load FieldOption
                        await _context.Entry(fieldType)
                                                    .Reference(f => f.FieldOption) // Load FieldOption
                                                    .LoadAsync();// Load the FieldOption and its FieldOptionValues
                    }
                }
            }

            return form;
        }

        public async Task<List<FieldInfo>> GetFormFieldInfo(string formname)
        {
            // Retrieve the form and its related data
            var form = await _context.DynamicForms
                .Where(f =>
                    (f.Id.ToString() == formname) ||
                    (f.FormName!.Replace(" ", "-") == formname) ||
                    (f.Country!.CountryName!.Replace(" ", "-") == formname) ||
                    (f.State!.StateName!.Replace(" ", "-") == formname) ||
                    (f.City!.CityName!.Replace(" ", "-") == formname)
                )
                .Include(f => f.FormsSections!.Where(x => !x.IsDelete && x.IsActive))
                    .ThenInclude(fs => fs.FormFields!.Where(x => !x.IsDelete && x.IsActive))
                        .ThenInclude(ff => ff.FieldType)
                .FirstOrDefaultAsync();

            // If the form is not found, return an empty list
            if (form == null) return new List<FieldInfo>();

            // Extract and return FieldName and FieldDescription
            var fieldInfoList = new List<FieldInfo>();

            foreach (var section in form.FormsSections!)
            {
                foreach (var field in section.FormFields!)
                {
                    fieldInfoList.Add(new FieldInfo
                    {
                        FieldName = field.FieldType!.FieldName!,
                        FieldDescription = field.FieldType!.FieldDescription!,
                    });
                }
            }

            return fieldInfoList;
        }
    }
}