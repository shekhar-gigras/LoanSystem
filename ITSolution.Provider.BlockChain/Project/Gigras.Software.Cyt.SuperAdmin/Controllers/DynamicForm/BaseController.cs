using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    public class BaseController : Controller
    {
        public readonly IDynamicFormService _dynamicFormService;
        public readonly ICytAdminService _cytAdminService;

        public BaseController(IDynamicFormService dynamicFormService, ICytAdminService cytAdminService)
        {
            _dynamicFormService = dynamicFormService;
            _cytAdminService = cytAdminService;
        }

        protected async Task SetLeftMenuFormDataAsync()
        {
            var forms = await _dynamicFormService.GetAllAsync(
                x => x.IsActive && !x.IsDelete,
                y => y.Country!,
                y => y.State!,
                y => y.City!
            );

            ViewBag.Forms = forms;
        }

        protected async Task GetDbSchema()
        {
            var dbSetNames = typeof(CytContext)
                .GetProperties()
                .Where(p => !p.Name.ToLower().Contains("dynamic") && p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.Name)
                .ToList();

            ViewBag.DbSetNames = dbSetNames;
        }

        protected async Task<string> GetListUrl(int formid)
        {
            var obj = await _dynamicFormService.GetAllAsync(
                x => x.Id == formid && x.IsActive && !x.IsDelete,
                y => y.Country!,
                y => y.State!,
                y => y.City!
            );
            string url = "/sadmin/borrower";

            if (obj != null && obj.Count() > 0)
            {
                var objForm = obj.FirstOrDefault();

                // Build URL based on available data
                if (objForm!.City != null)
                {
                    url = $"{url}/city/{objForm.City.CityName!.Replace(" ", "-")}";
                }
                else if (objForm.State != null)
                {
                    url = $"{url}/state/{objForm.State.StateName!.Replace(" ", "-")}";
                }
                else if (objForm!.Country != null)
                {
                    url = $"{url}/country/{objForm.Country!.CountryName!.Replace(" ", "-")}";
                }
                else
                {
                    url = $"{url}/{objForm.FormName!.Replace(" ", "-")}";
                }
            }
            return url;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await GetDbSchema();
            // Call the method to set ViewBag.Forms before the action executes
            await SetLeftMenuFormDataAsync();
            ViewBag.UserId = await _cytAdminService.GetUserId();
            // Proceed to the action
            await next();
        }
    }
}