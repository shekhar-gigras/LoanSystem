using Gigras.Software.BlockChain.Service;
using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.Cyt.SuperAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers
{
    [Route("sadmin")]
    [Authorize(Roles = "Admin,User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDynamicFormService _dynamicFormService;
        private readonly ILoanDetailsService _loanDetailsService;
        private readonly ILoanProvider _loanProvider;
        private readonly ISmartContractService _smartContractService;
        private readonly ISmartContractBorrowerService _smartContractBorrowerService;

        public HomeController(IDynamicFormService dynamicFormService,
            ILogger<HomeController> logger,
            ILoanDetailsService loanDetailsService,
            ILoanProvider loanProvider,
            ISmartContractService smartContractService,
            ISmartContractBorrowerService smartContractBorrowerService
            )
        {
            _dynamicFormService = dynamicFormService;
            _logger = logger;
            _loanDetailsService = loanDetailsService;
            _loanProvider = loanProvider;
            _smartContractService = smartContractService;
            _smartContractBorrowerService = smartContractBorrowerService;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var data = await _loanDetailsService.GetInfo();
            await _smartContractService.InitializeAsync(true);
            var response = await _smartContractService.GetContractOwnerAddressAsync();
            data.LenderAddress = response.Status ? response.Result!.ToString() : string.Empty;
            if (!string.IsNullOrEmpty(data.LenderAddress))
            {
                response = await _smartContractService.GetLennderBalanceAsync(data.LenderAddress!);
                data.LenderBalance = response.Status ? Convert.ToDecimal(response.Result!) : 0;
            }
            response = await _smartContractService.GetWalletAsync();
            data.WalletBalance = response.Status ? Convert.ToDecimal(response.Result!.ToString()) : 0;

            ViewBag.Forms = await _dynamicFormService.GetAllAsync(x => x.IsActive && !x.IsDelete, y => y.Country!, y => y.State!, y => y.City!);
            return View(data);
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}