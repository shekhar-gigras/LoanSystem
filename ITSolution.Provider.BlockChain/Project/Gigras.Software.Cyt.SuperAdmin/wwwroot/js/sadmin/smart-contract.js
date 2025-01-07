// multiLoanContract.js
class LoanContract {
    constructor(contractAddress, abi, provider) {
        // Initialize web3 instance
        this.web3 = new Web3(provider);
        this.contract = new this.web3.eth.Contract(abi, contractAddress);
    }

    async IsLendder() {
        try {
            this.showLoader();
            let address = await this.getAddress();
            await this.contract.methods.checkBalanceOfSmartContract().call({
                from: address // Ensure this is the lender's address
            });
            console.log('Welcome Lendder:', address);
            this.hideLoader();
            //Swal.fire({
            //    text: "Welcome Lendder",
            //    icon: "success",
            //    buttonsStyling: false,
            //    confirmButtonText: "Ok, got it!",
            //    customClass: {
            //        confirmButton: "btn fw-bold btn-primary"
            //    }
            //});
            return true;
        } catch (error) {
            this.hideLoader();
            console.error('Error checking is lendder:', error.message);
            Swal.fire({
                text: `Please login as a lender`,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            }).then(() => {
                location.href = "/sadmin";
            });
            return false;
        }
    }

