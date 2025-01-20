using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.General.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;
using System.Transactions;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.API
{
    [ApiController]
    [Route("api/loandetails")]
    public class LoanDetailsController : Controller
    {
        private readonly ILoanDetailsService _loanDetailsService;
        private readonly ICytAdminService _cytAdminService;

        public LoanDetailsController(ILoanDetailsService loanDetailsService, ICytAdminService cytAdminService)
        {
            _loanDetailsService = loanDetailsService;
            _cytAdminService = cytAdminService;
        }

        [Route("{loanid}")]
        [HttpGet]
        public async Task<ActionResult<LoanDetails>> GetLoanData(string loanid)
        {
            var obj = await _loanDetailsService.GetByIdAsync(0, x => x.LoanId.ToString().ToLower() == loanid.ToLower());
            return this.Ok(obj);
        }

        [Route("{loanid}")]
        [HttpPost]
        public async Task<ActionResult> Delete(string loanid)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _loanDetailsService.GetByIdAsync(0, x => x.LoanId.ToString().ToLower() == loanid.ToLower() && !x.IsDelete,
                    new Expression<Func<LoanDetails, object?>>[] { x => x.LoanTransDetails });
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    var objAdd = new LoanDetails();
                    ObjectPopulator.CopyObject<LoanDetails>(obj, objAdd);

                    obj.IsDelete = true;
                    obj.UpdatedAt = DateTime.UtcNow;
                    obj.UpdatedBy = await _cytAdminService.GetUserName();

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
                    await _loanDetailsService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("{loanid}/{metamaskid}")]
        [HttpPost]
        public async Task<ActionResult> ApproveLoan(string loanid,string metamaskid)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _loanDetailsService.GetByIdAsync(0, x => x.LoanId.ToString().ToLower() == loanid.ToLower() && !x.IsDelete,
                    new Expression<Func<LoanDetails, object?>>[] { x => x.LoanTransDetails });
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    var objAdd = new LoanDetails();
                    ObjectPopulator.CopyObject<LoanDetails>(obj, objAdd);
                    obj.MetaMaskID = metamaskid;
                    obj.IsApproved = true;
                    obj.UpdatedAt = DateTime.UtcNow;
                    obj.UpdatedBy = await _cytAdminService.GetUserName();
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
                    await _loanDetailsService.UpdateAsync(obj);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("{loanid}")]
        [HttpPut]
        public async Task<IActionResult> RejectLoan(string loanid)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var obj = await _loanDetailsService.GetByIdAsync(0, x => x.LoanId.ToString().ToLower() == loanid.ToLower() && !x.IsDelete,
                    new Expression<Func<LoanDetails, object?>>[] { x => x.LoanTransDetails });
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    var objAdd = new LoanDetails();
                    ObjectPopulator.CopyObject<LoanDetails>(obj, objAdd);

                    obj.IsRejected = true;
                    obj.UpdatedAt = DateTime.UtcNow;
                    obj.UpdatedBy = await _cytAdminService.GetUserName();
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
                    await _loanDetailsService.UpdateAsync(obj);

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