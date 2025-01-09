using Gigras.Software.Cyt.Repositories.CytRepo;

using Microsoft.Extensions.DependencyInjection;

namespace Gigras.Software.Cyt.Repositories
{
    public static class RepositoryRegistration
    {
        public static void AddCytRepository(this IServiceCollection services)
        {
            services.AddScoped<ICytAdminRepository, CytAdminRepository>();
            services.AddScoped<ICytCityRepository, CytCityRepository>();
            services.AddScoped<ICytCountryRepository, CytCountryRepository>();
            services.AddScoped<ICytStateRepository, CytStateRepository>();
            services.AddScoped<ICytUserRepository, CytUserRepository>();
            services.AddScoped<IDynamicFieldOptionRepository, DynamicFieldOptionRepository>();
            services.AddScoped<IDynamicFieldOptionValueRepository, DynamicFieldOptionValueRepository>();
            services.AddScoped<IDynamicFieldTypeRepository, DynamicFieldTypeRepository>();
            services.AddScoped<IDynamicFieldValidationRepository, DynamicFieldValidationRepository>();
            services.AddScoped<IDynamicFieldTypeValidaionRepository, DynamicFieldTypeValidaionRepository>();
            services.AddScoped<IDynamicFormFieldRepository, DynamicFormFieldRepository>();
            services.AddScoped<IDynamicFormRepository, DynamicFormRepository>();
            services.AddScoped<IDynamicFormSectionRepository, DynamicFormSectionRepository>();
            services.AddScoped<IDynamicValidationRepository, DynamicValidationRepository>();
            services.AddScoped<IDynamicCtrlRepository, DynamicCtrlRepository>();
            services.AddScoped<IDynamicBlockChainRepository, DynamicBlockChainRepository>();
            services.AddScoped<IDynamicUserDataRepository, DynamicUserDataRepository>();
            services.AddScoped<ISmartContractAbiRepository, SmartContractAbiRepository>();
            services.AddScoped<ISmartContractAddressRepository, SmartContractAddressRepository>();
            services.AddScoped<IBorrowerLoanRepository, BorrowerLoanRepository>();
            services.AddScoped<ILoanDetailsRepository, LoanDetailsRepository>();
        }
    }
}