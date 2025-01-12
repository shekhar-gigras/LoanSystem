using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
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

        [Route("borrower/aau/{formname}")]
        [HttpGet]
        [Authorize(Roles = "Admin,User")] // Specify multiple roles here
        public async Task<IActionResult> BothFormList(string formname)
        {
            Gigras.Software.BlockChain.Service.LoanProvider.GetDetail();
            var objForm = new Form();
            var obj = await _dynamicFormService.GetAllAsync(x => x.FormName!.ToLower() == formname);
            if (obj != null && obj.Any())
            {
                objForm = obj.FirstOrDefault()!;
                ViewBag.FormDescription = obj.FirstOrDefault()!.FormDescription;
            }
            return View("~/Views/Borrower/index.cshtml", objForm);
        }

        [Route("borrower/aau/form/{formname}")]
        [HttpGet]
        [Authorize(Roles = "Admin,User")] // Specify multiple roles here
        public async Task<IActionResult> BothForm(string formname)
        {
            var form = await _dynamicFormService.GetForm("form", formname);
            return View("~/Views/Borrower/Add.cshtml", form);
        }

        [Route("borrower/aau/{formid}/get-userdata-list")]
        [HttpGet]
        [Authorize(Roles = "Admin,User")] // Specify multiple roles here
        public async Task<IActionResult> getBothUserDataList(int formid)
        {
            try
            {
                var objForm = await _dynamicFormService.GetByIdAsync(formid);
                if (objForm != null)
                {
                    var entityName = objForm.EntityName?.ToLower();

                    if (!string.IsNullOrEmpty(objForm?.EntityName))
                    {
                        // Mapping entity names to corresponding service calls
                        var serviceMapping = new Dictionary<string, Func<Task<object>>>
                        {
                            { "loandetails", async () => await _loanDetailsService.GetList() },
                        };

                        if (serviceMapping.TryGetValue(entityName, out var serviceCall))
                        {
                            var data = await serviceCall();
                            return Ok(new { entity = entityName, data });
                        }
                    }

                    // Fallback for cases where EntityName is null or doesn't match
                    var fallbackData = await _dynamicUserDataService.GetUserDataList(formid);
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

        [Route("borrower/aau/{formid}/get-user-data")]
        [HttpGet]
        [Authorize(Roles = "Admin,User")] // Specify multiple roles here
        public async Task<IActionResult> GetBothJsonData(int formid, int id)
        {
            try
            {
                var objForm = await _dynamicFormService.GetByIdAsync(formid);
                if (objForm == null)
                    return Ok();

                var entityName = objForm.EntityName?.ToLower();

                // Map entity names to corresponding service calls
                var serviceMapping = new Dictionary<string, Func<Task<object>>>
                {
                        { "loandetails", async () => await _loanDetailsService.GetData(id) },
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
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Route("borrower/aau/{formid}/edit-form/{id:int}")]
        [HttpGet]
        [Authorize(Roles = "Admin,User")] // Specify multiple roles here
        public async Task<IActionResult> BothEditForm(int formid, int id)
        {
            try
            {
                ViewBag.UserId = await _cytAdminService.GetUserId();
                var form = await _dynamicFormService.GetForm(formid);

                if (form == null)
                {
                    string url = await GetListUrl(id);
                    return Redirect(url);
                }
                var entityName = form.EntityName!?.ToLower();

                // Dictionary for Service Mapping
                var serviceMapping = new Dictionary<string, Func<Task<object?>>>
                {
                        { "loandetails", async () => await _loanDetailsService.GetEditData(id) },
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

        [Route("borrower/aau/{formid}/{loanid}/submit-form")]
        [HttpPost]
        [Authorize(Roles = "Admin,User")] // Specify multiple roles here
        public async Task<IActionResult> SubmitForm(int formid, string loanid, Dictionary<string, List<string>> fieldValuesList)
        {
            try
            {
                Dictionary<string, string> fieldValues = fieldValuesList.ToDictionary(
                            kvp => kvp.Key,
                            kvp => string.Join(",", kvp.Value)
                        );
                fieldValues["Entity"] = (fieldValues.ContainsKey("Entity") ? fieldValues["Entity"] : "");
                fieldValues["Id"] = (!fieldValues.ContainsKey("Id") || (fieldValues.ContainsKey("Id") && string.IsNullOrEmpty(fieldValues["Id"])) ? "0" : fieldValues["Id"]);
                fieldValues["LoanId"] = loanid;
                fieldValues["FormId"] = formid.ToString();
                var entity = fieldValues["Entity"].ToString();
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

        //Admin
        [Route("borrower/a/{formname}")]
        [HttpGet]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> AdminFormList(string formname)
        {
            var objForm = new Form();
            var obj = await _dynamicFormService.GetAllAsync(x => x.FormName!.ToLower() == formname);
            if (obj != null && obj.Any())
            {
                objForm = obj.FirstOrDefault()!;
                ViewBag.FormDescription = obj.FirstOrDefault()!.FormDescription;
            }
            return View("~/Views/Borrower/index.cshtml", objForm);
        }

        [Route("borrower/a/form/{formname}")]
        [HttpGet]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> AdminForm(string formname)
        {
            var form = await _dynamicFormService.GetForm("form", formname);
            return View("~/Views/Borrower/Add.cshtml", form);
        }

        [Route("borrower/a/{formid}/get-userdata-list")]
        [HttpGet]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> getAdminUserDataList(int formid)
        {
            try
            {
                var objForm = await _dynamicFormService.GetByIdAsync(formid);
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
                    var fallbackData = await _dynamicUserDataService.GetUserDataList(formid);
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

        [Route("borrower/a/{formid}/get-user-data")]
        [HttpGet]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> GetAdminJsonData(int formid, int id)
        {
            try
            {
                var objForm = await _dynamicFormService.GetByIdAsync(formid);
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
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Route("borrower/a/{formid}/edit-form/{id:int}")]
        [HttpGet]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> AdminEditForm(int formid, int id)
        {
            try
            {
                ViewBag.UserId = await _cytAdminService.GetUserId();
                var form = await _dynamicFormService.GetForm(formid);

                if (form == null)
                {
                    string url = await GetListUrl(id);
                    return Redirect(url);
                }
                var entityName = form.EntityName!?.ToLower();

                // Dictionary for Service Mapping
                var serviceMapping = new Dictionary<string, Func<Task<object?>>>
                {
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

        //Admin

        [Route("borrower/a/{formid}/{loanid}/submit-form")]
        [HttpPost]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> AdminSubmitForm(int formid, string loanid, Dictionary<string, List<string>> fieldValuesList)
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
                fieldValues["LoanId"] = loanid;
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