using Gigras.Software.Cyt.Services.CytServcies;
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

        public EthereumController(ISmartContractAbiService smartContractAbiService,
            ISmartContractAddressService smartContractAddressService,
            IDynamicUserDataService dynamicUserDataService)
        {
            _smartContractAbiService = smartContractAbiService;
            _smartContractAddressService = smartContractAddressService;
            _dynamicUserDataService = dynamicUserDataService;
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
    }

    public class RequestBorrows
    {
        public List<string>? RequestedBorrowers { get; set; }
        public List<string>? ActiveBorrowers { get; set; }
    }
}