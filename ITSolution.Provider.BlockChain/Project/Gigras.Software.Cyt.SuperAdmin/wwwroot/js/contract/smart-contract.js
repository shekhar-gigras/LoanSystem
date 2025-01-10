// multiLoanContract.js
class LoanContract {
    constructor(contractAddress, abi, provider) {
        // Initialize web3 instance
        this.web3 = new Web3(provider);
        this.contract = new this.web3.eth.Contract(abi, contractAddress);
    }

    async getlendderAddress() {
        try {
            let lenderaddress = await this.contract.methods.lender().call();
            return lenderaddress;
        } catch {
            return null;
        }
    }

    async IsLendder() {
        try {
            let address = (await this.getAddress()).toLowerCase();
            let lenderAddress = (await this.contract.methods.lender().call()).toLowerCase();

            return address === lenderAddress;
        } catch {
            return false;
        }
    }

    async getContractBalance() {
        try {
            let address = await this.getAddress();
            const balance = await this.contract.methods.checkBalanceOfSmartContract().call({
                from: address // Ensure this is the lender's address
            });
            //let balanceEther = this.web3.utils.fromWei(balance, 'ether');
            return balance;
        } catch {
            return null;
        }
    }

    async AddfundContract(amount) {
        try {
            const accounts = await this.web3.eth.getAccounts();
            const lenderAddress = accounts[0];

            // Call fundContract method
            await this.contract.methods.fundContract()
                .send({ from: lenderAddress, value: this.web3.utils.toWei(amount.toString(), 'wei') });
            return true
        } catch {
            return false;
        }
    }

    async changeLender(newLenderAddress) {
        try {
            const currentAddress = await this.getAddress();

            // Change lender
            await this.contract.methods.changeLender(newLenderAddress).send({
                from: currentAddress // Only the current lender can call this
            });
            return true;
        } catch {
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
            return balanceEther;
        } catch (error) {
            console.error('Error fetching lender balance:', error.message);
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
            return requestedBorrowers;
        } catch {
            return null;
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
            return activeBorrowers;
        } catch (error) {
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
            return true;
        } catch {
            return false;
        }
    }

    async AddCommonLoanDetails(data) {
        try {
            const currentAddress = await this.getAddress();

            // Change lender
            await this.contract.methods.setCommonAndInterestDetails(
                data.loanId,
                data.emiPaymentStartDate,
                data.principalAmount,
                data.fixedInterestRate,
                data.monthlyPaymentAmount,
                data.maturityDate,
                data.emiPaymentDay,
                data.interestRateChangeDate,
                data.lenderName,
                data.borrowerName
            ).send({
                from: currentAddress // Only the current lender can call this
            });
            return true;
        } catch (error) {
            return false;
        }
    }

    async AddPaymentAndLateLoanDetails(data) {
        try {
            const currentAddress = await this.getAddress();

            // Change lender
            await this.contract.methods.setPaymentAndLateDetails(
                data.loanId,
                data.latePaymentGracePeriod,
                data.lateChargePercentage,
                data.margin,
                data.currentIndex,
                data.maxInterestRateAtFirstChange,
                data.minInterestRateAtFirstChange,
                data.maxInterestRateAfterChange,
                data.minInterestRateAfterChange
            ).send({
                from: currentAddress // Only the current lender can call this
            });
            return true;
        } catch (error) {
            return false;
        }
    }

    async addOtherLoanDetails(data) {
        try {
            const currentAddress = await this.getAddress();

            // Change lender
            await this.contract.methods.setAdjustableInterestRateDetails(
                data.loanId,
                data.noteDate,
                data.city,
                data.state,
                data.propertyAddress,
                data.paymentLocation
            ).send({
                from: currentAddress // Only the current lender can call this
            });
            return true;
        } catch {
            return false;
        }
    }

    async AddUpdateLoanDetail(loanid, data) {
        try {
            await this.AddCommonLoanDetails(data);
            //await this.AddPaymentAndLateLoanDetails(data)
            //await this.addOtherLoanDetails(data)
            return true;
        } catch {
            return null;
        }
    }

    async getLoanInfo(loanid) {
        try {
            const currentAddress = await this.getAddress();

            // Change lender
            let objDetail = await this.contract.methods.getLoanDetails(loanid).call({
                from: currentAddress // Only the current lender can call this
            });
            return objDetail;
        } catch {
            return null;
        }
    }

    async getLoanIds() {
        try {
            const currentAddress = await this.getAddress();

            // Change lender
            let objDetail = await this.contract.methods.getAllLoanIds().call({
                from: currentAddress // Only the current lender can call this
            });
            let distinctObjDetail = [...new Set(objDetail)];

            return distinctObjDetail;
        } catch {
            return null;
        }
    }

    async RemoveLoan(loanid) {
        try {
            const currentAddress = await this.getAddress();

            // Change lender
            let objDetail = await this.contract.methods.deleteLoan(loanid).send({
                from: currentAddress // Only the current lender can call this
            });
            return objDetail;
        } catch {
            return null;
        }
    }

    async getAddress() {
        try {
            const accounts = await this.web3.eth.getAccounts();
            const Address = accounts[0];
            return Address;
        } catch {
            return null;
        }
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