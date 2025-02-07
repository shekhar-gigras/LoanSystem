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
    public interface ISmartContractBorrowerService
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

        //Specified Loan
        Task<ResponseModel> GetActiveBorrowerAddressesAsync(string lenderaddress);

        Task<ResponseModel> GetRequestedBorrowerAddressesAsync(string lenderaddress);

        Task<ResponseModel> GetTotalBorrowersAsync();

        Task<ResponseModel> RequestLoanAsync(string lenderAddress, string borrowerAddress, BorrowerLoanRequest loanRequest);

        Task<ResponseModel> ApproveLoanAsync(string lenderAddress, string borrowerAddress);

        Task<ResponseModel> PrepareTransaction(TransactionRequestModel request);
    }

    public class SmartContractBorrowerService : SmartContractBaseService, ISmartContractBorrowerService
    {
        public SmartContractBorrowerService(
                ILogger<SmartContractBorrowerService> logger,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration configuration,
                ISmartContractAbiService smartContractAbiService,
                ISmartContractAddressService smartContractAddressService) : base(logger, webHostEnvironment, configuration, smartContractAbiService, smartContractAddressService)
        {
        }

        public async Task<ResponseModel> GetActiveBorrowerAddressesAsync(string lenderaddress)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("getActiveBorrowers");
                var output = await Function.CallAsync<List<string>>(from: lenderaddress, null, null, null, null);
                response = new ResponseModel() { Status = true, Message = "Transaction successfully", Result = output, Final = output };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> GetRequestedBorrowerAddressesAsync(string lenderaddress)
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("getRequestedBorrowers");
                var output = await Function.CallAsync<List<string>>(from: lenderaddress, null, null, null, null);
                response = new ResponseModel() { Status = true, Message = "Transaction successfully", Result = output, Final = output };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> GetTotalBorrowersAsync()
        {
            var response = new ResponseModel();
            try
            {
                var Function = _contract.GetFunction("getBorrowerCount");
                var output = await Function.CallAsync<int>();
                response = new ResponseModel() { Status = true, Message = "Transaction successfully", Result = output, Final = output };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseModel> RequestLoanAsync(string lenderAddress, string borrowerAddress, BorrowerLoanRequest loanRequest)
        {
            var response = new ResponseModel();

            try
            {
                // Get the function from the contract
                var Function = _contract.GetFunction("requestLoan");

                // Estimate gas for the transaction
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasLimit = await Function.EstimateGasAsync(
                    from: borrowerAddress,
                    gas: gasPrice,
                    value: new HexBigInteger(0),
                    functionInput: new object[]
                    {
                        loanRequest.LoanAmount,
                        loanRequest.InterestRate,
                        loanRequest.RepaymentDurationInMonths
                    }
                );
                // Get the nonce for the borrower address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(borrowerAddress);
                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
                    from: borrowerAddress,
                    gas: new HexBigInteger(gasLimit),
                    value: new HexBigInteger(0),
                    functionInput: new object[]
                    {
                        loanRequest.LoanAmount,
                        loanRequest.InterestRate,
                        loanRequest.RepaymentDurationInMonths
                    }
                );
                transactionInput.Nonce = new HexBigInteger(nonce.Value);

                // Sign the transaction
                var signedTransaction = await _account.TransactionManager.SignTransactionAsync(transactionInput);
                // Send the transaction
                var transactionHash = await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);

                response = new ResponseModel()
                {
                    Status = true,
                    Message = "Loan requested successfully",
                    Result = transactionHash,
                    Final = transactionHash
                };

                return response;
            }
            catch (SmartContractRevertException ex)
            {
                response = new ResponseModel()
                {
                    Status = false,
                    Message = $"Smart contract revert: {ex.Message}"
                };

                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel()
                {
                    Status = false,
                    Message = $"Error occurred: {ex.Message}"
                };

                return response;
            }
        }

        public async Task<ResponseModel> ApproveLoanAsync(string lenderAddress, string borrowerAddress)
        {
            var response = new ResponseModel();

            try
            {
                var Function = _contract.GetFunction("approveLoan");

                // Estimate gas for the transaction
                var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
                var gasEstimate = await Function.EstimateGasAsync(
                    from: lenderAddress,  // Lender's address
                    gas: gasPrice,
                    value: new HexBigInteger(0), // No Ether value required for this function
                    functionInput: new object[] { borrowerAddress }
                );

                // Get the nonce for the lender's address
                var nonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(lenderAddress);

                // Create the transaction input
                var transactionInput = Function.CreateTransactionInput(
                    from: lenderAddress,
                    gas: new HexBigInteger(gasEstimate),
                    value: new HexBigInteger(0),
                    functionInput: new object[] { borrowerAddress }
                );

                // Sign the transaction with the lender's private key
                var signedTransaction = await _account.TransactionManager.SignTransactionAsync(transactionInput);

                // Send the raw transaction
                var transactionHash = await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);

                response = new ResponseModel()
                {
                    Status = true,
                    Message = "Loan approved and funded successfully.",
                    Result = transactionHash,
                    Final = transactionHash
                };

                return response;
            }
            catch (SmartContractRevertException ex)
            {
                response = new ResponseModel()
                {
                    Status = false,
                    Message = $"Smart contract revert: {ex.Message}"
                };
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseModel()
                {
                    Status = false,
                    Message = $"Error occurred: {ex.Message}"
                };
                return response;
            }
        }
    }
}