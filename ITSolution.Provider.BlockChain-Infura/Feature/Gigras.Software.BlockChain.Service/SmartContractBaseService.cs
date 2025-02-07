using Gigras.Software.BlockChain.Service.Model;
using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.General.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System.Numerics;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gigras.Software.BlockChain.Service
{
    public class SmartContractBaseService
    {
        private readonly ILogger<SmartContractBaseService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ISmartContractAbiService _smartContractAbiService;
        private readonly ISmartContractAddressService _smartContractAddressService;

        public readonly string _inFuraUrl;
        public readonly string _privateKey;
        public string _publicKey;

        public string _contractAbi;
        public string _contractAddress;
        public Web3 _web3;
        public Contract _contract;
        public Account _account;

        public SmartContractBaseService(
                ILogger<SmartContractBaseService> logger,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration configuration,
                ISmartContractAbiService smartContractAbiService,
                ISmartContractAddressService smartContractAddressService
            )
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _smartContractAbiService = smartContractAbiService;
            _smartContractAddressService = smartContractAddressService;

            var configurationSection = _configuration.GetSection("InfuraSetting");
            _inFuraUrl = configurationSection["InFuraUrl"];
            _privateKey = configurationSection["PrivateKey"];
        }

        public async Task<ResponseModel> PrepareTransaction(TransactionRequestModel request)
        {
            try
            {
                // Convert Ether to Wei
                var valueInWei = new HexBigInteger(request.AmountInEther.ToString());

                // Load the contract
                var contract = _web3.Eth.GetContract(_contractAbi, _contractAddress);

                // Get the function you want to call (e.g., fundContract)
                var function = contract.GetFunction(request.FunctionName);

                // Encode the function call data
                var data = function.GetData();

                // Estimate gas
                var gasLimit = await _web3.Eth.Transactions.EstimateGas.SendRequestAsync(new CallInput
                {
                    From = request.SenderAddress,
                    To = _contractAddress,
                    Value = new HexBigInteger(valueInWei),
                    Data = data
                });

                // Get the current gas price
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();

                // Return the prepared transaction data
                return new ResponseModel()
                {
                    Status = true,
                    Result = new
                    {
                        From = request.SenderAddress,
                        To = _contractAddress,
                        Gas = gasLimit.Value.ToString(),
                        GasPrice = gasPrice.Value.ToString(),
                        Value = valueInWei.ToString(),
                        Data = data
                    }
                };
            }
            catch (System.Exception ex)
            {
                return new ResponseModel() { Status = false, Message = ex.Message };
            }
        }

        public async Task InitializeAsync(bool IsRequiredTrasaction = false)
        {
            try
            {
                // Fetch contract ABI and address
                var abiEntity = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var addressEntity = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abiEntity == null || addressEntity == null)
                {
                    var response = new ResponseModel() { Status = false, Message = "Smart contract ABI or address is not configured properly." };
                    _logger.LogError($"Failed to initialize SmartContract: {JsonConvert.SerializeObject(response)}");
                }

                var abi = abiEntity!.Abi!;
                var address = addressEntity!.ContractAddress!;

                _contractAbi = JsonDocument.Parse(abi).RootElement.GetRawText();
                _contractAddress = address;

                // Initialize Web3 and contract
                if (!IsRequiredTrasaction)
                {
                    _web3 = new Web3(_inFuraUrl);
                    _web3.TransactionManager.UseLegacyAsDefault = true;
                }
                else
                {
                    _account = new Account(_privateKey);
                    _web3 = new Web3(_account, _inFuraUrl);
                }
                _contract = _web3.Eth.GetContract(_contractAbi, _contractAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize LoanProvider: {ex.Message}");
            }
        }

        public async Task<ResponseModel> GetPublicKey()
        {
            var response = new ResponseModel();
            try
            {
                var ecKey = new EthECKey(_privateKey);
                string publicKeyHex = ecKey.GetPubKey().ToHex(true); // Compressed format
                string publicKeyUncompressedHex = ecKey.GetPubKey(false).ToHex(false); // Uncompressed format
                string address = ecKey.GetPublicAddress();
                _publicKey = publicKeyHex;
                response = new ResponseModel() { Status = true, Result = publicKeyHex, Final = publicKeyUncompressedHex };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> GetContractOwnerAddressAsync()
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("lender");
                var output = await Function.CallAsync<string>();
                response = new ResponseModel() { Status = true, Result = output };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> GetWalletAsync()
        {
            var response = new ResponseModel();
            try
            {
                var result = await _web3.Eth.GetBalance.SendRequestAsync(this._contractAddress);
                var output = Web3.Convert.FromWei(result.Value);

                response = new ResponseModel() { Status = true, Result = result.Value, Final = output };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> GetBalanceAsync(string address)
        {
            var response = new ResponseModel();
            try
            {
                var result = await _web3.Eth.GetBalance.SendRequestAsync(address);
                var output = Web3.Convert.FromWei(result.Value);

                response = new ResponseModel() { Status = true, Result = output, Final = output };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> GetContractBalanceAsync()
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("checkBalanceOfSmartContract");
                var output = await Function.CallAsync<int>();
                response = new ResponseModel() { Status = true, Result = output, Final = output };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> GetBorrowBalanceAsync(string borroweraddress)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("getBorrowerBalance");
                var output = await Function.CallAsync<BigInteger>(borroweraddress);
                var balanceInEther = Web3.Convert.FromWei(output);
                response = new ResponseModel() { Status = true, Result = output, Final = balanceInEther };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> GetLennderBalanceAsync(string lenderaddress)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("getLenderBalance");

                // Set the lender's address as the 'from' parameter (only the lender is allowed to call this)
                var callInput = new CallInput
                {
                    From = lenderaddress, // Set the lender's address
                    To = _contractAddress, // The contract address
                    Data = Function.GetData() // Encoded function data
                };

                var hexResult = await _web3.Eth.Transactions.Call.SendRequestAsync(callInput);

                if (!string.IsNullOrEmpty(hexResult))
                {
                    var hexBigInt = new HexBigInteger(hexResult);

                    // Convert the balance from Wei to Ether (if the balance is in Wei)
                    decimal output = Web3.Convert.FromWei(hexBigInt.Value);
                    response = new ResponseModel() { Status = true, Result = output, Final = Math.Round(output, 4) };
                    return response;
                }
                response = new ResponseModel() { Status = false, Message = "No hexResult found" };
                return response;
            }
            catch (SmartContractRevertException ex)
            {
                // Handle the revert exception here (you can log it or throw it as needed)
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> AddContractFundAsync(string lenderAddress, int amountInEther)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("fundContract");
                // Convert Ether amount to Wei
                var valueInWei = amountInEther;

                // Get current gas price and estimate gas limit
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasLimit = await Function.EstimateGasAsync(
                     from: lenderAddress,    // Replace with your wallet address
                     gas: gasPrice, // Set an appropriate gas limit
                     value: new HexBigInteger(valueInWei) // Ether amount in Wei
                );

                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Offline transaction signing
                var transactionInput = new TransactionInput
                {
                    From = lenderAddress,
                    To = _contractAddress,
                    Gas = new HexBigInteger(gasLimit),
                    GasPrice = new HexBigInteger(gasPrice),
                    Value = new HexBigInteger(valueInWei),
                    Data = Function.GetData() // Call fundContract()
                };
                var signedTransaction = await _account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the signed transaction
                var transactionHash = await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);

                response = new ResponseModel() { Status = true, Message = "Transaction successfully", Result = transactionHash, Final = transactionHash };

                return response; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response; // Return the transaction hash
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response; // Return the transaction hash
            }
        }

        public async Task<ResponseModel> AddContractFundDataAsync(string lenderAddress, int amountInEther)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("fundContract");
                // Convert Ether amount to Wei
                var valueInWei = amountInEther;

                // Get current gas price and estimate gas limit
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasLimit = await Function.EstimateGasAsync(
                     from: lenderAddress,    // Replace with your wallet address
                     gas: gasPrice, // Set an appropriate gas limit
                     value: new HexBigInteger(valueInWei) // Ether amount in Wei
                );

                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Offline transaction signing
                var transactionInput = new TransactionInput
                {
                    From = lenderAddress,
                    To = _contractAddress,
                    Gas = new HexBigInteger(gasLimit),
                    GasPrice = new HexBigInteger(gasPrice),
                    Value = new HexBigInteger(valueInWei),
                    Data = Function.GetData() // Call fundContract()
                };

                // Prepare the transaction data for external wallets
                var transactionData = new
                {
                    From = lenderAddress,
                    To = _contractAddress,
                    Gas = gasLimit.Value.ToString(),
                    GasPrice = gasPrice.ToString(),
                    Nonce = nonce.Value.ToString(),
                    Value = new HexBigInteger(valueInWei),
                    Data = Function.GetData() // The encoded function call
                };
                response = new ResponseModel
                {
                    Status = true,
                    Message = "Transaction data generated successfully. Please sign with your wallet.",
                    Result = transactionData
                };
                return response; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response; // Return the transaction hash
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response; // Return the transaction hash
            }
        }

        public async Task<ResponseModel> WithdrawalContractFundAsync(string lenderAddress, decimal amountToWithdraw)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("takeOutContractFunds");

                // Estimate the gas for the transaction
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasLimit = await Function.EstimateGasAsync(
                                     from: lenderAddress,    // Replace with your wallet address
                                     gas: gasPrice, // Set an appropriate gas limit
                                     value: new HexBigInteger(0),
                                     functionInput: new object[] { amountToWithdraw }
                                );
                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Sign and send the transaction
                var transactionInput = Function.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasLimit),
                    gasPrice: null, // Optional: Specify a gas price or let it use the default
                    value: null, // No Ether is sent to the function
                    functionInput: new object[] { amountToWithdraw }
                );
                var signedTransaction = await _account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the raw transaction
                var transactionHash = await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
                response = new ResponseModel() { Status = true, Message = "Transaction successfully", Result = transactionHash, Final = transactionHash };

                return response;
            }
            catch (SmartContractRevertException ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> WithdrawalContractFundDataAsync(string lenderAddress, decimal amountToWithdraw)
        {
            var response = new ResponseModel();

            try
            {
                // Get the function from the contract
                var function = _contract.GetFunction("takeOutContractFunds");

                // Estimate the gas for the transaction
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasLimit = await function.EstimateGasAsync(
                    from: lenderAddress, // Sender's wallet address
                    gas: null,
                    value: new HexBigInteger(0), // No value sent with this call
                    functionInput: new object[] { amountToWithdraw }
                );

                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction data for external signing
                var transactionInput = function.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasLimit.Value),
                    gasPrice: gasPrice,
                    value: new HexBigInteger(0), // No Ether sent
                    functionInput: new object[] { amountToWithdraw }
                );

                // Prepare the transaction data for external wallets
                var transactionData = new
                {
                    From = transactionInput.From,
                    To = transactionInput.To,
                    Gas = transactionInput.Gas.Value.ToString(),
                    GasPrice = transactionInput.GasPrice.Value.ToString(),
                    Nonce = nonce.Value.ToString(),
                    Data = transactionInput.Data,
                    Value = transactionInput.Value?.Value.ToString() ?? "0"
                };

                // Return the transaction data to the client for signing via external wallet
                response = new ResponseModel
                {
                    Status = true,
                    Message = "Transaction data generated successfully. Please sign with your wallet.",
                    Result = transactionData
                };

                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel
                {
                    Status = false,
                    Message = ex.Message
                };
                return response;
            }
        }

        public async Task<ResponseModel> ChangeContractOwnerAsync(string lenderAddress, string newLenderAddress)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("changeLender");

                // Estimate gas for the transaction
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasLimit = await Function.EstimateGasAsync(
                                     from: lenderAddress,    // Replace with your wallet address
                                     gas: gasPrice, // Set an appropriate gas limit
                                     value: new HexBigInteger(0),
                                     functionInput: new object[] { newLenderAddress }
                                );

                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasLimit),
                    value: new HexBigInteger(0),
                    functionInput: new object[] { newLenderAddress } // Input: the new lender's address
                );

                var signedTransaction = await _account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the raw transaction
                var transactionHash = await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
                response = new ResponseModel() { Status = true, Message = "Transaction successfully", Result = transactionHash, Final = transactionHash };

                return response;
            }
            catch (SmartContractRevertException ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> ChangeContractOwnerDataAsync(string lenderAddress, string newLenderAddress)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("changeLender");

                // Estimate gas for the transaction
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasLimit = await Function.EstimateGasAsync(
                                     from: lenderAddress,    // Replace with your wallet address
                                     gas: gasPrice, // Set an appropriate gas limit
                                     value: new HexBigInteger(0),
                                     functionInput: new object[] { newLenderAddress }
                                );

                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasLimit),
                    value: new HexBigInteger(0),
                    functionInput: new object[] { newLenderAddress } // Input: the new lender's address
                );

                // Prepare the transaction data
                var transactionData = new
                {
                    From = lenderAddress,
                    To = _contractAddress,
                    Gas = gasLimit.Value.ToString(),
                    GasPrice = gasPrice.Value.ToString(),
                    Nonce = nonce.Value.ToString(),
                    Value = "0", // No Ether is sent
                    Data = Function.GetData(new object[] { newLenderAddress }) // Encoded function call
                };
                response = new ResponseModel
                {
                    Status = true,
                    Message = "Transaction data generated successfully. Please sign and send it with an external wallet.",
                    Result = transactionData
                };

                return response;
            }
            catch (SmartContractRevertException ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }
    }
}