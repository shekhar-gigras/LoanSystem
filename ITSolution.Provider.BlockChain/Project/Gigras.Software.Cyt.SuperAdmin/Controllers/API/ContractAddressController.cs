using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.General.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Transactions;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.API
{
    [ApiController]
    [Route("api/contractaddress")]
    public class ContractAddressController : Controller
    {
        private readonly ISmartContractAddressService _smartContractAddressService;
        private readonly ICytAdminService _cytAdminService;

        public ContractAddressController(ISmartContractAddressService smartContractAddressService, ICytAdminService cytAdminService)
        {
            _smartContractAddressService = smartContractAddressService;
            _cytAdminService = cytAdminService;
        }

        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _smartContractAddressService.GetByIdAsync(id);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsDelete = !obj.IsDelete;
                    obj.UpdatedAt = DateTime.UtcNow;
                    obj.UpdatedBy = await _cytAdminService.GetUserName();
                    await _smartContractAddressService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("active/{id:int}")]
        [HttpPost]
        public async Task<IActionResult> Active(int id, [FromBody] StatusUpdateRequest statusUpdateRequest)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var objList = await _smartContractAddressService.GetAllAsync();
                    foreach (var item in objList)
                    {
                        item.IsActive = false;
                        item.UpdatedAt = DateTime.UtcNow;
                        item.UpdatedBy = await _cytAdminService.GetUserName();
                        await _smartContractAddressService.UpdateAsync(item);
                    }
                    var obj = await _smartContractAddressService.GetByIdAsync(id);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsActive = !obj.IsActive;
                    await _smartContractAddressService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }
    }
}