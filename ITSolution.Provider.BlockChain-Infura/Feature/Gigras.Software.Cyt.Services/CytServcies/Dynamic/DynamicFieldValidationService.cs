using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicFieldValidationService : IGenericCytService<FieldValidation>
    {
        // Additional methods for DynamicFormService
        Task<List<FieldValidation>> GetActiveFieldValidationAsync();

        Task AddFieldValidationAsync(FieldValidation fieldType);

        Task EditFieldValidationAsync(FieldValidation fieldType);

        Task DeleteFieldValidationAsync(int id, bool isDeleted, int updatedBy);

        Task ToggleFieldValidationStatusAsync(int id, bool isActive, int updatedBy);
    }

    public class DynamicFieldValidationService : GenericCytService<FieldValidation>, IDynamicFieldValidationService
    {
        private readonly IDynamicFieldValidationRepository _dynamicFieldValidationRepository;
        private readonly ICytAdminService _cytAdminService;

        public DynamicFieldValidationService(IDynamicFieldValidationRepository dynamicFieldValidationRepository, ICytAdminService cytAdminService) : base(dynamicFieldValidationRepository)
        {
            _dynamicFieldValidationRepository = dynamicFieldValidationRepository;
            _cytAdminService = cytAdminService;
        }

        public async Task<List<FieldValidation>> GetActiveFieldValidationAsync()
        {
            return await _dynamicFieldValidationRepository.GetAllAsync(ft => ft.IsActive);
        }

        public async Task AddFieldValidationAsync(FieldValidation fieldType)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                uid = userIdAsInt;
            }

            fieldType.CreatedAt = DateTime.Now;
            fieldType.UpdatedAt = DateTime.Now;
            fieldType.CreatedBy = uid;
            fieldType.UpdatedBy = uid;
            fieldType.IsActive = true;
            fieldType.IsDelete = false;
            await _dynamicFieldValidationRepository.AddAsync(fieldType);
        }

        public async Task EditFieldValidationAsync(FieldValidation fieldType)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                uid = userIdAsInt;
            }

            var existingFieldType = await _dynamicFieldValidationRepository.GetByIdAsync(fieldType.Id);
            if (existingFieldType != null)
            {
                existingFieldType.ValidationName = fieldType.ValidationName;
                existingFieldType.ValidationType = fieldType.ValidationType;
                existingFieldType.ValidationValue = fieldType.ValidationValue;
                existingFieldType.ErrorMessage = fieldType.ErrorMessage;
                existingFieldType.UpdatedAt = DateTime.Now;
                existingFieldType.UpdatedBy = uid;
                existingFieldType.IsActive = true;
                existingFieldType.IsDelete = false;
                await _dynamicFieldValidationRepository.UpdateAsync(existingFieldType);
            }
        }

        public async Task DeleteFieldValidationAsync(int id, bool isDeleted, int updatedBy)
        {
            var fieldType = await _dynamicFieldValidationRepository.GetByIdAsync(id);
            if (fieldType != null)
            {
                fieldType.IsDelete = isDeleted;
                fieldType.UpdatedAt = DateTime.Now;
                fieldType.UpdatedBy = updatedBy;
                await _dynamicFieldValidationRepository.UpdateAsync(fieldType);
            }
        }

        public async Task ToggleFieldValidationStatusAsync(int id, bool isActive, int updatedBy)
        {
            string? userid = await _cytAdminService.GetUserId();
            int uid = 1;
            if (int.TryParse(userid, out int userIdAsInt))
            {
                updatedBy = userIdAsInt;
            }
            var fieldType = await _dynamicFieldValidationRepository.GetByIdAsync(id);
            if (fieldType != null)
            {
                fieldType.IsActive = isActive;
                fieldType.UpdatedAt = DateTime.Now;
                fieldType.UpdatedBy = updatedBy;
                await _dynamicFieldValidationRepository.UpdateAsync(fieldType);
            }
        }

        // Additional methods specific to DynamicForm
    }
}