    async getContractBalance() {
        try {
            let address = await this.getAddress();
            const balance = await this.contract.methods.checkBalanceOfSmartContract().call({
                from: address // Ensure this is the lender's address
            });
            let balanceEther = this.web3.utils.fromWei(balance, 'ether');
            console.log('Contract balance:', balanceEther);
            Swal.fire({
                text: `Contract Balance is :${balance}`,
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return balance;
        } catch (error) {
            console.error('Error getting contract balance:', error.message);
            Swal.fire({
                text: `Lender can be use this functionality, Please login as a lender`,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return 0;
        }
    }

    async AddfundContract(amount) {
        try {
            const accounts = await this.web3.eth.getAccounts();
            const lenderAddress = accounts[0];

            // Call fundContract method
            await this.contract.methods.fundContract()
                .send({ from: lenderAddress, value: web3.utils.toWei(amount.toString(), 'wei') });
            console.log('Contract funded successfully!');
            Swal.fire({
                text: `Add fund ${amount} Successfully`,
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return true
        } catch (error) {
            console.error('Error funding contract:', error.message);
            Swal.fire({
                text: `Error getting Add fundcntract: :${error.message}`,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return false;
        }
    }

    async changeLender(newLenderAddress) {
        try {
            this.showLoader("Changing lender...");
            const currentAddress = await this.getAddress();

            // Change lender
            await this.contract.methods.changeLender(newLenderAddress).send({
                from: currentAddress // Only the current lender can call this
            });

            console.log('Lender changed successfully to:', newLenderAddress);
            Swal.fire({
                text: `Lender changed successfully to: ${newLenderAddress}`,
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            this.hideLoader();
            return true;
        } catch (error) {
            this.hideLoader();
            console.error('Error changing lender:', error.message);
            Swal.fire({
                text: `Error changing lender: ${error.message}`,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return false;
        }
    }

    async getBorrowerBalance(borrowerAddress) {
        try {
            this.showLoader("Changing lender...");
            // Fetch the balance of the borrower in Wei
            const balanceWei = await this.web3.eth.getBalance(borrowerAddress);

            // Convert the balance to Ether for better readability
            let balanceEther = this.web3.utils.fromWei(balanceWei, 'ether');

            console.log(`Balance of borrower ${borrowerAddress}: ${balanceEther} ether`);
            balanceEther = parseFloat(balanceEther).toFixed(4);
            this.hideLoader();
            Swal.fire({
                text: `Borrow Balance is : ${balanceEther}`,
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return balanceEther;
        } catch (error) {
            console.error(`Error fetching balance for ${borrowerAddress}:`, error.message);
            Swal.fire({
                text: `Error fetching borrower balance: ${error.message}`,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return null;
        }
    }

    async getLenderBalance() {
        try {
            const currentAddress = await this.getAddress();
            // Fetch the lender's address
            const lenderAddress = await this.contract.methods.lender().call({
                from: currentAddress // Only the current lender can call this
            });

            // Fetch the balance of the lender in Wei
            const balanceWei = await this.web3.eth.getBalance(lenderAddress);

            // Convert the balance to Ether for better readability
            let balanceEther = this.web3.utils.fromWei(balanceWei, "ether");
            balanceEther = parseFloat(balanceEther).toFixed(4);

            console.log(`Lender balance: ${balanceEther} ether`);
            Swal.fire({
                text: `Lender Balance: ${balanceEther} ether`,
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return balanceEther;
        } catch (error) {
            console.error('Error fetching lender balance:', error.message);
            Swal.fire({
                text: `Error fetching lender balance: ${error.message}`,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return null;
        }
    }

    async getRequestedBorrowers() {
        try {
            const lenderAddress = await this.getAddress(); // Get the current lender's address

            // Call the smart contract method to get requested borrowers
            const requestedBorrowers = await this.contract.methods.getRequestedBorrowers().call({
                from: lenderAddress // Only the lender can access this data
            });

            console.log('Requested Borrowers:', requestedBorrowers);
            return requestedBorrowers;
        } catch (error) {
            console.error('Error fetching requested borrowers:', error.message);
            Swal.fire({
                text: `Error fetching requested borrowers: ${error.message}`,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return [];
        }
    }

    async getLoanDetails(borrowerAddress) {
        try {
            // Fetch loan details for the borrower
            const loanDetails = await this.contract.methods.loans(borrowerAddress).call();
            let loanMessage = [];
            loanMessage["LoanAmount"] = this.web3.utils.fromWei(loanDetails.loanAmount.toString(), 'ether');
            loanMessage["InterestRate"] = loanDetails.interestRate;
            loanMessage["TotalRepayment"] = this.web3.utils.fromWei(loanDetails.totalRepayment.toString(), 'ether');
            loanMessage["LoanFunded"] = loanDetails.isLoanFunded ? 'Yes' : 'No';
            loanMessage["LoanRepaid"] = loanDetails.isLoanRepaid ? 'Yes' : 'No';
            // Prepare the loan details message
            const loanMessageArray = [
                `Loan Amount: ${this.web3.utils.fromWei(loanDetails.loanAmount.toString(), 'ether')} ETH`,
                `Interest Rate: ${loanDetails.interestRate}%`,
                `Total Repayment: ${this.web3.utils.fromWei(loanDetails.totalRepayment.toString(), 'ether')} ETH`,
                `Loan Funded: ${loanDetails.isLoanFunded ? 'Yes' : 'No'}`,
                `Loan Repaid: ${loanDetails.isLoanRepaid ? 'Yes' : 'No'}`
            ];
            //`Repayment Due Date: ${new Date(loanDetails.repaymentDueDate * 1000).toLocaleString()}`,

            if (loanDetails.repaymentDueDate == BigInt(0)) {
                loanMessage["RepaymentDueDate"] = "";
            }
            else {
                loanMessage["RepaymentDueDate"] = new Date(Number(loanDetails.repaymentDueDate) * 1000).toLocaleString();
            }

            const loanMessageHtml = loanMessageArray.join('<br>');
            // Show loan details in Swal message
            Swal.fire({
                title: 'Loan Details',
                html: loanMessageHtml,
                icon: 'info',
                buttonsStyling: false,
                confirmButtonText: 'Ok, got it!',
                customClass: {
                    confirmButton: 'btn fw-bold btn-primary'
                }
            });
            return loanMessage;
        } catch (error) {
            console.error('Error fetching loan details:', error.message);

            // Show error message if there's an issue
            Swal.fire({
                text: `Error fetching loan details: ${error.message}`,
                icon: 'error',
                buttonsStyling: false,
                confirmButtonText: 'Ok, got it!',
                customClass: {
                    confirmButton: 'btn fw-bold btn-primary'
                }
            });
            return null;
        }
    }

    async requestLoan(amount, interestRate, repaymentDuration, borrowerAddress) {
        try {
            // Call the requestLoan method from the smart contract
            await this.contract.methods.requestLoan(amount, interestRate, repaymentDuration)
                .send({ from: borrowerAddress });
            console.log('Loan requested successfully!');
            return true;
        } catch (error) {
            console.error('Error requesting loan:', error.message);
            return false;
        }
    }

    async fetchRequestedBorrowers() {
        try {
            const lenderAddress = await this.getAddress(); // Get the current lender's address
            const requestedBorrowers = await this.contract.methods.getRequestedBorrowers().call({ from: lenderAddress });
            console.log("Requested Borrowers:", requestedBorrowers);
            return requestedBorrowers;
        } catch (error) {
            console.error("Error fetching requested borrowers:", error.message);
            Swal.fire({
                text: `Error fetching requested borrowers: ${error.message}`,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return [];
        }
    }

    async approveLoan(borrowerAddress) {
        try {
            // Confirmation dialog
            const accounts = await this.web3.eth.getAccounts();
            const lenderAddress = accounts[0];
            // Approve loan for the borrower
            await this.contract.methods.approveLoan(borrowerAddress)
                .send({ from: lenderAddress });
            console.log('Loan approved for borrower:', borrowerAddress);
            return true;
        } catch (error) {
            // Error alert
            console.error('Error approving loan:', error.message);
            return false;
        }
    }

    async rejectLoanRequest(borrowerAddress) {
        try {
            const accounts = await this.web3.eth.getAccounts();
            const lenderAddress = accounts[0]; // Assuming the lender is the first account
            // Call the rejectLoanReq function from the smart contract
            await this.contract.methods.rejectLoanReq(borrowerAddress)
                .send({ from: lenderAddress });

            console.log(`Loan request from ${borrowerAddress} has been rejected.`);
            return true;
        } catch (error) {
            console.error("Error rejecting loan request:", error.message);
            return false;
        }
    }

    async getActiveBorrowers() {
        try {
            const accounts = await this.web3.eth.getAccounts();
            const lenderAddress = accounts[0];
            const activeBorrowers = await this.contract.methods.getActiveBorrowers().call({ from: lenderAddress });
            console.log('Active borrowers:', activeBorrowers);
            return activeBorrowers;
        } catch (error) {
            console.error('Error getting active borrowers:', error.message);
            return [];
        }
    }




    async repayLoan(borrowerAddress, repaymentAmount) {
        try {
            const accounts = await this.web3.eth.getAccounts();
            const borrower = accounts[0];

            // Repay loan
            await this.contract.methods.repayLoan()
                .send({ from: borrower, value: web3.utils.toWei(repaymentAmount.toString(), 'wei') });
            console.log('Loan repaid successfully!');
        } catch (error) {
            console.error('Error repaying loan:', error.message);
        }
    }

    async isRepaymentOverdue(borrowerAddress) {
        try {
            // Call isRepaymentOverdue function
            const overdueStatus = await this.contract.methods.isRepaymentOverdue(borrowerAddress).call();
            console.log(`Is repayment overdue for ${borrowerAddress}:`, overdueStatus);
            return overdueStatus;
        } catch (error) {
            console.error('Error checking if repayment is overdue:', error.message);
            return false;
        }
    }

    async getBorrowerCount() {
        try {
            const borrowerCount = await this.contract.methods.getBorrowerCount().call();
            console.log('Total borrower count:', borrowerCount);
            return borrowerCount;
        } catch (error) {
            console.error('Error getting borrower count:', error.message);
            return 0;
        }
    }

    async takeOutFunds() {
        try {
            // Get the accounts (assuming MetaMask or another Web3 provider is used)
            const accounts = await this.web3.eth.getAccounts();
            const lenderAddress = accounts[0]; // The lender is the first account

            // Send the transaction to take out funds
            const receipt = await this.contract.methods.takeOutContractFunds().send({ from: lenderAddress });

            // Show success message (could be enhanced with a library like SweetAlert2)
            console.log("Funds successfully withdrawn:", receipt);

            await Swal.fire({
                title: "Success!",
                text: "The funds have been successfully withdrawn.",
                icon: "success",
            });
        } catch (error) {
            // Handle any errors
            console.error("Error withdrawing funds:", error.message);

            await Swal.fire({
                title: "Error!",
                text: `Failed to withdraw funds: ${error.message}`,
                icon: "error",
            });
        }
    }

    async getAddress() {
        const accounts = await this.web3.eth.getAccounts();
        const Address = accounts[0];
        return Address;
    }

    async showLoader(message = "Please wait...") {
        Swal.fire({
            title: message,
            didOpen: () => {
                Swal.showLoading();
            },
            allowOutsideClick: false,
            allowEscapeKey: false
        });
    }

    // Function to hide SweetAlert2 loading spinner
    async hideLoader() {
        Swal.close();
    }
}