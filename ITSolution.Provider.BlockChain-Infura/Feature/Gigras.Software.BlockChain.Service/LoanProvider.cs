using Gigras.Software.BlockChain.Service.Model;
using Gigras.Software.Cyt.Services.CytServcies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Text.Json;

namespace Gigras.Software.BlockChain.Service
{
    public class LoanProvider : ILoanProvider
    {
        private readonly ILogger<LoanProvider> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _configurationSection;
        private readonly ISmartContractAbiService _smartContractAbiService;
        private readonly ISmartContractAddressService _smartContractAddressService;

        private readonly string _inFuraUrl;
        private readonly string _privateKey;

        public LoanProvider(ILogger<LoanProvider> logger,
             IWebHostEnvironment webHostEnvironment, IConfiguration configuration,
            ISmartContractAbiService smartContractAbiService,
             ISmartContractAddressService smartContractAddressService
            )
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _smartContractAbiService = smartContractAbiService;
            _smartContractAddressService = smartContractAddressService;
            _configurationSection = _configuration.GetSection("InfuraSetting");
            _inFuraUrl = _configurationSection["InFuraUrl"];
            _privateKey = _configurationSection["PrivateKey"];
        }

        public async Task<string> GetSmartContractOwnerAddress()
        {
            try
            {
                var Abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddres = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var web3 = new Web3(_inFuraUrl);
                Abi!.Abi = JsonDocument.Parse(Abi.Abi!).RootElement.GetRawText();

                var contract = web3.Eth.GetContract(Abi!.Abi, smartContractAddres!.ContractAddress);
                var lenderFunction = contract.GetFunction("lender");
                var lenderAddress = await lenderFunction.CallAsync<string>();
                return lenderAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return string.Empty;
        }

        public async Task<int> GetSmartContractBalance()
        {
            try
            {
                var Abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddres = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var web3 = new Web3(_inFuraUrl);
                Abi!.Abi = JsonDocument.Parse(Abi.Abi!).RootElement.GetRawText();

                var contract = web3.Eth.GetContract(Abi!.Abi, smartContractAddres!.ContractAddress);
                var lenderFunction = contract.GetFunction("checkBalanceOfSmartContract");
                var balance = await lenderFunction.CallAsync<int>();
                return balance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return -1;
        }

        public async Task<decimal> GetLenderBalance(string lenderAddress)
        {
            try
            {
                var Abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddres = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var web3 = new Web3(_inFuraUrl);

                Abi!.Abi = JsonDocument.Parse(Abi.Abi!).RootElement.GetRawText();

                var contract = web3.Eth.GetContract(Abi!.Abi, smartContractAddres!.ContractAddress);
                var lenderFunction = contract.GetFunction("getLenderBalance");

                // Set the lender's address as the 'from' parameter (only the lender is allowed to call this)
                var callInput = new CallInput
                {
                    From = lenderAddress, // Set the lender's address
                    To = smartContractAddres.ContractAddress, // The contract address
                    Data = lenderFunction.GetData() // Encoded function data
                };

                var hexResult = await web3.Eth.Transactions.Call.SendRequestAsync(callInput);

                if (!string.IsNullOrEmpty(hexResult))
                {
                    var hexBigInt = new HexBigInteger(hexResult);

                    // Convert the balance from Wei to Ether (if the balance is in Wei)
                    decimal balanceInEther = Web3.Convert.FromWei(hexBigInt.Value);
                    return balanceInEther;
                }
            }
            catch (SmartContractRevertException ex)
            {
                // Handle the revert exception here (you can log it or throw it as needed)
                Console.WriteLine($"Smart contract error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return 0m; // Return 0 if any error occurs
        }

        public async Task<string> FundContractAsync(string lenderAddress, int amountInEther)
        {
            try
            {
                // Create an account using the private key
                var account = new Account(_privateKey);
                var web3 = new Web3(account, _inFuraUrl);
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Retrieve the smart contract ABI and address
                var abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddress = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abi == null || smartContractAddress == null)
                {
                    throw new Exception("Smart contract ABI or address not found.");
                }

                // Parse the ABI
                abi.Abi = JsonDocument.Parse(abi.Abi!).RootElement.GetRawText();

                // Access the smart contract and the "fundContract" function
                var contract = web3.Eth.GetContract(abi.Abi, smartContractAddress.ContractAddress);
                var fundContractFunction = contract.GetFunction("fundContract");

                // Convert Ether amount to Wei
                var valueInWei = amountInEther;// Web3.Convert.ToWei(amountInEther);

                // Get current gas price and estimate gas limit
                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                var gasLimit = await fundContractFunction.EstimateGasAsync(
                     from: lenderAddress,    // Replace with your wallet address
                     gas: new Nethereum.Hex.HexTypes.HexBigInteger(200000), // Set an appropriate gas limit
                     value: new HexBigInteger(valueInWei) // Ether amount in Wei
                );

                // Get the nonce for the sender's address
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Offline transaction signing
                var transactionInput = new TransactionInput
                {
                    From = lenderAddress,
                    To = smartContractAddress.ContractAddress,
                    Gas = new HexBigInteger(gasLimit),
                    GasPrice = new HexBigInteger(gasPrice),
                    Value = new HexBigInteger(valueInWei),
                    Data = fundContractFunction.GetData() // Call fundContract()
                };

                var offlineTransactionSigner = new AccountOfflineTransactionSigner();
                var signedTransaction = await account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the signed transaction
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);

                Console.WriteLine($"Transaction successfully sent. Hash: {transactionHash}");

                return transactionHash; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                Console.WriteLine($"Smart contract error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetLoanIds()
        {
            try
            {
                var Abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddres = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var web3 = new Web3(_inFuraUrl);
                Abi!.Abi = JsonDocument.Parse(Abi.Abi!).RootElement.GetRawText();

                var contract = web3.Eth.GetContract(Abi!.Abi, smartContractAddres!.ContractAddress);
                var lenderFunction = contract.GetFunction("getAllLoanIds");
                var loanids = await lenderFunction.CallAsync<List<string>>();
                return loanids;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return null;
        }

        public async Task<string> TakeOutContractFundsAsync(string lenderAddress, decimal amountToWithdraw)
        {
            try
            {
                // Create an account with the private key
                var account = new Account(_privateKey);
                var web3 = new Web3(account, _inFuraUrl);
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Retrieve the ABI and smart contract address
                var abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddress = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abi == null || smartContractAddress == null)
                {
                    throw new Exception("Smart contract ABI or address not found.");
                }

                // Parse the ABI
                abi.Abi = JsonDocument.Parse(abi.Abi!).RootElement.GetRawText();

                // Access the smart contract and "takeOutContractFunds" function
                var contract = web3.Eth.GetContract(abi.Abi, smartContractAddress.ContractAddress);
                var takeOutContractFundsFunction = contract.GetFunction("takeOutContractFunds");

                // Convert the withdrawal amount to Wei
                var amountInWei = amountToWithdraw;// Web3.Convert.ToWei(amountToWithdraw);

                // Estimate the gas for the transaction
                var gasLimit = await takeOutContractFundsFunction.EstimateGasAsync(
                                     from: lenderAddress,    // Replace with your wallet address
                                     gas: new Nethereum.Hex.HexTypes.HexBigInteger(200000), // Set an appropriate gas limit
                                     value: new HexBigInteger(0),
                                     functionInput: new object[] { amountInWei }
                                );
                // Get the nonce for the sender's address
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Sign and send the transaction
                var transactionInput = takeOutContractFundsFunction.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasLimit),
                    gasPrice: null, // Optional: Specify a gas price or let it use the default
                    value: null, // No Ether is sent to the function
                    functionInput: new object[] { amountInWei }
                );

                var offlineTransactionSigner = new AccountOfflineTransactionSigner();
                var signedTransaction = await account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the raw transaction
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
                Console.WriteLine($"Transaction successfully sent. Hash: {transactionHash}");

                return transactionHash; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                Console.WriteLine($"Smart contract error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ChangeLenderAsync(string lenderAddress, string newLenderAddress)
        {
            try
            {
                // Initialize Web3 and account
                var account = new Account(_privateKey);  // Use the private key of the current lender
                var web3 = new Web3(account, _inFuraUrl);
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Retrieve the ABI and smart contract address
                var abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddress = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abi == null || smartContractAddress == null)
                {
                    throw new Exception("Smart contract ABI or address not found.");
                }

                // Parse the ABI
                abi.Abi = JsonDocument.Parse(abi.Abi!).RootElement.GetRawText();

                // Access the smart contract and "changeLender" function
                var contract = web3.Eth.GetContract(abi.Abi, smartContractAddress.ContractAddress);
                var changeLenderFunction = contract.GetFunction("changeLender");

                // Estimate gas for the transaction
                var gasLimit = await changeLenderFunction.EstimateGasAsync(
                                     from: lenderAddress,    // Replace with your wallet address
                                     gas: new Nethereum.Hex.HexTypes.HexBigInteger(200000), // Set an appropriate gas limit
                                     value: new HexBigInteger(0),
                                     functionInput: new object[] { newLenderAddress }
                                );

                Console.WriteLine($"Gas estimate: {gasLimit}");

                // Get the nonce for the sender's address
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = changeLenderFunction.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasLimit),
                    value: new HexBigInteger(0),
                    functionInput: new object[] { newLenderAddress } // Input: the new lender's address
                );

                // Sign the transaction using the account
                var offlineTransactionSigner = new AccountOfflineTransactionSigner();
                var signedTransaction = await account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the raw transaction
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
                Console.WriteLine($"Transaction successfully sent. Hash: {transactionHash}");

                return transactionHash; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                Console.WriteLine($"Smart contract error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> SetCommonAndInterestDetailsAsync(string lenderAddress, LoanDetails loanDetails)
        {
            try
            {
                // Initialize Web3 and account
                var account = new Account(_privateKey);  // Use the private key of the account calling the function
                var web3 = new Web3(account, _inFuraUrl);
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Retrieve the ABI and smart contract address
                var abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddress = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abi == null || smartContractAddress == null)
                {
                    throw new Exception("Smart contract ABI or address not found.");
                }

                // Parse the ABI
                abi.Abi = JsonDocument.Parse(abi.Abi!).RootElement.GetRawText();

                // Access the smart contract and "setCommonAndInterestDetails" function
                var contract = web3.Eth.GetContract(abi.Abi, smartContractAddress.ContractAddress);
                var setCommonAndInterestDetailsFunction = contract.GetFunction("setCommonAndInterestDetails");

                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await setCommonAndInterestDetailsFunction.EstimateGasAsync(
                                    from: lenderAddress,    // Replace with your wallet address
                                    gas: gasPrice, // Set an appropriate gas limit
                                    value: new HexBigInteger(0),
                                    functionInput: new object[]
                                        {
                                            loanDetails.LoanId,
                                            loanDetails.EmiPaymentStartDate,
                                            loanDetails.PrincipalAmount,
                                            loanDetails.FixedInterestRate,
                                            loanDetails.MonthlyPaymentAmount,
                                            loanDetails.MaturityDate,
                                            loanDetails.EmiPaymentDay,
                                            loanDetails.InterestRateChangeDate,
                                            loanDetails.LenderName,
                                            loanDetails.BorrowerName
                                        }
                               );

                Console.WriteLine($"Gas estimate: {gasEstimate}");

                // Get the nonce for the sender's address
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = setCommonAndInterestDetailsFunction.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasEstimate),
                    value: new HexBigInteger(0),
                    functionInput: new object[]
                    {
                        loanDetails.LoanId,
                        loanDetails.EmiPaymentStartDate,
                        loanDetails.PrincipalAmount,
                        loanDetails.FixedInterestRate,
                        loanDetails.MonthlyPaymentAmount,
                        loanDetails.MaturityDate,
                        loanDetails.EmiPaymentDay,
                        loanDetails.InterestRateChangeDate,
                        loanDetails.LenderName,
                        loanDetails.BorrowerName
                    }
                );

                var offlineTransactionSigner = new AccountOfflineTransactionSigner();
                var signedTransaction = await account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the raw transaction
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
                Console.WriteLine($"Transaction successfully sent. Hash: {transactionHash}");

                return transactionHash; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                Console.WriteLine($"Smart contract error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> SetPaymentAndLateDetailsAsync(string lenderAddress, InterestDetails details)
        {
            try
            {
                // Initialize Web3 and account
                var account = new Account(_privateKey);  // Use the private key of the account calling the function
                var web3 = new Web3(account, _inFuraUrl);
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Retrieve the ABI and smart contract address
                var abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddress = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abi == null || smartContractAddress == null)
                {
                    throw new Exception("Smart contract ABI or address not found.");
                }

                // Parse the ABI
                abi.Abi = JsonDocument.Parse(abi.Abi!).RootElement.GetRawText();

                // Access the smart contract and "setCommonAndInterestDetails" function
                var contract = web3.Eth.GetContract(abi.Abi, smartContractAddress.ContractAddress);
                var setPaymentAndLateDetailsFunction = contract.GetFunction("setPaymentAndLateDetails");

                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await setPaymentAndLateDetailsFunction.EstimateGasAsync(
                                    from: lenderAddress,    // Replace with your wallet address
                                    gas: gasPrice, // Set an appropriate gas limit
                                    value: new HexBigInteger(0),
                                    functionInput: new object[]
                                        {
                                            details.LoanId,
                                            details.LatePaymentGracePeriod,
                                            details.LateChargePercentage,
                                            details.Margin,
                                            details.CurrentIndex,
                                            details.MaxInterestRateAtFirstChange,
                                            details.MinInterestRateAtFirstChange,
                                            details.MaxInterestRateAfterChange,
                                            details.MinInterestRateAfterChange
                                        }
                               );

                Console.WriteLine($"Gas estimate: {gasEstimate}");

                // Get the nonce for the sender's address
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = setPaymentAndLateDetailsFunction.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasEstimate),
                    value: new HexBigInteger(0),
                    functionInput: new object[]
                    {
                        details.LoanId,
                        details.LatePaymentGracePeriod,
                        details.LateChargePercentage,
                        details.Margin,
                        details.CurrentIndex,
                        details.MaxInterestRateAtFirstChange,
                        details.MinInterestRateAtFirstChange,
                        details.MaxInterestRateAfterChange,
                        details.MinInterestRateAfterChange
                    }
                );

                var offlineTransactionSigner = new AccountOfflineTransactionSigner();
                var signedTransaction = await account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the raw transaction
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
                Console.WriteLine($"Transaction successfully sent. Hash: {transactionHash}");

                return transactionHash; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                Console.WriteLine($"Smart contract error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> SetAdjustableInterestRateDetailsAsync(string lenderAddress, OtherDetails details)
        {
            try
            {
                // Initialize Web3 and account
                var account = new Account(_privateKey);  // Use the private key of the account calling the function
                var web3 = new Web3(account, _inFuraUrl);
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Retrieve the ABI and smart contract address
                var abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddress = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abi == null || smartContractAddress == null)
                {
                    throw new Exception("Smart contract ABI or address not found.");
                }

                // Parse the ABI
                abi.Abi = JsonDocument.Parse(abi.Abi!).RootElement.GetRawText();

                // Access the smart contract and "setCommonAndInterestDetails" function
                var contract = web3.Eth.GetContract(abi.Abi, smartContractAddress.ContractAddress);
                var setAdjustableInterestRateDetailsFunction = contract.GetFunction("setAdjustableInterestRateDetails");

                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await setAdjustableInterestRateDetailsFunction.EstimateGasAsync(
                                    from: lenderAddress,    // Replace with your wallet address
                                    gas: gasPrice, // Set an appropriate gas limit
                                    value: new HexBigInteger(0),
                                    functionInput: new object[]
                                        {
                                            details.LoanId,
                                            details.NoteDate,
                                            details.City,
                                            details.State,
                                            details.PropertyAddress,
                                            details.PaymentLocation
                                        }
                               );

                Console.WriteLine($"Gas estimate: {gasEstimate}");

                // Get the nonce for the sender's address
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = setAdjustableInterestRateDetailsFunction.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasEstimate),
                    value: new HexBigInteger(0),
                    functionInput: new object[]
                    {
                        details.LoanId,
                        details.NoteDate,
                        details.City,
                        details.State,
                        details.PropertyAddress,
                        details.PaymentLocation
                    }
                );

                var offlineTransactionSigner = new AccountOfflineTransactionSigner();
                var signedTransaction = await account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the raw transaction
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
                Console.WriteLine($"Transaction successfully sent. Hash: {transactionHash}");

                return transactionHash; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                Console.WriteLine($"Smart contract error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Loan> GetLoanDetailsAsync(string loanId)
        {
            try
            {
                // Initialize Web3
                var web3 = new Web3(_inFuraUrl);

                // Retrieve ABI and contract address
                var abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddress = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abi == null || smartContractAddress == null)
                {
                    throw new Exception("Smart contract ABI or address not found.");
                }

                // Parse ABI
                abi.Abi = JsonDocument.Parse(abi.Abi!).RootElement.GetRawText();

                // Get the contract and function
                var contract = web3.Eth.GetContract(abi.Abi, smartContractAddress.ContractAddress);
                var getLoanDetailsFunction = contract.GetFunction("getLoanDetails");

                // Call the function
                var result = await getLoanDetailsFunction.CallDeserializingToObjectAsync<Loan>(loanId);

                // Return the deserialized result
                return result;
            }
            catch (SmartContractRevertException ex)
            {
                Console.WriteLine($"Smart contract error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> DeleteLoanAsync(string lenderAddress, string loanId)
        {
            try
            {
                // Initialize Web3 and account
                var account = new Account(_privateKey);  // Use the private key of the account calling the function
                var web3 = new Web3(account, _inFuraUrl);
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Retrieve the ABI and smart contract address
                var abi = await _smartContractAbiService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);
                var smartContractAddress = await _smartContractAddressService.GetByIdAsync(null, x => x.IsActive && !x.IsDelete);

                if (abi == null || smartContractAddress == null)
                {
                    throw new Exception("Smart contract ABI or address not found.");
                }

                // Parse the ABI
                abi.Abi = JsonDocument.Parse(abi.Abi!).RootElement.GetRawText();

                // Access the smart contract and "setCommonAndInterestDetails" function
                var contract = web3.Eth.GetContract(abi.Abi, smartContractAddress.ContractAddress);
                var deleteLoanFunction = contract.GetFunction("deleteLoan");

                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await deleteLoanFunction.EstimateGasAsync(
                                    from: lenderAddress,    // Replace with your wallet address
                                    gas: gasPrice, // Set an appropriate gas limit
                                    value: new HexBigInteger(0),
                                    functionInput: loanId
                               );

                Console.WriteLine($"Gas estimate: {gasEstimate}");

                // Get the nonce for the sender's address
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = deleteLoanFunction.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasEstimate),
                    value: new HexBigInteger(0),
                    functionInput: loanId
                );

                var offlineTransactionSigner = new AccountOfflineTransactionSigner();
                var signedTransaction = await account.TransactionManager.SignTransactionAsync(
                       transactionInput
                   );

                // Send the raw transaction
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
                Console.WriteLine($"Transaction successfully sent. Hash: {transactionHash}");

                return transactionHash; // Return the transaction hash
            }
            catch (SmartContractRevertException ex)
            {
                Console.WriteLine($"Smart contract error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}