using Gigras.Software.BlockChain.Service.Model;
using Gigras.Software.Cyt.Services.CytServcies;
using Gigras.Software.General.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Hex.HexTypes;

namespace Gigras.Software.BlockChain.Service
{
    public interface ISmartContractService
    {
        Task InitializeAsync(bool IsRequiredTrasaction = false);

        Task<ResponseModel> GetBalanceAsync(string address);

        Task<ResponseModel> GetContractOwnerAddressAsync();

        Task<ResponseModel> GetWalletAsync();

        Task<ResponseModel> GetContractBalanceAsync();

        Task<ResponseModel> GetBorrowBalanceAsync(string borroweraddress);

        Task<ResponseModel> GetLennderBalanceAsync(string lenderaddress);

        Task<ResponseModel> AddContractFundAsync(string lenderAddress, int amountInEther);

        Task<ResponseModel> AddContractFundDataAsync(string lenderAddress, int amountInEther);

        Task<ResponseModel> WithdrawalContractFundAsync(string lenderAddress, decimal amountToWithdraw);

        Task<ResponseModel> WithdrawalContractFundDataAsync(string lenderAddress, decimal amountToWithdraw);

        Task<ResponseModel> ChangeContractOwnerAsync(string lenderAddress, string newLenderAddress);

        Task<ResponseModel> ChangeContractOwnerDataAsync(string lenderAddress, string newLenderAddress);

        //Specific for loans
        Task<ResponseModel> GetLoanIds();

        Task<ResponseModel> SetCommonAndInterestDetailsAsync(string lenderAddress, LoanDetails loanDetails);

        Task<ResponseModel> SetCommonAndInterestDetailsDataAsync(string lenderAddress, LoanDetails loanDetails);

        Task<ResponseModel> GetCommonAndInterestDetailsAsync(string loanId);

        Task<ResponseModel> SetPaymentAndLateDetailsAsync(string lenderAddress, InterestDetails details);

        Task<ResponseModel> GetPaymentAndLateDetailsAsync(string loanId);

        Task<ResponseModel> SetAdjustableInterestRateDetailsAsync(string lenderAddress, OtherDetails details);

        Task<ResponseModel> SetPaymentAndLateDetailsDataAsync(string lenderAddress, InterestDetails details);

        Task<ResponseModel> SetAdjustableInterestRateDetailsDataAsync(string lenderAddress, OtherDetails details);

        Task<ResponseModel> GetAdjustableInterestRateDetailsAsync(string loanId);

        Task<ResponseModel> GetLoanDetailsAsync(string loanId);

        Task<ResponseModel> DeleteLoanAsync(string lenderAddress, string loanId);

        Task<ResponseModel> DeleteLoanDataAsync(string lenderAddress, string loanId);

        Task<ResponseModel> GetPublicKey();

        Task<ResponseModel> PrepareTransaction(TransactionRequestModel request);
    }

    public class SmartContractService : SmartContractBaseService, ISmartContractService
    {
        public SmartContractService(
                ILogger<SmartContractService> logger,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration configuration,
                ISmartContractAbiService smartContractAbiService,
                ISmartContractAddressService smartContractAddressService) : base(logger, webHostEnvironment, configuration, smartContractAbiService, smartContractAddressService)
        {
        }

