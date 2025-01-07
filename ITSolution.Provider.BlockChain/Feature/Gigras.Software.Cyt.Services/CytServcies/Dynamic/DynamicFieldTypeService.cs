using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;
using Microsoft.EntityFrameworkCore;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicFieldTypeService : IGenericCytService<FieldType>
    {
        Task<List<FieldType>> GetActiveFieldTypesAsync();

        Task AddFieldTypeAsync(FieldType fieldType);

        Task EditFieldTypeAsync(FieldType fieldType);

        Task DeleteFieldTypeAsync(int id, bool isDeleted, int updatedBy);

        Task ToggleFieldTypeStatusAsync(int id, bool isActive, int updatedBy);

        // Additional methods for DynamicFormService
    }

    public class DynamicFieldTypeService : GenericCytService<FieldType>, IDynamicFieldTypeService
    {
        private readonly IDynamicFieldTypeRepository _dynamicFieldTypeRepository;
        private readonly ICytAdminService _cytAdminService;

        public DynamicFieldTypeService(IDynamicFieldTypeRepository dynamicFieldTypeRepository, ICytAdminService cytAdminService) : base(dynamicFieldTypeRepository)
        {
            _dynamicFieldTypeRepository = dynamicFieldTypeRepository;
            _cytAdminService = cytAdminService;
        }

        public async Task<List<FieldType>> GetActiveFieldTypesAsync()
        {
            return await _dynamicFieldTypeRepository.GetAllAsync(ft => ft.IsActive);
        }

        public async Task AddFieldTypeAsync(FieldType fieldType)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                uid = userIdAsInt;
            }

            fieldType.Steps = Math.Round(fieldType.Steps ?? 0, 4);
            fieldType.CreatedAt = DateTime.Now;
            fieldType.UpdatedAt = DateTime.Now;
            if (fieldType.FieldTypeValidations != null && fieldType.FieldTypeValidations.Count() > 0)
            {
                foreach (var item in fieldType.FieldTypeValidations)
                {
                    item.CreatedAt = DateTime.Now;
                    item.UpdatedAt = DateTime.Now;
                    item.CreatedBy = uid;
                    item.UpdatedBy = uid;
                    item.IsActive = true;
                    item.IsDelete = false;
                }
            }
            await _dynamicFieldTypeRepository.AddAsync(fieldType);
        }

        public async Task EditFieldTypeAsync(FieldType fieldType)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                uid = userIdAsInt;
            }

            var existingFieldType = await _dynamicFieldTypeRepository.GetByIdAsync(fieldType.Id,
                 x => x.Include("FieldTypeValidations"));
            if (existingFieldType != null)
            {
                existingFieldType.MaxLength = fieldType.MaxLength;
                existingFieldType.RangeStart = fieldType.RangeStart;
                existingFieldType.RangeEnd = fieldType.RangeEnd;
                existingFieldType.MinValue = fieldType.MinValue;
                existingFieldType.MaxValue = fieldType.MaxValue;
                existingFieldType.CtrlType = fieldType.CtrlType;
                existingFieldType.DefaultValue = fieldType.DefaultValue;
                existingFieldType.Steps = Math.Round(fieldType.Steps ?? 0, 4);

                existingFieldType.BlockChainFieldId = fieldType.BlockChainFieldId;
                existingFieldType.FieldTypeOptionId = fieldType.FieldTypeOptionId;
                existingFieldType.FieldName = fieldType.FieldName;
                existingFieldType.FieldDescription = fieldType.FieldDescription;
                existingFieldType.HasOptions = fieldType.HasOptions;
                existingFieldType.RequiresScript = fieldType.RequiresScript;
                existingFieldType.UpdatedAt = DateTime.Now;
                existingFieldType.UpdatedBy = uid;
                if (existingFieldType.FieldTypeValidations != null && existingFieldType.FieldTypeValidations.Count() > 0)
                {
                    foreach (var item in existingFieldType.FieldTypeValidations)
                    {
                        item.IsActive = false;
                        item.IsDelete = true;
                    }
                }
                if (fieldType.FieldTypeValidations != null && fieldType.FieldTypeValidations.Count() > 0)
                {
                    foreach (var item in fieldType.FieldTypeValidations)
                    {
                        var obj = existingFieldType.FieldTypeValidations.FirstOrDefault(x => x.FieldValidationId == item.FieldValidationId);
                        if (obj == null)
                        {
                            existingFieldType.FieldTypeValidations.Add(new FieldTypeValidation()
                            {
                                FieldValidationId = item.FieldValidationId,
                                UpdatedAt = DateTime.Now,
                                UpdatedBy = uid,
                                CreatedAt = DateTime.Now,
                                CreatedBy = uid,
                                IsActive = true,
                                IsDelete = false,
                            });
                        }
                        else
                        {
                            obj.FieldValidationId = item.FieldValidationId;
                            obj.UpdatedAt = DateTime.Now;
                            obj.UpdatedBy = uid;
                            obj.IsActive = true;
                            obj.IsDelete = false;
                        }
                    }
                }
                await _dynamicFieldTypeRepository.UpdateAsync(existingFieldType);
            }
        }

        public async Task DeleteFieldTypeAsync(int id, bool isDeleted, int updatedBy)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                updatedBy = userIdAsInt;
            }

            var fieldType = await _dynamicFieldTypeRepository.GetByIdAsync(id);
            if (fieldType != null)
            {
                fieldType.IsDelete = isDeleted;
                fieldType.UpdatedAt = DateTime.Now;
                fieldType.UpdatedBy = updatedBy;
                await _dynamicFieldTypeRepository.UpdateAsync(fieldType);
            }
        }

        public async Task ToggleFieldTypeStatusAsync(int id, bool isActive, int updatedBy)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                updatedBy = userIdAsInt;
            }
            var fieldType = await _dynamicFieldTypeRepository.GetByIdAsync(id);
            if (fieldType != null)
            {
                fieldType.IsActive = isActive;
                fieldType.UpdatedAt = DateTime.Now;
                fieldType.UpdatedBy = updatedBy;
                await _dynamicFieldTypeRepository.UpdateAsync(fieldType);
            }
        }

        // Additional methods specific to DynamicForm
    }
}