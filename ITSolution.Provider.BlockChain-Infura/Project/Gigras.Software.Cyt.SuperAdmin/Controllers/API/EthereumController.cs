using Gigras.Software.BlockChain.Service;
using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.General.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gigras.Software.Cyt.SuperAdmin.Controllers.API
{
    [ApiController]
    [Route("api/ethereum")]
    public class EthereumController : ControllerBase
    {
        private readonly ISmartContractAbiService _smartContractAbiService;
        private readonly ISmartContractAddressService _smartContractAddressService;
        private readonly IDynamicUserDataService _dynamicUserDataService;
        private readonly ISmartContractService _smartContractService;

        public EthereumController(ISmartContractAbiService smartContractAbiService,
            ISmartContractAddressService smartContractAddressService,
            ISmartContractService smartContractService,
            IDynamicUserDataService dynamicUserDataService)
        {
            _smartContractAbiService = smartContractAbiService;
            _smartContractAddressService = smartContractAddressService;
            _dynamicUserDataService = dynamicUserDataService;
            _smartContractService = smartContractService;
        }

        [HttpPost("borrower-lists")]
        public async Task<IActionResult> GetBorrowLists(RequestBorrows requestBorrows)
        {
            try
            {
                if (requestBorrows.RequestedBorrowers != null && requestBorrows.RequestedBorrowers.Count() > 0)
                {
                    var obj = await _dynamicUserDataService.GetAllAsync(x => x.IsActive && !x.IsDelete && requestBorrows.RequestedBorrowers!.Contains(x.MetaMaskID!));
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Handle circular references
                    };
                    string json = JsonConvert.SerializeObject(obj, settings);
                    return this.Ok(json);
                }
                else if (requestBorrows.ActiveBorrowers != null && requestBorrows.ActiveBorrowers.Count() > 0)
                {
                    var obj = await _dynamicUserDataService.GetAllAsync(x => x.IsActive && !x.IsDelete && requestBorrows.ActiveBorrowers!.Contains(x.MetaMaskID!));
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Handle circular references
                    };
                    string json = JsonConvert.SerializeObject(obj, settings);
                    return this.Ok(json);
                }
                return this.Ok();
            }
            catch (Exception ex)
            {
                return this.Ok(ex.Message);
            }
        }

        [HttpGet("abi")]
        public async Task<IActionResult> GetAbiDetail()
        {
            var abi = await _smartContractAbiService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            return this.Ok(abi.FirstOrDefault());
        }

        [HttpGet("contract")]
        public async Task<IActionResult> GetContractDetail()
        {
            var address = await _smartContractAddressService.GetAllAsync(x => x.IsActive && !x.IsDelete);
            return this.Ok(address.FirstOrDefault());
        }

        [HttpGet("wallet-balance")]
        public async Task<IActionResult> WalletBalance()
        {
            await _smartContractService.InitializeAsync();
            var response = await _smartContractService.GetWalletAsync();
            return this.Ok(response);
        }

        [HttpPost("add-wallet-fund")]
        public async Task<IActionResult> AddWalletFund(RequestModel request)
        {
            await _smartContractService.InitializeAsync(true);
            var response = await _smartContractService.AddContractFundDataAsync(request.SenderAddress!, request.AmountInEther!.Value);
            return this.Ok(response);
        }

        [HttpPost("change-lender")]
        public async Task<IActionResult> ChangeLender(RequestModel request)
        {
            await _smartContractService.InitializeAsync();
            var response = await _smartContractService.ChangeContractOwnerDataAsync(request.SenderAddress!, request.RequestAddress!);
            return this.Ok(response);
        }

        [HttpPost("prepare-transaction")]
        public async Task<IActionResult> PrepareTransaction(TransactionRequestModel request)
        {
            await _smartContractService.InitializeAsync();
            var response = await _smartContractService.PrepareTransaction(request);
            return Ok(response);
        }
    }
}