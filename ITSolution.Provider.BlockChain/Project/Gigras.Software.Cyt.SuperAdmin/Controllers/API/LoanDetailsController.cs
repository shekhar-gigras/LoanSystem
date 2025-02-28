using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Cyt.ViewModel;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.General.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;
using System.Transactions;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.API
{
    [ApiController]
    [Route("api/loandetails")]
    [Authorize(Roles = "Admin,Lender")] // Specify multiple roles here
    public class LoanDetailsController : Controller
    {
        private readonly ILoanDetailsService _loanDetailsService;
        private readonly ICytAdminService _cytAdminService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ILoanBuyInterestService _loanBuyInterestService;

        public LoanDetailsController(ILoanDetailsService loanDetailsService,
            ICytAdminService cytAdminService,
            IWebHostEnvironment webHostEnvironment, IConfiguration configuration,
            ILoanBuyInterestService loanBuyInterestService)
        {
            _loanDetailsService = loanDetailsService;
            _cytAdminService = cytAdminService;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _loanBuyInterestService = loanBuyInterestService;
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
        [Authorize(Roles = "Lender")] // Specify multiple roles here
        public async Task<ActionResult> Delete(string loanid)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var lenderid = await _cytAdminService.GetUserUniqueId();

                    var obj = await _loanDetailsService.GetByIdAsync(0, x => x.LendderId.ToString() == lenderid && x.LoanId.ToString().ToLower() == loanid.ToLower() && !x.IsDelete,
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
                        obj.LoanTransDetails.Add(objLoanTrans);
                    }
                    else
                    {
                        obj.LoanTransDetails = new List<LoanTransDetails>();
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
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
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<ActionResult> ApproveLoan(string loanid, string metamaskid)
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
                        obj.LoanTransDetails.Add(objLoanTrans);
                    }
                    else
                    {
                        obj.LoanTransDetails = new List<LoanTransDetails>();
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
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
        [Authorize(Roles = "Admin")] // Specify multiple roles here
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
                        obj.LoanTransDetails.Add(objLoanTrans);
                    }
                    else
                    {
                        obj.LoanTransDetails = new List<LoanTransDetails>();
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
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

        [Route("sell")]
        [HttpPost]
        [Authorize(Roles = "Lender")] // Specify multiple roles here
        public async Task<IActionResult> SellLoan(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var lenderid = await _cytAdminService.GetUserUniqueId();

                    var obj = await _loanDetailsService.GetByIdAsync(0, x => x.LendderId.ToString() == lenderid && x.LoanId.ToString().ToLower() == request.recordid!.ToLower() && !x.IsDelete,
                    new Expression<Func<LoanDetails, object?>>[] { x => x.LoanTransDetails });
                    if (obj == null)
                    {
                        return this.Ok(HttpStatusCode.NotFound);
                    }
                    var objAdd = new LoanDetails();
                    ObjectPopulator.CopyObject<LoanDetails>(obj, objAdd);

                    obj.IsLoanSell = !obj.IsLoanSell;
                    obj.IsApprovedTransferLoan = false;
                    obj.UpdatedAt = DateTime.UtcNow;
                    obj.UpdatedBy = await _cytAdminService.GetUserName();
                    if (obj.LoanTransDetails != null)
                    {
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
                        obj.LoanTransDetails.Add(objLoanTrans);
                    }
                    else
                    {
                        obj.LoanTransDetails = new List<LoanTransDetails>();
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
                        obj.LoanTransDetails.Add(objLoanTrans);
                    }
                    await _loanDetailsService.UpdateAsync(obj);
                    var users = await _cytAdminService.GetAllAsync(x => x.UserName!.ToLower() == "admin");
                    var objUser = users.FirstOrDefault()!;
                    string rootpath = _webHostEnvironment.ContentRootPath + "//wwwroot";
                    string template = await EmailHelper.ReadEmailTemplate(rootpath, "LoanSell.html", objUser.Name!, "", "Loan sell from " + objUser.Name!);
                    template = template.Replace("[USER]", await _cytAdminService.GetUserName());
                    template = template.Replace("[MESSAGE]", obj.IsLoanSell ? "Selling" : "Stop Selling");
                    template = template.Replace("[LOAN]", obj.LoanId.ToString());
                    await EmailHelper.SendEmailAsync(_configuration, objUser.Name!, objUser.Email!, "Loan sell from " + objUser.Name!, template);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("transfer-approved/{loanid}")]
        [HttpPost]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> TransferApprovedLoan(string loanid)
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

                    obj.IsApprovedTransferLoan = true;
                    obj.UpdatedAt = DateTime.UtcNow;
                    obj.UpdatedBy = await _cytAdminService.GetUserName();
                    if (obj.LoanTransDetails != null)
                    {
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
                        obj.LoanTransDetails.Add(objLoanTrans);
                    }
                    else
                    {
                        obj.LoanTransDetails = new List<LoanTransDetails>();
                        var objLoanTrans = new LoanTransDetails();
                        ObjectPopulator.CopyObject(obj, objLoanTrans);
                        objLoanTrans.Id = 0;
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

        [Route("loan-sell-intereset")]
        [HttpPost]
        [Authorize(Roles = "Lender")] // Specify multiple roles here
        public async Task<IActionResult> SellLoanInterest(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var objSeller = await _loanDetailsService.GetByIdAsync(0, x => x.LoanId.ToString() == request.recordid);
                    var lenderid = await _cytAdminService.GetUserUniqueId();
                    var loanBuy = new LoanBuyInterest();
                    loanBuy.LoanDetailId = objSeller!.Id;
                    loanBuy.LoanId = Guid.Parse(request.recordid!);
                    loanBuy.BuyerId = Guid.Parse(lenderid!);
                    loanBuy.SellerId = objSeller!.LendderId;
                    loanBuy.IsInterested = true;
                    loanBuy.CreatedBy = await _cytAdminService.GetUserName();
                    loanBuy.UpdatedBy = await _cytAdminService.GetUserName();
                    await _loanBuyInterestService.AddAsync(loanBuy);

                    var users = await _cytAdminService.GetAllAsync(x => x.UserName!.ToLower() == "admin");
                    var objUser = users.FirstOrDefault()!;
                    string rootpath = _webHostEnvironment.ContentRootPath + "//wwwroot";
                    string template = await EmailHelper.ReadEmailTemplate(rootpath, "LoanSellInterest.html", objUser.Name!, "", "Loan sell Interest from " + objUser.Name!);
                    template = template.Replace("[USER]", await _cytAdminService.GetUserName());
                    template = template.Replace("[MESSAGE]", loanBuy.IsInterested ? "Interested" : "Rejected");
                    template = template.Replace("[LOAN]", loanBuy.LoanId.ToString());
                    await EmailHelper.SendEmailAsync(_configuration, objUser.Name!, objUser.Email!, "Loan sell Interest from " + objUser.Name!, template);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("loan-sell-reject")]
        [HttpPost]
        [Authorize(Roles = "Lender")] // Specify multiple roles here
        public async Task<IActionResult> SellLoanReject(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var objSeller = await _loanDetailsService.GetByIdAsync(0, x => x.LoanId.ToString() == request.recordid);
                    var lenderid = await _cytAdminService.GetUserUniqueId();
                    var loanBuy = new LoanBuyInterest();
                    loanBuy.LoanDetailId = objSeller!.Id;
                    loanBuy.LoanId = Guid.Parse(request.recordid!);
                    loanBuy.BuyerId = Guid.Parse(lenderid!);
                    loanBuy.SellerId = objSeller!.LendderId;
                    loanBuy.IsDenied = true;
                    loanBuy.CreatedBy = await _cytAdminService.GetUserName();
                    loanBuy.UpdatedBy = await _cytAdminService.GetUserName();
                    await _loanBuyInterestService.AddAsync(loanBuy);

                    var users = await _cytAdminService.GetAllAsync(x => x.UserName!.ToLower() == "admin");
                    var objUser = users.FirstOrDefault()!;
                    string rootpath = _webHostEnvironment.ContentRootPath + "//wwwroot";
                    string template = await EmailHelper.ReadEmailTemplate(rootpath, "LoanSellInterest.html", objUser.Name!, "", "Loan sell Interest from " + objUser.Name!);
                    template = template.Replace("[USER]", await _cytAdminService.GetUserName());
                    template = template.Replace("[MESSAGE]", loanBuy.IsInterested ? "Interested" : "Rejected");
                    template = template.Replace("[LOAN]", loanBuy.LoanId.ToString());
                    await EmailHelper.SendEmailAsync(_configuration, objUser.Name!, objUser.Email!, "Loan sell Interest from " + objUser.Name!, template);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("sell-buy-interest-approved")]
        [HttpPost]
        [Authorize(Roles = "Lender")] // Specify multiple roles here
        public async Task<IActionResult> SellLoanInterestApproved(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var lenderid = await _cytAdminService.GetUserUniqueId();

                    var objSeller = await _loanBuyInterestService.GetByIdAsync(Convert.ToInt32(request.recordid), x => x.SellerId.ToString() == lenderid && !x.IsAdminApproved);
                    var objSellers = await _loanBuyInterestService.GetAllAsync(x => x.Id != Convert.ToInt32(request.recordid) && x.LoanId == objSeller!.LoanId && x.SellerId.ToString() == lenderid && !x.IsClosedSellerLoan && !x.IsAdminApproved);
                    if (objSeller != null)
                    {
                        foreach (var item in objSellers)
                        {
                            item.IsSellerApproved = false;
                            item.UpdatedBy = await _cytAdminService.GetUserName();
                            item.UpdatedAt = DateTime.Now;
                            await _loanBuyInterestService.UpdateAsync(item);
                        }
                    }

                    objSeller!.IsSellerApproved = true;
                    objSeller.UpdatedBy = await _cytAdminService.GetUserName();
                    objSeller.UpdatedAt = DateTime.Now;
                    await _loanBuyInterestService.UpdateAsync(objSeller);

                    var users = await _cytAdminService.GetAllAsync(x => x.UserName!.ToLower() == "admin");
                    var objUser = users.FirstOrDefault()!;
                    string rootpath = _webHostEnvironment.ContentRootPath + "//wwwroot";
                    string template = await EmailHelper.ReadEmailTemplate(rootpath, "LoanSellInterest.html", objUser.Name!, "", "Seller approved the Loan for buying from " + objUser.Name!);
                    template = template.Replace("[USER]", await _cytAdminService.GetUserName());
                    template = template.Replace("[MESSAGE]", "Approved");
                    template = template.Replace("[LOAN]", objSeller.LoanId.ToString());
                    await EmailHelper.SendEmailAsync(_configuration, objUser.Name!, objUser.Email!, "Seller approved the Loan for buying from " + objUser.Name!, template);
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("admin-sell-buy-interest-approved")]
        [HttpPost]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> AdminLoanInterestApproved(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var objSeller = await _loanBuyInterestService.GetByIdAsync(Convert.ToInt32(request.recordid));
                    var objSellers = await _loanBuyInterestService.GetAllAsync(x => x.Id != Convert.ToInt32(request.recordid) && x.LoanId == objSeller!.LoanId);
                    if (objSeller != null)
                    {
                        foreach (var item in objSellers)
                        {
                            item.IsDelete = true;
                            objSeller!.IsSellerApproved = false;
                            item.IsClosedSellerLoan = true;
                            item.UpdatedBy = await _cytAdminService.GetUserName();
                            item.UpdatedAt = DateTime.Now;
                            await _loanBuyInterestService.UpdateAsync(item);
                        }
                    }
                    objSeller!.DealAmount = request.dealAmount;
                    objSeller!.Comments = request.comments;
                    objSeller!.IsSellerApproved = true;
                    objSeller!.IsAdminApproved = true;
                    objSeller!.IsClosedSellerLoan = true;
                    objSeller.UpdatedBy = await _cytAdminService.GetUserName();
                    objSeller.UpdatedAt = DateTime.Now;
                    await _loanBuyInterestService.UpdateAsync(objSeller);
                    var objLoan = await _loanDetailsService.GetByIdAsync(0, x => x.LoanId == objSeller.LoanId);
                    objLoan!.DealAmount = request.dealAmount;
                    objLoan!.Comments = request.comments;
                    objLoan!.IsApprovedTransferLoan = true;
                    objLoan!.IsLoanSell = false;
                    objLoan.LendderId = objSeller.BuyerId;
                    await _loanDetailsService.UpdateAsync(objLoan);

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return this.Ok(HttpStatusCode.NotFound);
            }
            return this.Ok(HttpStatusCode.OK);
        }

        [Route("admin-sell-buy-interest-reject")]
        [HttpPost]
        [Authorize(Roles = "Admin")] // Specify multiple roles here
        public async Task<IActionResult> AdminLoanInterestRejected(ApiRequestModel request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var objSeller = await _loanBuyInterestService.GetByIdAsync(Convert.ToInt32(request.recordid));
                    objSeller!.IsDenied = true;
                    objSeller!.IsSellerApproved = false;
                    objSeller.UpdatedBy = await _cytAdminService.GetUserName();
                    objSeller.UpdatedAt = DateTime.Now;
                    await _loanBuyInterestService.UpdateAsync(objSeller);
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