async function GetAllLendderInfo() {
    if (typeof loanContract == 'undefined')
        await checkMetaMaskConnection();
    else {
        if (typeof window.ethereum !== 'undefined') {
            const accounts = await ethereum.request({ method: 'eth_accounts' });
            if (accounts.length > 0) {
                Swal.fire({
                    title: "Please wait.. we are getting the all information.",
                    didOpen: () => {
                        Swal.showLoading();
                    },
                    allowOutsideClick: false,
                    allowEscapeKey: false
                });
                try {
                    let isprocess = await loanContract.IsLendder();
                    if (isprocess) {
                        let balance = await loanContract.getContractBalance();
                        if (balance != null) {
                            PreviousBalance = balance;
                            document.getElementById('contract-balance').textContent = "Balance : " + balance;
                            document.getElementById('current-balance').textContent = "Balance : " + balance;
                            document.getElementById('takeout-balance').textContent = "Balance : " + balance;
                            document.getElementById('header-smart-contract-balance').textContent = "Smart Contract Balance : " + balance;
                        } else {
                            document.getElementById('contract-balance').textContent = "Balance : " + 0;
                            document.getElementById('current-balance').textContent = "Balance : " + 0;
                            document.getElementById('takeout-balance').textContent = "Balance : " + 0;
                            document.getElementById('header-smart-contract-balance').textContent = "Smart Contract Balance : " + 0;
                        }
                        balance = await loanContract.getLenderBalance();
                        if (balance != null) {
                            document.getElementById('lendder-balance').textContent = "Balance : " + balance;
                        }
                        else {
                            document.getElementById('lendder-balance').textContent = "Balance : " + 0;
                        }
                        let totalborrower = await loanContract.getRequestedBorrowers();
                        if (totalborrower != null) {
                            document.getElementById('requested-borrower').textContent = "Total : " + totalborrower.length;
                        }
                        else {
                            document.getElementById('requested-borrower').textContent = "Total : " + 0;
                        }
                        totalborrower = await loanContract.getActiveBorrowers();
                        if (totalborrower != null) {
                            document.getElementById('active-borrower').textContent = "Total : " + totalborrower.length;
                            document.getElementById('header-total-active-borrow').textContent = "Total Active Borrow : " + totalborrower.length;
                       }
                        else {
                            document.getElementById('active-borrower').textContent = "Total : " + 0;
                            document.getElementById('header-total-active-borrow').textContent = "Total Active Borrow : " + 0;
                      }
                        let lenderaddress = await loanContract.getlendderAddress();
                        if (lenderaddress != null) {
                            PrevioudLender = lenderaddress;
                            document.getElementById('lendder-address').textContent = "Address : " + lenderaddress;
                       }
                        else {
                            document.getElementById('lendder-address').textContent = "Address : ";
                        }

                        Swal.close();
                    }
                    else {
                        Swal.fire({
                            text: `Please login as a lender`,
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Ok, got it!",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary"
                            }
                        });
                    }
                }
                catch {
                    swal.fire({
                        title: "error!",
                        text: 'Error connecting to the contract:',
                        icon: "error",
                        confirmButtonText: "OK"
                    });
                }
            }
        }
    }
}
