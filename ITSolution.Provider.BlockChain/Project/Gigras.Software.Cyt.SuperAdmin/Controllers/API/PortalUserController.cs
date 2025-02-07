using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Cyt.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Transactions;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.API
{
    [ApiController]
    [Route("api/portaluser")]
    [Authorize(Roles = "Admin")] // Specify multiple roles here
    public class PortalUserController : Controller
    {
        private readonly ICytAdminService _cytAdminService;

        public PortalUserController(ICytAdminService cytAdminService)
        {
            _cytAdminService = cytAdminService;
        }

        [Route("active")]
        [HttpPost]
        public async Task<IActionResult> Active(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _cytAdminService.GetByIdAsync(0, x => x.UserId.ToString() == request.recordid);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsActive = !obj.IsActive;
                    await _cytAdminService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _cytAdminService.GetByIdAsync(0, x => x.UserId.ToString() == request.recordid);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsDelete = !obj.IsDelete;
                    await _cytAdminService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("block")]
        [HttpPost]
        public async Task<IActionResult> Block(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _cytAdminService.GetByIdAsync(0, x => x.UserId.ToString() == request.recordid);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsBlock = !obj.IsBlock;
                    await _cytAdminService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("addloanstatus")]
        [HttpPost]
        public async Task<IActionResult> AddLoan(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _cytAdminService.GetByIdAsync(0, x => x.UserId.ToString() == request.recordid);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsAddLoan = !obj.IsAddLoan;
                    await _cytAdminService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("editloanstatus")]
        [HttpPost]
        public async Task<IActionResult> EditLoan(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _cytAdminService.GetByIdAsync(0, x => x.UserId.ToString() == request.recordid);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsEditLoan = !obj.IsEditLoan;
                    await _cytAdminService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }
        [Route("deleteloanstatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteLoan(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _cytAdminService.GetByIdAsync(0, x => x.UserId.ToString() == request.recordid);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsDeleteLoan = !obj.IsDeleteLoan;
                    await _cytAdminService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("visibleloanstatus")]
        [HttpPost]
        public async Task<IActionResult> VisibleLoanSttaus(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _cytAdminService.GetByIdAsync(0, x => x.UserId.ToString() == request.recordid);
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    obj.IsVisibleLoanSale = !obj.IsVisibleLoanSale;
                    await _cytAdminService.UpdateAsync(obj);
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