        public async Task<ResponseModel> GetLoanIds()
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("getAllLoanIds");
                var output = await Function.CallAsync<List<string>>();
                response = new ResponseModel() { Status = true, Message = "Transaction successfully", Result = output, Final = output };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> SetCommonAndInterestDetailsAsync(string lenderAddress, LoanDetails loanDetails)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("setCommonAndInterestDetails");

                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await Function.EstimateGasAsync(
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
                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
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

        public async Task<ResponseModel> SetCommonAndInterestDetailsDataAsync(string lenderAddress, LoanDetails loanDetails)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("setCommonAndInterestDetails");

                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await Function.EstimateGasAsync(
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
                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
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

        public async Task<ResponseModel> GetCommonAndInterestDetailsAsync(string loanId)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("commonAndInterestDetailsMap");

                // Call the function

                var output = await Function.CallDeserializingToObjectAsync<CommonAndInterestDetails>(loanId);
                response = new ResponseModel() { Status = true, Result = output };
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

        public async Task<ResponseModel> SetPaymentAndLateDetailsAsync(string lenderAddress, InterestDetails details)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("setCommonAndInterestDetails");

                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await Function.EstimateGasAsync(
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
                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
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

        public async Task<ResponseModel> SetPaymentAndLateDetailsDataAsync(string lenderAddress, InterestDetails details)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("setCommonAndInterestDetails");

                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await Function.EstimateGasAsync(
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
                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
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

        public async Task<ResponseModel> GetPaymentAndLateDetailsAsync(string loanId)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("paymentAndLateDetailsMap");

                // Call the function

                var output = await Function.CallDeserializingToObjectAsync<PaymentAndLateDetails>(loanId);
                response = new ResponseModel() { Status = true, Result = output };
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

        public async Task<ResponseModel> SetAdjustableInterestRateDetailsAsync(string lenderAddress, OtherDetails details)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("setCommonAndInterestDetails");

                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await Function.EstimateGasAsync(
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
                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
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

        public async Task<ResponseModel> SetAdjustableInterestRateDetailsDataAsync(string lenderAddress, OtherDetails details)
        {
            var response = new ResponseModel();
            try
            {
                var function = _contract.GetFunction("setCommonAndInterestDetails");

                // Estimate gas price and limit
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await function.EstimateGasAsync(
                    from: lenderAddress,
                    gas: null, // Let the gas estimation process determine the limit
                    value: new HexBigInteger(0), // No Ether is sent
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

                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Prepare the transaction data
                var transactionData = new
                {
                    From = lenderAddress,
                    To = _contractAddress,
                    Gas = gasEstimate.Value.ToString(),
                    GasPrice = gasPrice.Value.ToString(),
                    Nonce = nonce.Value.ToString(),
                    Value = "0", // No Ether is sent
                    Data = function.GetData(new object[]
                    {
                details.LoanId,
                details.NoteDate,
                details.City,
                details.State,
                details.PropertyAddress,
                details.PaymentLocation
                    })
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
                response = new ResponseModel
                {
                    Status = false,
                    Message = $"Smart contract revert error: {ex.Message}"
                };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel
                {
                    Status = false,
                    Message = $"Error occurred: {ex.Message}"
                };
                return response;
            }
        }

        public async Task<ResponseModel> GetAdjustableInterestRateDetailsAsync(string loanId)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("paymentAndLateDetailsMap");

                // Call the function

                var output = await Function.CallDeserializingToObjectAsync<AdjustableInterestRateDetails>(loanId);
                response = new ResponseModel() { Status = true, Result = output };
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

        public async Task<ResponseModel> GetLoanDetailsAsync(string loanId)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("getLoanDetails");

                // Call the function
                var output = await Function.CallDeserializingToObjectAsync<Loan>(loanId);
                response = new ResponseModel() { Status = true, Result = output };
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

        public async Task<ResponseModel> DeleteLoanAsync(string lenderAddress, string loanId)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("deleteLoan");

                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await Function.EstimateGasAsync(
                                    from: lenderAddress,    // Replace with your wallet address
                                    gas: gasPrice, // Set an appropriate gas limit
                                    value: new HexBigInteger(0),
                                    functionInput: loanId
                               );
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasEstimate),
                    value: new HexBigInteger(0),
                    functionInput: loanId
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

        public async Task<ResponseModel> DeleteLoanDataAsync(string lenderAddress, string loanId)
        {
            var response = new ResponseModel();

            try
            {
                var function = _contract.GetFunction("deleteLoan");

                // Estimate gas price and limit
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await function.EstimateGasAsync(
                    from: lenderAddress,
                    gas: null, // Allow automatic determination of the limit
                    value: new HexBigInteger(0),
                    functionInput: new object[] { loanId }
                );

                // Get the nonce for the sender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Prepare the transaction data
                var transactionData = new
                {
                    From = lenderAddress,
                    To = _contractAddress,
                    Gas = gasEstimate.Value.ToString(),
                    GasPrice = gasPrice.Value.ToString(),
                    Nonce = nonce.Value.ToString(),
                    Value = "0", // No Ether is sent
                    Data = function.GetData(new object[] { loanId })
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
                response = new ResponseModel
                {
                    Status = false,
                    Message = $"Smart contract revert error: {ex.Message}"
                };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel
                {
                    Status = false,
                    Message = $"Error occurred: {ex.Message}"
                };
                return response;
            }
        }
    }
}