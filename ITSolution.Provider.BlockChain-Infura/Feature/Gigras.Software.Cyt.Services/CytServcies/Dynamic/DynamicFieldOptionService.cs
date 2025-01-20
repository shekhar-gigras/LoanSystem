using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;
using Microsoft.EntityFrameworkCore;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicFieldOptionService : IGenericCytService<FieldOption>
    {
        // Additional methods for DynamicFormService
        Task<List<FieldOption>> GetActiveFieldOptionAsync();

        Task AddFieldOptionAsync(FieldOption fieldType);

        Task EditFieldOptionAsync(FieldOption fieldType);

        Task DeleteFieldOptionAsync(int id, bool isDeleted, int updatedBy);

        Task ToggleFieldOptionAsync(int id, bool isActive, int updatedBy);
    }

    public class DynamicFieldOptionService : GenericCytService<FieldOption>, IDynamicFieldOptionService
    {
        private readonly IDynamicFieldOptionRepository _dynamicFieldOptionRepository;
        private readonly ICytAdminService _cytAdminService;

        public DynamicFieldOptionService(IDynamicFieldOptionRepository dynamicFieldOptionRepository, ICytAdminService cytAdminService) : base(dynamicFieldOptionRepository)
        {
            _dynamicFieldOptionRepository = dynamicFieldOptionRepository;
            _cytAdminService = cytAdminService;
        }

        public async Task<List<FieldOption>> GetActiveFieldOptionAsync()
        {
            return await _dynamicFieldOptionRepository.GetAllAsync(ft => ft.IsActive);
        }

        public async Task AddFieldOptionAsync(FieldOption fieldType)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                uid = userIdAsInt;
            }

            int i = 0;
            if (fieldType.OptionLabel != null && fieldType.OptionValue != null)
            {
                foreach (var item in fieldType.OptionLabel)
                {
                    fieldType.OptionValues!.Add(new FieldOptionValue()
                    {
                        OptionLabel = item,
                        OptionValue = fieldType.OptionValue[i],
                        CreatedAt = DateTime.Now,
                        CreatedBy = uid,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = uid,
                        IsActive = true,
                        IsDelete = false,
                    });
                    i++;
                }
            }
            else
            {
                throw new InvalidOperationException("OptionLabel or OptionValue is null.");
            }

            // Set metadata fields
            fieldType.CreatedAt = DateTime.Now;
            fieldType.CreatedBy = uid;
            fieldType.UpdatedAt = DateTime.Now;
            fieldType.UpdatedBy = uid;
            await _dynamicFieldOptionRepository.AddAsync(fieldType);
        }

        public async Task EditFieldOptionAsync(FieldOption fieldType)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                uid = userIdAsInt;
            }
            var existingFieldType = await _dynamicFieldOptionRepository.GetByIdAsync(fieldType.Id,
                 include: query => query.Include(x => x.OptionValues.Where(ov => ov.IsActive && !ov.IsDelete))); // Eagerly load OptionValues);
            if (existingFieldType != null)
            {
                foreach (var existingOption in existingFieldType.OptionValues)
                {
                    var matchingNewOption = fieldType.OptionLabel?
                        .Select((label, index) => new { label, value = fieldType.OptionValue![index] })
                        .FirstOrDefault(o => o.label.ToLower() == existingOption.OptionLabel!.ToLower());

                    if (matchingNewOption != null)
                    {
                        // Update existing option
                        existingOption.OptionLabel = matchingNewOption.label;
                        existingOption.OptionValue = matchingNewOption.value;
                        existingOption.UpdatedAt = DateTime.Now;
                        existingOption.UpdatedBy = uid;
                        existingOption.IsActive = true;
                        existingOption.IsDelete = false;
                    }
                    else
                    {
                        // Mark as inactive/deleted
                        existingOption.IsActive = false;
                        existingOption.IsDelete = true;
                        existingOption.UpdatedAt = DateTime.Now;
                        existingOption.UpdatedBy = uid;
                    }
                }
                if (fieldType.OptionLabel != null && fieldType.OptionValue != null)
                {
                    int i = 0;
                    foreach (var label in fieldType.OptionLabel)
                    {
                        var existingOption = existingFieldType.OptionValues
                            .FirstOrDefault(x => x.OptionLabel!.ToLower() == label.ToLower());

                        if (existingOption == null)
                        {
                            // Add new option
                            existingFieldType.OptionValues.Add(new FieldOptionValue
                            {
                                OptionLabel = label,
                                OptionValue = fieldType.OptionValue[i],
                                CreatedAt = DateTime.Now,
                                CreatedBy = uid,
                                UpdatedAt = DateTime.Now,
                                UpdatedBy = uid,
                                IsActive = true,
                                IsDelete = false
                            });
                        }
                        i++;
                    }
                }
                else
                {
                    throw new InvalidOperationException("OptionLabel or OptionValue is null.");
                }
                existingFieldType.OptionName = fieldType.OptionName;
                existingFieldType.IsDynamic = fieldType.IsDynamic;
                existingFieldType.SourceTable = fieldType.SourceTable;
                existingFieldType.TextValueField = fieldType.TextValueField;
                existingFieldType.Condition = fieldType.Condition;
                existingFieldType.UpdatedAt = DateTime.Now;
                existingFieldType.UpdatedBy = uid;
                await _dynamicFieldOptionRepository.UpdateAsync(existingFieldType);
            }
        }

        public async Task DeleteFieldOptionAsync(int id, bool isDeleted, int updatedBy)
        {
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                updatedBy = userIdAsInt;
            }
            var fieldType = await _dynamicFieldOptionRepository.GetByIdAsync(id);
            if (fieldType != null)
            {
                fieldType.IsDelete = isDeleted;
                fieldType.UpdatedAt = DateTime.Now;
                fieldType.UpdatedBy = updatedBy;
                await _dynamicFieldOptionRepository.UpdateAsync(fieldType);
            }
        }

        public async Task ToggleFieldOptionAsync(int id, bool isActive, int updatedBy)
        {
            string? userid = await _cytAdminService.GetUserId();
            if (int.TryParse(userid, out int userIdAsInt))
            {
                updatedBy = userIdAsInt;
            }

            var fieldType = await _dynamicFieldOptionRepository.GetByIdAsync(id);
            if (fieldType != null)
            {
                fieldType.IsActive = isActive;
                fieldType.UpdatedAt = DateTime.Now;
                fieldType.UpdatedBy = updatedBy;
                await _dynamicFieldOptionRepository.UpdateAsync(fieldType);
            }
        }

        // Additional methods specific to DynamicForm
    }
}