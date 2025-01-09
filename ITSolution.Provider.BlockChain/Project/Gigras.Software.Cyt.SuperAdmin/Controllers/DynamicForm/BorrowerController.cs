using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.DynamicForm
{
    [Route("sadmin")]
    [Authorize]
    public class BorrowerController : BaseController
    {
        private readonly IDynamicUserDataService _dynamicUserDataService;
        private readonly ISmartContractAbiService _smartContractAbiService;
        private readonly ISmartContractAddressService _smartContractAddressService;
        private readonly ILoanDetailsService _loanDetailsService;

        public BorrowerController(IDynamicFormService dynamicFormService, ICytAdminService cytAdminService,
            IDynamicUserDataService dynamicUserDataService, ISmartContractAbiService smartContractAbiService,
            ISmartContractAddressService smartContractAddressService, ILoanDetailsService loanDetailsService) : base(dynamicFormService, cytAdminService)
        {
            _dynamicUserDataService = dynamicUserDataService;
            _smartContractAbiService = smartContractAbiService;
            _smartContractAddressService = smartContractAddressService;
            _loanDetailsService = loanDetailsService;
        }
        [Route("borrower/{formname}")]
        [HttpGet]
        public async Task<IActionResult> FormList(string formname)
        {
            var obj = await _dynamicFormService.GetAllAsync(x => x.FormName!.ToLower() == formname);
            if (obj != null && obj.Any())
            {
                ViewBag.FormDescription = obj.FirstOrDefault()!.FormDescription;
            }
            else
            {
                ViewBag.FormDescription = formname;
            }
            ViewBag.FormName = formname;
            return View("~/Views/Borrower/index.cshtml");
        }

        [Route("borrower/form/{formname}")]
        [HttpGet]
        public async Task<IActionResult> Form(string formname)
        {
            var form = await _dynamicFormService.GetForm("form", formname);
            return View("~/Views/Borrower/Add.cshtml", form);
        }

        [Route("borrower/get-userdata-list")]
        [HttpGet]
        public async Task<IActionResult> getUserDataList(string csc)
        {
            try
            {
                var objForm = await _dynamicFormService.GetForm(csc.ToLower());
                if (objForm != null)
                {
                    var entityName = objForm.EntityName?.ToLower();

                    if (!string.IsNullOrEmpty(objForm?.EntityName))
                    {

                        // Mapping entity names to corresponding service calls
                        var serviceMapping = new Dictionary<string, Func<Task<object>>>
                        {
                               { "smartcontractabi", async () => await _smartContractAbiService.GetList() },
                             { "smartcontractaddress", async () => await _smartContractAddressService.GetList() },
                       };

                        if (serviceMapping.TryGetValue(entityName, out var serviceCall))
                        {
                            var data = await serviceCall();
                            return Ok(new { entity = entityName, data });
                        }
                    }

                    // Fallback for cases where EntityName is null or doesn't match
                    var fallbackData = await _dynamicUserDataService.GetUserDataList(csc);
                    return Ok(new { entity = entityName, fallbackData });
                }
            }
            catch (Exception ex)
            {
                // Log exception (if logging is available)
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Ok();
        }

        [Route("borrower/get-user-data")]
        [HttpGet]
        public async Task<IActionResult> GetJsonData(int id, string csc)
        {
            try
            {
                var objForm = await _dynamicFormService.GetForm(csc.ToLower());
                if (objForm == null)
                    return Ok();

                var entityName = objForm.EntityName?.ToLower();

                // Map entity names to corresponding service calls
                var serviceMapping = new Dictionary<string, Func<Task<object>>>
                {
                      { "smartcontractabi", async () => await _smartContractAbiService.GetData(id) },
                     { "smartcontractaddress", async () => await _smartContractAddressService.GetData(id) },
               };

                if (!string.IsNullOrEmpty(objForm?.EntityName))
                {
                    if (serviceMapping.TryGetValue(entityName, out var serviceCall))
                    {
                        var data = await serviceCall();

                        // Define JSON serialization settings
                        var settings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Handle circular references
                        };

                        string json = JsonConvert.SerializeObject(data, settings);
                        return Ok(json);
                    }
                }

                var dataObj = await _dynamicUserDataService.GetUserData(id);
                if (dataObj == null) return NotFound();
                return Ok(dataObj.UserData);


                return Ok(); // Fallback if entityName doesn't match any case
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Route("borrower/edit-form/{id:int}/{csc}")]
        [HttpGet]
        public async Task<IActionResult> EditForm(int id, string csc)
        {
            try
            {
                ViewBag.UserId = await _cytAdminService.GetUserId();
                var objForm = await _dynamicFormService.GetForm(csc.ToLower());

                if (objForm == null)
                {
                    string url = await GetListUrl(id);
                    return Redirect(url);
                }

                var form = await _dynamicFormService.GetForm("any", objForm.Id.ToString());

                ViewBag.ParentId = id;
                var entityName = objForm.EntityName.ToLower();

                // Dictionary for Service Mapping
                var serviceMapping = new Dictionary<string, Func<Task<object?>>>
                {
                     { "smartcontractabi", async () => await _smartContractAbiService.GetEditData(id) },
                     { "smartcontractaddress", async () => await _smartContractAddressService.GetEditData(id) },
              };

                if (serviceMapping.TryGetValue(entityName, out var serviceCall))
                {
                    var obj = await serviceCall();
                    if (obj != null)
                    {
                        var settings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Handle circular references
                        };
                        string json = JsonConvert.SerializeObject(obj, settings);
                        form.FieldValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        return View("~/Views/Borrower/Add.cshtml", form);
                    }
                    else
                    {
                        return View("~/Views/Borrower/Add.cshtml", form);
                    }
                }

                var userData = await _dynamicUserDataService.GetUserEditData(id);
                if (userData != null)
                {
                    form.FieldValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(userData.UserData!);
                    form.FieldValues!["Id"] = userData.Id.ToString();
                    return View("~/Views/Borrower/Add.cshtml", form);
                }


                string fallbackUrl = await GetListUrl(id);
                return Redirect(fallbackUrl);
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Route("borrower/submit-form")]
        [HttpPost]
        public async Task<IActionResult> SubmitForm(Dictionary<string, List<string>> fieldValuesList)
        {
            try
            {
                Dictionary<string, string> fieldValues = fieldValuesList.ToDictionary(
                            kvp => kvp.Key,
                            kvp => string.Join(",", kvp.Value)
                        );
                fieldValues["Entity"] = (fieldValues.ContainsKey("Entity") ? fieldValues["Entity"] : "");
                fieldValues["Id"] = (!fieldValues.ContainsKey("Id") || (fieldValues.ContainsKey("Id") && string.IsNullOrEmpty(fieldValues["Id"])) ? "0" : fieldValues["Id"]);

                var entity = fieldValues["Entity"].ToString();
                var formid = Convert.ToInt32(fieldValues["FormId"]);
                var form = await _dynamicFormService.GetForm("any", formid.ToString());
                var checkbox = form.FormsSections!.Select(x => x.FormFields!.Where(cv => cv.FieldType!.CtrlType!.ToLower() == "checkbox").Select(x => x.FieldType!.FieldName)).ToList();
                foreach (var item in checkbox)
                {
                    foreach (var childitem in item)
                    {
                        string fieldKey = childitem!.Replace(" ", "_");
                        fieldValues[fieldKey] = string.Join(",", Request.Form[fieldKey]);
                    }
                }
                switch (entity.ToLower())
                {
                    case "contract-address":
                        await _smartContractAddressService.SubmitData(fieldValues);
                        break;

                    case "contract-abi":
                        await _smartContractAbiService.SubmitData(fieldValues);
                        break;
                    case "loandetails":
                        await _dynamicUserDataService.SubmitData(fieldValues);
                        await _loanDetailsService.SubmitData(fieldValues);
                        break;
                    default:
                        await _dynamicUserDataService.SubmitData(fieldValues);
                        break;
                }
                return this.Ok();
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}