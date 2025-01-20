using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.CytServcies.Dynamic;
using Gigras.Software.Cyt.Services.CytService;
using Gigras.Software.Cyt.Services.ICytServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gigras.Software.Cyt.Services
{
    public static class ServiceRegistration
    {
        public static void AddCytServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Register all services in the MyApp.Services project
            services.AddScoped<ICytAdminService, CytAdminService>();
            services.AddScoped<ICytCityService, CytCityService>();
            services.AddScoped<ICytCountryService, CytCountryService>();
            services.AddScoped<ICytStateService, CytStateService>();
            services.AddScoped<ICytUserService, CytUserService>();
            services.AddScoped<IDynamicFieldOptionService, DynamicFieldOptionService>();
            services.AddScoped<IDynamicFieldOptionValueService, DynamicFieldOptionValueService>();
            services.AddScoped<IDynamicFieldTypeService, DynamicFieldTypeService>();
            services.AddScoped<IDynamicFieldTypeValidaionService, DynamicFieldTypeValidaionService>();
            services.AddScoped<IDynamicFieldValidationService, DynamicFieldValidationService>();
            services.AddScoped<IDynamicFormFieldService, DynamicFormFieldService>();
            services.AddScoped<IDynamicFormService, DynamicFormService>();
            services.AddScoped<IDynamicFormSectionService, DynamicFormSectionService>();
            services.AddScoped<IDynamicBlockChainService, DynamicBlockChainService>();
            services.AddScoped<IDynamicCtrlService, DynamicCtrlService>();
            services.AddScoped<IDynamicValidationService, DynamicValidationService>();
            services.AddScoped<IDynamicUserDataService, DynamicUserDataService>();
            services.AddScoped<ISmartContractAbiService, SmartContractAbiService>();
            services.AddScoped<ISmartContractAddressService, SmartContractAddressService>();
            services.AddScoped<IBorrowerLoanService, BorrowerLoanService>();
            services.AddScoped<ILoanDetailsService, LoanDetailsService>();
            services.AddScoped<ILoanTransDetailsService, LoanTransDetailsService>();
        }
    }
}