using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicUserDataService : IGenericCytService<DynamicUserData>
    {
        // Additional methods for DynamicFormService
        Task<List<DynamicUserData>> GetUserDataList(string csc);

        Task<DynamicUserData> GetUserData(int id);

        Task<DynamicUserData> GetUserEditData(int id);

        Task<DynamicUserData> SubmitData(Dictionary<string, string> fieldValues);

        Task<List<dynamic>> GetFilteredUserDataAsync(List<string> requestedBorrowers, string FieldName);
    }

    public class DynamicUserDataService : GenericCytService<DynamicUserData>, IDynamicUserDataService
    {
        private readonly IDynamicUserDataRepository _Repository;
        private readonly ICytAdminService _cytAdminService;

        public DynamicUserDataService(IDynamicUserDataRepository Repository, ICytAdminService cytAdminService) : base(Repository)
        {
            _Repository = Repository;
            _cytAdminService = cytAdminService;
        }

        // Additional methods specific to DynamicForm
        public async Task<List<DynamicUserData>> GetUserDataList(string csc)
        {
            var userdetail = await _cytAdminService.GetUserDetails();
            var data = await _Repository.GetAllAsync(cv => cv.IsActive && !cv.IsDelete && ((userdetail.Roles.Contains("User") && cv.CreatedBy.ToString() == userdetail.UserId) || userdetail.Roles.Contains("Admin")),
                                   new Expression<Func<DynamicUserData, object?>>[] { x => x.Form!, y => y.Form!.Country, y => y.Form!.State, y => y.Form!.City }
                                   );
            data = data.Where(cv =>
                (cv.Form?.Country?.CountryName?.ToLower() ?? string.Empty) == csc.ToLower() ||
                (cv.Form?.State?.StateName?.ToLower() ?? string.Empty) == csc.ToLower() ||
                (cv.Form?.City?.CityName?.ToLower() ?? string.Empty) == csc.ToLower() ||
                (cv.Form?.FormName?.ToLower() ?? string.Empty) == csc.ToLower()
            ).ToList();
            return data;
        }

        public async Task<DynamicUserData> GetUserData(int id)
        {
            var data = await _Repository.GetAllAsync(x => x.Id == id && !x.IsDelete && x.IsActive);
            return data.FirstOrDefault()!;
        }

        public async Task<DynamicUserData> GetUserEditData(int id)
        {
            var data = await _Repository.GetAllAsync(x => x.Id == id && !x.IsDelete && x.IsActive);
            return data.FirstOrDefault()!;
        }

        public async Task<DynamicUserData> SubmitData(Dictionary<string, string> fieldValues)
        {
            int id = Convert.ToInt32(fieldValues["Id"].ToString());
            if (id == 0)
            {
                return await Add(fieldValues);
            }
            else
            {
                return await Edit(id, fieldValues);
            }
        }

        private async Task<DynamicUserData> Add(Dictionary<string, string> fieldValues)
        {
            fieldValues["SystemFormId"] = (!fieldValues.ContainsKey("SystemFormId") || (fieldValues.ContainsKey("SystemFormId") && string.IsNullOrEmpty(fieldValues["SystemFormId"])) ? "0" : fieldValues["SystemFormId"]);
            var formid = Convert.ToInt32(fieldValues["SystemFormId"]);
            var json = JsonConvert.SerializeObject(fieldValues);
            fieldValues["MetaMaskID"] = (!fieldValues.ContainsKey("MetaMaskID") || (fieldValues.ContainsKey("MetaMaskID") && string.IsNullOrEmpty(fieldValues["MetaMaskID"])) ? null : fieldValues["MetaMaskID"]);
            var lookup = new DynamicUserData()
            {
                MetaMaskID = fieldValues["MetaMaskID"],
                FormId = formid,
                UserData = json,
                CreatedAt = DateTime.Now,
                CreatedBy = Convert.ToInt32(await _cytAdminService.GetUserId()),
                UpdatedAt = DateTime.Now,
                UpdatedBy = Convert.ToInt32(await _cytAdminService.GetUserId()),
                IsActive = true,
                IsDelete = false,
                OldUserId = formid
            };
            return await _Repository.AddAsync(lookup);
        }

        private async Task<DynamicUserData> Edit(int id, Dictionary<string, string> fieldValues)
        {
            var objLook = await _Repository.GetByIdAsync(id);
            if (objLook != null)
            {
                objLook.IsActive = false;
                objLook.IsDelete = true;
                await _Repository.UpdateAsync(objLook);
            }

            return objLook!;
        }

        public async Task<List<dynamic>> GetFilteredUserDataAsync(List<string> requestedBorrowers, string FieldName)
        {
            // Fetch all active and non-deleted user data
            var obj = await _Repository.GetAllAsync(x => x.IsActive && !x.IsDelete);

            // Initialize the filtered results list
            var filteredResults = new List<dynamic>();

            foreach (var item in obj)
            {
                // Assume 'userdata' is a JSON string in the current object
                if (item.UserData != null)
                {
                    try
                    {
                        // Deserialize 'userdata' JSON to a dynamic object
                        var userDataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.UserData);

                        // Check if the provided MetaMask field name exists and matches any in the requestedBorrowers list
                        if (userDataJson != null && userDataJson.TryGetValue(FieldName, out var metaMaskId) &&
                            requestedBorrowers.Contains(metaMaskId.ToString()))
                        {
                            // Add the current item to the filtered results
                            filteredResults.Add(item);
                        }
                    }
                    catch (JsonException ex)
                    {
                        // Handle JSON deserialization error
                        Console.WriteLine($"Error deserializing userdata for item: {item.Id}, Error: {ex.Message}");
                    }
                }
            }

            return filteredResults;
        }
    }
}