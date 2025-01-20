using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.General.Helper;
using Gigras.Software.General.Model;
using Gigras.Software.Generic.Services;
using System.Linq.Expressions;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface ILoanDetailsService : IGenericCytService<LoanDetails>
    {
        // Additional methods for DynamicFormService
        Task<LoanDetails> SubmitData(Dictionary<string, string> fieldValues);

        Task<List<LoanDetails>> GetList();

        Task<LoanDetails> GetData(int id);

        Task<LoanDetails> GetEditData(int id);

        Task<LoanCountInfo> GetInfo();

        Task<List<LoanTransDetails>> GetTransList(string q = "");

        Task<LoanTransDetails> GetTransData(int id);
    }

    public class LoanDetailsService : GenericCytService<LoanDetails>, ILoanDetailsService
    {
        private readonly ILoanDetailsRepository _LoanDetailsRepository;
        private readonly ICytAdminService _cytAdminService;
        private readonly ILoanTransDetailsRepository _LoanTransDetailsRepository;

        public LoanDetailsService(ILoanDetailsRepository LoanDetailsRepository, ICytAdminService cytAdminService, ILoanTransDetailsRepository loanTransDetailsRepository) : base(LoanDetailsRepository)
        {
            _LoanDetailsRepository = LoanDetailsRepository;
            _cytAdminService = cytAdminService;
            _LoanTransDetailsRepository = loanTransDetailsRepository;
        }

        public async Task<LoanDetails> SubmitData(Dictionary<string, string> fieldValues)
        {
            string loanid = fieldValues["LoanId"].ToString();

            return await Add(loanid, fieldValues);
        }

        private async Task<LoanDetails> Add(string loanid, Dictionary<string, string> fieldValues)
        {
            try
            {
                var objList = await _LoanDetailsRepository.GetAllAsync(x => !x.IsDelete && x.LoanId.ToString().ToLower() == loanid.ToLower(),
                    new Expression<Func<LoanDetails, object?>>[] { x => x.LoanTransDetails });
                if (objList.Count() > 0)
                {
                    var obj = objList.FirstOrDefault();
                    ObjectPopulator.PopulateObject<LoanDetails>(obj, fieldValues);
                    obj.IsActive = true;
                    obj.IsDelete = false;
                    obj.IsApproved = false;
                    obj.IsRejected = false;
                    obj.UpdatedBy = await _cytAdminService.GetUserName();
                    obj.UpdatedAt = DateTime.Now;
                    if (obj.LoanTransDetails != null)
                    {
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
                        objLoanTrans.LinkId = obj.Id;
                        obj.LoanTransDetails.Add(objLoanTrans);
                    }
                    else
                    {
                        obj.LoanTransDetails = new List<LoanTransDetails>();
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
                        objLoanTrans.LinkId = obj.Id;
                        obj.LoanTransDetails.Add(objLoanTrans);
                    }
                    await _LoanDetailsRepository.UpdateAsync(obj);
                    return obj;
                }
                else
                {
                    var obj = new LoanDetails();
                    ObjectPopulator.PopulateObject<LoanDetails>(obj, fieldValues);
                    obj.IsActive = true;
                    obj.IsDelete = false;
                    obj.IsApproved = false;
                    obj.IsRejected = false;
                    obj.CreatedAt = DateTime.Now;
                    obj.CreatedBy = await _cytAdminService.GetUserName();
                    obj.UpdatedBy = await _cytAdminService.GetUserName();
                    obj.UpdatedAt = DateTime.Now;
                    obj.LoanTransDetails = new List<LoanTransDetails>();

                    var objLoanTrans = new LoanTransDetails();
                    ObjectPopulator.CopyObject(obj, objLoanTrans);
                    objLoanTrans.Id = 0;
                    obj.LoanTransDetails.Add(objLoanTrans);
                    return await _LoanDetailsRepository.AddAsync(obj);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<LoanDetails>> GetList()
        {
            var username = await _cytAdminService.GetUserName();
            var userdetail = await _cytAdminService.GetUserDetails();

            var data = await _LoanDetailsRepository.GetAllAsync(x =>
                x.IsActive
                && !x.IsDelete
                && (
                    (userdetail.Roles.Contains("User") && x.CreatedBy == username) || (userdetail.Roles.Contains("Admin") && !x.IsRejected && !x.IsApproved)
                )
            );
            data = data.OrderBy(x => x.LoanId).ToList();
            return data.ToList();
        }

        public async Task<List<LoanTransDetails>> GetTransList(string q = "")
        {
            var data = new List<LoanTransDetails>();

            if (!string.IsNullOrEmpty(q))
            {
                var allData = await _LoanTransDetailsRepository.GetAllAsync();

                // Try to parse the user input as a DateTime (DDMMYYYY format)
                DateTime? parsedDate = null;
                if (DateTime.TryParseExact(q, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out var dateResult))
                {
                    parsedDate = dateResult;
                }

                data = allData
                    .Where(item =>
                    {
                        return item.GetType().GetProperties()
                            .Where(prop => prop.PropertyType == typeof(string) || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(DateTime))
                            .Any(prop =>
                            {
                                var value = prop.GetValue(item);

                                if (value is string stringValue)
                                {
                                    // Handle string fields (case-insensitive)
                                    return stringValue.Contains(q, StringComparison.OrdinalIgnoreCase);
                                }
                                else if (value is decimal decimalValue)
                                {
                                    // Handle decimal fields (convert the query to decimal if possible)
                                    if (decimal.TryParse(q, out var decimalQuery))
                                    {
                                        return decimalValue.ToString().Contains(decimalQuery.ToString());
                                    }
                                    return false;
                                }
                                else if (value is DateTime dateValue)
                                {
                                    // Handle DateTime fields (compare date parts only, without time)
                                    return parsedDate.HasValue && Convert.ToInt32(dateValue.ToString("yyyyMMdd")) == Convert.ToInt32(parsedDate.Value.ToString("yyyyMMdd"));
                                }

                                return false;
                            });
                    })
                    .ToList();
            }
            else
            {
                var allData = await _LoanTransDetailsRepository.GetAllAsync();
                data = allData.ToList();
            }
            data = data.OrderBy(x => x.UpdatedAt).ThenBy(x => x.CreatedAt).ToList();
            return data.Take(200).ToList();
        }

        public async Task<LoanDetails> GetData(int id)
        {
            var data = await _LoanDetailsRepository.GetByIdAsync(id);
            return data;
        }

        public async Task<LoanTransDetails> GetTransData(int id)
        {
            var data = await _LoanTransDetailsRepository.GetByIdAsync(id);
            return data;
        }

        public async Task<LoanDetails> GetEditData(int id)
        {
            var data = await GetData(id);
            return data;
        }

        public async Task<LoanCountInfo> GetInfo()
        {
            var username = await _cytAdminService.GetUserName();
            var userdetail = await _cytAdminService.GetUserDetails();
            var data = await _LoanDetailsRepository.GetAllAsync(x => (userdetail.Roles.Contains("User") && x.CreatedBy == username) || userdetail.Roles.Contains("Admin"));

            // Calculate counts
            var countDeleted = data.Count(x => x.IsDelete);
            var countApproved = data.Count(x => x.IsApproved);
            var countRejected = data.Count(x => x.IsRejected);
            var countPending = data.Count(x => !x.IsApproved && !x.IsRejected && !x.IsDelete);

            var DeletedAmount = data.Where(x => x.IsDelete).Sum(x => x.PrincipalAmount);
            var ApprovedAmount = data.Where(x => x.IsApproved).Sum(x => x.PrincipalAmount);
            var RejectedAmount = data.Where(x => x.IsRejected).Sum(x => x.PrincipalAmount);
            var PendingAmount = data.Where(x => !x.IsApproved && !x.IsRejected && !x.IsDelete).Sum(x => x.PrincipalAmount);

            // Create a result object to hold the data and counts
            var result = new LoanCountInfo()
            {
                Records = data,
                CountDeleted = countDeleted,
                CountApproved = countApproved,
                CountRejected = countRejected,
                CountPending = countPending,
                DeletedAmount = DeletedAmount,
                ApprovedAmount = ApprovedAmount,
                RejectedAmount = RejectedAmount,
                PendingAmount = PendingAmount
            };
            return result;
        }

        // Additional methods specific to DynamicForm
    }
}