async function CheckSmartContractBalance() {
    if (typeof loanContract == 'undefined')
        await checkMetaMaskConnection();
    else {
        if (typeof window.ethereum !== 'undefined') {
            const accounts = await ethereum.request({ method: 'eth_accounts' });
            if (accounts.length > 0) {
                Swal.fire({
                    title: "Please wait.. we are getting the smart contract balance.",
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
                            document.getElementById('contract-balance').textContent = "Balance : " + balance;
                            PreviousBalance = balance;
                            setTimeout(async () => {
                                Swal.close(); // Close any previous Swal instances
                                Swal.fire({
                                    text: `Smart Contract Balance is : ` + balance,
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary"
                                    }
                                }).then(async () => {  // Make this async
                                    let formData = new FormData();
                                    formData.set("FormId","19")
                                    formData.set("SmartContractBalance", balance)
                                    formData.set("MetaMaskID", await loanContract.getAddress())
                                    let actionUrl = "/sadmin/borrower/submit-form"
                                    await DashBoardSubmitData(actionUrl,formData);  // Now it will work
                                });
                            }, 0);
                        } else {
                            document.getElementById('contract-balance').textContent = "Balance : " + 0;
                            setTimeout(() => {
                                Swal.close(); // Close any previous Swal instances
                                Swal.fire({
                                    text: `Error to get smart contract balance`,
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary"
                                    }
                                });
                            }, 0);
                        }
                        Swal.close();
                    }
                    else {
                        setTimeout(() => {
                            Swal.close(); // Close any previous Swal instances
                            Swal.fire({
                                text: `Please login as a lender`,
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Ok, got it!",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-primary"
                                }
                            });
                        }, 0);
                    }
                }
                catch {
                    setTimeout(() => {
                        Swal.close(); // Close any previous Swal instances
                        Swal.fire({
                            title: "Error!",
                            text: 'Error connecting to the contract.',
                            icon: "error",
                            confirmButtonText: "OK"
                        });
                    }, 0);
                }
            }
        }
    }
}
