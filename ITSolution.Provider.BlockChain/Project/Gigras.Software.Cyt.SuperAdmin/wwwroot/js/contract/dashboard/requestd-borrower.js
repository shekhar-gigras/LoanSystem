async function CheckRequestedBorrower() {
    if (typeof loanContract == 'undefined')
        await checkMetaMaskConnection();
    else {
        if (typeof window.ethereum !== 'undefined') {
            const accounts = await ethereum.request({ method: 'eth_accounts' });
            if (accounts.length > 0) {
                Swal.fire({
                    title: "Please wait.. we are getting the requested borrowers.",
                    didOpen: () => {
                        Swal.showLoading();
                    },
                    allowOutsideClick: false,
                    allowEscapeKey: false
                });
                try {
                    let isprocess = await loanContract.IsLendder();
                    if (isprocess) {
                        let balance = await loanContract.getRequestedBorrowers();
                        if (balance != null) {
                            document.getElementById('requested-borrower').textContent = "Total : " + balance.length;
                            setTimeout(() => {
                                Swal.close(); // Close any previous Swal instances
                                Swal.fire({
                                    text: `Requested Borrowers is : ` + balance.length,
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary"
                                    }
                                });
                            }, 0);
                        } else {
                            document.getElementById('requested-borrower').textContent = "Total : " + 0;
                            setTimeout(() => {
                                Swal.close(); // Close any previous Swal instances
                                Swal.fire({
                                    text: `Error to get Requested Borrowers`,
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
                        swal.fire({
                            title: "error!",
                            text: 'Error connecting to the contract:',
                            icon: "error",
                            confirmButtonText: "OK"
                        });
                    }, 0);
                }
            }
        }
    }
}