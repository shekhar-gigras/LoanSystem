using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Gigras.Software.BlockChain.Service
{
    public static class LoanProvider
    {
        private static string rpcUrl = "https://sepolia.infura.io/v3/4d39d517c0a5417fbc15dde4bad2e182"; // Replace with your Ethereum node URL
        private static string contractAddress = "0x5b2a6640153D1df9b6a8bB37E9B07EBa9F06b637"; // Replace with your contract address
        private static string abi = @"[{""inputs"":[],""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""lender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount"",""type"":""uint256""}],""name"":""ContractFunded"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""string"",""name"":""loanId"",""type"":""string""},{""indexed"":false,""internalType"":""string"",""name"":""propertyAddress"",""type"":""string""}],""name"":""LoanCreated"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""string"",""name"":""loanId"",""type"":""string""}],""name"":""LoanDeleted"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""string"",""name"":""loanId"",""type"":""string""},{""indexed"":false,""internalType"":""string"",""name"":""propertyAddress"",""type"":""string""}],""name"":""LoanUpdated"",""type"":""event""},{""inputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""name"":""adjustableInterestRateDetailsMap"",""outputs"":[{""internalType"":""string"",""name"":""noteDate"",""type"":""string""},{""internalType"":""string"",""name"":""city"",""type"":""string""},{""internalType"":""string"",""name"":""state"",""type"":""string""},{""internalType"":""string"",""name"":""propertyAddress"",""type"":""string""},{""internalType"":""string"",""name"":""paymentLocation"",""type"":""string""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_address"",""type"":""address""}],""name"":""changeLender"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""name"":""checkBalanceOfSmartContract"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""name"":""commonAndInterestDetailsMap"",""outputs"":[{""internalType"":""string"",""name"":""emiPaymentStartDate"",""type"":""string""},{""internalType"":""uint256"",""name"":""principalAmount"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""fixedInterestRate"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""monthlyPaymentAmount"",""type"":""uint256""},{""internalType"":""string"",""name"":""maturityDate"",""type"":""string""},{""internalType"":""uint8"",""name"":""emiPaymentDay"",""type"":""uint8""},{""internalType"":""string"",""name"":""interestRateChangeDate"",""type"":""string""},{""internalType"":""string"",""name"":""lenderName"",""type"":""string""},{""internalType"":""string"",""name"":""borrowerName"",""type"":""string""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":""loanId"",""type"":""string""}],""name"":""deleteLoan"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""name"":""fundContract"",""outputs"":[],""stateMutability"":""payable"",""type"":""function""},{""inputs"":[],""name"":""getAllLoanIds"",""outputs"":[{""internalType"":""string[]"",""name"":"""",""type"":""string[]""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_borrower"",""type"":""address""}],""name"":""getBorrowerBalance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""getLenderBalance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":""loanId"",""type"":""string""}],""name"":""getLoanDetails"",""outputs"":[{""components"":[{""internalType"":""string"",""name"":""emiPaymentStartDate"",""type"":""string""},{""internalType"":""uint256"",""name"":""principalAmount"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""fixedInterestRate"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""monthlyPaymentAmount"",""type"":""uint256""},{""internalType"":""string"",""name"":""maturityDate"",""type"":""string""},{""internalType"":""uint8"",""name"":""emiPaymentDay"",""type"":""uint8""},{""internalType"":""string"",""name"":""interestRateChangeDate"",""type"":""string""},{""internalType"":""string"",""name"":""lenderName"",""type"":""string""},{""internalType"":""string"",""name"":""borrowerName"",""type"":""string""}],""internalType"":""struct LoanAgreementContract.CommonAndInterestDetails"",""name"":"""",""type"":""tuple""},{""components"":[{""internalType"":""uint8"",""name"":""latePaymentGracePeriod"",""type"":""uint8""},{""internalType"":""uint256"",""name"":""lateChargePercentage"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""margin"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""currentIndex"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""maxInterestRateAtFirstChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""minInterestRateAtFirstChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""maxInterestRateAfterChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""minInterestRateAfterChange"",""type"":""uint256""}],""internalType"":""struct LoanAgreementContract.PaymentAndLateDetails"",""name"":"""",""type"":""tuple""},{""components"":[{""internalType"":""string"",""name"":""noteDate"",""type"":""string""},{""internalType"":""string"",""name"":""city"",""type"":""string""},{""internalType"":""string"",""name"":""state"",""type"":""string""},{""internalType"":""string"",""name"":""propertyAddress"",""type"":""string""},{""internalType"":""string"",""name"":""paymentLocation"",""type"":""string""}],""internalType"":""struct LoanAgreementContract.AdjustableInterestRateDetails"",""name"":"""",""type"":""tuple""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""lender"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""name"":""paymentAndLateDetailsMap"",""outputs"":[{""internalType"":""uint8"",""name"":""latePaymentGracePeriod"",""type"":""uint8""},{""internalType"":""uint256"",""name"":""lateChargePercentage"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""margin"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""currentIndex"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""maxInterestRateAtFirstChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""minInterestRateAtFirstChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""maxInterestRateAfterChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""minInterestRateAfterChange"",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":""loanId"",""type"":""string""},{""internalType"":""string"",""name"":""_noteDate"",""type"":""string""},{""internalType"":""string"",""name"":""_city"",""type"":""string""},{""internalType"":""string"",""name"":""_state"",""type"":""string""},{""internalType"":""string"",""name"":""_propertyAddress"",""type"":""string""},{""internalType"":""string"",""name"":""_paymentLocation"",""type"":""string""}],""name"":""setAdjustableInterestRateDetails"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":""loanId"",""type"":""string""},{""internalType"":""string"",""name"":""_emiPaymentStartDate"",""type"":""string""},{""internalType"":""uint256"",""name"":""_principalAmount"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_fixedInterestRate"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_monthlyPaymentAmount"",""type"":""uint256""},{""internalType"":""string"",""name"":""_maturityDate"",""type"":""string""},{""internalType"":""uint8"",""name"":""_emiPaymentDay"",""type"":""uint8""},{""internalType"":""string"",""name"":""_interestRateChangeDate"",""type"":""string""},{""internalType"":""string"",""name"":""_lenderName"",""type"":""string""},{""internalType"":""string"",""name"":""_borrowerName"",""type"":""string""}],""name"":""setCommonAndInterestDetails"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":""loanId"",""type"":""string""},{""internalType"":""uint8"",""name"":""_latePaymentGracePeriod"",""type"":""uint8""},{""internalType"":""uint256"",""name"":""_lateChargePercentage"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_margin"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_currentIndex"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_maxInterestRateAtFirstChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_minInterestRateAtFirstChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_maxInterestRateAfterChange"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_minInterestRateAfterChange"",""type"":""uint256""}],""name"":""setPaymentAndLateDetails"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_amount"",""type"":""uint256""}],""name"":""takeOutContractFunds"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""}]"; // Replace with your contract's ABI
        private static string mywallet = "0xF5cEA5e4B6126ffe06B2AEF2Ce8c1c3b981fA3F4";
        private static string privateKey = "70d6fbd05080e82449ad03908af1fa4d5aa2a0e09276707fa8c69c44521ffbbc";

        public static async Task<string> GetDetail()
        {
            var web3 = new Web3(rpcUrl);
            try
            {
                // Get the contract instance
                var contract = web3.Eth.GetContract(abi, contractAddress);

                // Access the "lender" function
                var lenderFunction = contract.GetFunction("lender");

                // Call the "lender" function
                var lenderAddress = await lenderFunction.CallAsync<string>();

                Console.WriteLine($"Lender Address: {lenderAddress}");

                // Access the "lender" function
                var fundFunction = contract.GetFunction("checkBalanceOfSmartContract");

                // Call the "lender" function
                var total = await fundFunction.CallAsync<int>();

                return lenderAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return "";
        }

        public static async Task AddMoney(string address)
        {
            var web3 = new Web3(rpcUrl);
            try
            {
                var account = new Account(privateKey);
                // Get the contract instance
                var contract = web3.Eth.GetContract(abi, contractAddress);

                // Access the "fundContract" function
                var fundContractFunction = contract.GetFunction("fundContract");
                var value = new Nethereum.Hex.HexTypes.HexBigInteger(50); // Amount in Ether

                var gasEstimate = await fundContractFunction.EstimateGasAsync(
                   "0xf1641499E7733F717F72a437F446a5472c3965be",
                   null,
                   null,
                   null
               );

                var transactionHash = await fundContractFunction.SendTransactionAndWaitForReceiptAsync(
                    "0xf1641499E7733F717F72a437F446a5472c3965be",
                    gasEstimate,
                    new HexBigInteger(50), // Value to send, if any
                    null
                );
                var transactionReceipt = await fundContractFunction.SendTransactionAsync(
                 from: "0xf1641499E7733F717F72a437F446a5472c3965be",    // Replace with your wallet address
                 gas: new Nethereum.Hex.HexTypes.HexBigInteger(200000), // Set an appropriate gas limit
                 value: value // Ether amount in Wei
             );

                //Console.WriteLine($"Transaction successful! Hash: {transactionReceipt.}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static async Task SendMoney(string address)
        {
            try
            {
                //address = mywallet;
                // Replace with your private key and Infura details
                var account = new Account(privateKey);
                var web3 = new Web3(account, rpcUrl);
                address = account.Address;
                // Contract details

                // Create the contract instance
                var contract = web3.Eth.GetContract(abi, contractAddress);

                // Get the fundContract function
                var fundContractFunction = contract.GetFunction("fundContract");

                // Amount of Ether to send (e.g., 1 ETH)
                decimal etherToSend = 1m;
                var weiToSend = 50;// Web3.Convert.ToWei(etherToSend);

                // Create the transaction input
                var transactionInput = fundContractFunction.CreateTransactionInput(
                    from: "0xf1641499E7733F717F72a437F446a5472c3965be", // Sender's address
                    gas: null, // Let Web3 determine the gas
                    value: new HexBigInteger(weiToSend) // Ether amount in Wei
                );

                var setAdjustableInterestRateDetailsFunction = contract.GetFunction("setAdjustableInterestRateDetails");

                // Estimate gas
                //var gasEstimate = await web3.Eth.Transactions.EstimateGas.SendRequestAsync(transactionInput);

                var gasEstimate = await setAdjustableInterestRateDetailsFunction.EstimateGasAsync(
                   "0xf1641499E7733F717F72a437F446a5472c3965be",
                   null,
                   null,
                   "Test00001",
                   "h1",
                   "h2",
                   "h3",
                   "h4",
                   "h5"
               );

                // Send the transaction
                var transactionHash = await setAdjustableInterestRateDetailsFunction.SendTransactionAndWaitForReceiptAsync(
                    "0xf1641499E7733F717F72a437F446a5472c3965be",
                    gasEstimate,
                    new HexBigInteger(0), // Value to send, if any
                    null,
                   "Test00001",
                   "h1",
                   "h2",
                   "h3",
                   "h4",
                   "h5"
                );

                // Send the transaction
                //var transactionHash = await fundContractFunction.SendTransactionAndWaitForReceiptAsync(
                //    address,
                //    gasEstimate,
                //    new HexBigInteger(weiToSend),
                //    null // No parameters for this function
                //);
                Console.WriteLine($"Transaction sent successfully! Hash: {transactionHash}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void GetDetail1()
        {
            try
            {
                // Replace with your details
                var privateKey = "YOUR_PRIVATE_KEY"; // Lender's private key
                var rpcUrl = "https://sepolia.infura.io/v3/YOUR_INFURA_PROJECT_ID";
                var contractAddress = "0x5b2a6640153D1df9b6a8bB37E9B07EBa9F06b637"; // Replace with your contract address
                var abi = @"[YOUR_CONTRACT_ABI]"; // Replace with your contract's ABI

                // Create an account and Web3 instance
                var account = new Account(privateKey);
                var web3 = new Web3(account, rpcUrl);

                // Get the contract
                var contract = web3.Eth.GetContract(abi, contractAddress);

                // Get the setAdjustableInterestRateDetails function
                var setAdjustableInterestRateDetailsFunction = contract.GetFunction("setAdjustableInterestRateDetails");

                // Prepare function parameters
                string loanId = "loan123";
                string interestRateChangeDate = "2025-01-12";
                uint margin = 200; // Example values, adjust as needed
                uint currentIndex = 1;
                uint maxInterestRateAtFirstChange = 500; // Example in basis points
                uint minInterestRateAtFirstChange = 100; // Example in basis points
                uint maxInterestRateAfterChange = 700; // Example in basis points
                uint minInterestRateAfterChange = 150; // Example in basis points

                // Estimate gas

                //Console.WriteLine($"Transaction successful! Hash: {transactionHash.TransactionHash}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}