using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicFieldOptionValueService : IGenericCytService<FieldOptionValue>
    {
        // Additional methods for DynamicFormService
        Task<List<FieldOptionValue>> GetActiveFieldOptionValueAsync();

        Task AddFieldOptionValueAsync(FieldOptionValue fieldType);

        Task EditFieldOptionValueAsync(FieldOptionValue fieldType);

        Task DeleteFieldOptionValueAsync(int id, bool isDeleted, int updatedBy);

        Task ToggleFieldOptionValueAsync(int id, bool isActive, int updatedBy);
    }

    public class DynamicFieldOptionValueService : GenericCytService<FieldOptionValue>, IDynamicFieldOptionValueService
    {
        private readonly IDynamicFieldOptionValueRepository _dynamicFieldOptionValueRepository;

        public DynamicFieldOptionValueService(IDynamicFieldOptionValueRepository dynamicFieldOptionValueRepository) : base(dynamicFieldOptionValueRepository)
        {
            _dynamicFieldOptionValueRepository = dynamicFieldOptionValueRepository;
        }

        public async Task<List<FieldOptionValue>> GetActiveFieldOptionValueAsync()
        {
            return await _dynamicFieldOptionValueRepository.GetAllAsync(ft => ft.IsActive);
        }

        public async Task AddFieldOptionValueAsync(FieldOptionValue fieldType)
        {
            fieldType.CreatedAt = DateTime.Now;
            fieldType.UpdatedAt = DateTime.Now;
            _dynamicFieldOptionValueRepository.AddAsync(fieldType);
        }

        public async Task EditFieldOptionValueAsync(FieldOptionValue fieldType)
        {
            var existingFieldType = await _dynamicFieldOptionValueRepository.GetByIdAsync(fieldType.Id);
            if (existingFieldType != null)
            {
                existingFieldType.OptionId = fieldType.OptionId;
                existingFieldType.OptionLabel = fieldType.OptionLabel;
                existingFieldType.OptionValue = fieldType.OptionValue;
                existingFieldType.UpdatedAt = DateTime.Now;
                existingFieldType.UpdatedBy = fieldType.UpdatedBy;
                await _dynamicFieldOptionValueRepository.UpdateAsync(existingFieldType);
            }
        }

        public async Task DeleteFieldOptionValueAsync(int id, bool isDeleted, int updatedBy)
        {
            var fieldType = await _dynamicFieldOptionValueRepository.GetByIdAsync(id);
            if (fieldType != null)
            {
                fieldType.IsDelete = isDeleted;
                fieldType.UpdatedAt = DateTime.Now;
                fieldType.UpdatedBy = updatedBy;
                await _dynamicFieldOptionValueRepository.UpdateAsync(fieldType);
            }
        }

        public async Task ToggleFieldOptionValueAsync(int id, bool isActive, int updatedBy)
        {
            var fieldType = await _dynamicFieldOptionValueRepository.GetByIdAsync(id);
            if (fieldType != null)
            {
                fieldType.IsActive = isActive;
                fieldType.UpdatedAt = DateTime.Now;
                fieldType.UpdatedBy = updatedBy;
                await _dynamicFieldOptionValueRepository.UpdateAsync(fieldType);
            }
        }

        // Additional methods specific to DynamicForm
    }
}