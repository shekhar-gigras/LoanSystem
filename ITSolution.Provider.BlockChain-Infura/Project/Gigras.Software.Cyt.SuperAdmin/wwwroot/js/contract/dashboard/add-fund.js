let PreviousBalance = 0;
$(document).ready(function () {
    // Open the modal when the 'Add' button is clicked
    document.getElementById('addButton').addEventListener('click', function () {
        $('#addFundModal').modal('show');
    });

    // Logic for 'Add Funds' button inside the modal
    document.getElementById('addFundButton').addEventListener('click', function () {
        const amount = document.getElementById('amount').value;
        if (amount && parseFloat(amount) > 0) {
            // Call the function to add funds to the smart contract
            AddSmartContractFund(amount);
        } else {
            setTimeout(() => {
                Swal.close(); // Close any previous Swal instances
                swal.fire({
                    title: "error!",
                    text: 'Please enter valid amount:',
                    icon: "error",
                    confirmButtonText: "OK"
                });
            }, 0);
        }
    });
});

async function AddSmartContractFund(amount) {
    if (typeof loanContract == 'undefined')
        await checkMetaMaskConnection();
    else {
        if (typeof window.ethereum !== 'undefined') {
            const accounts = await ethereum.request({ method: 'eth_accounts' });
            if (accounts.length > 0) {
                Swal.fire({
                    title: "Please wait.. we are adding funds",
                    didOpen: () => {
                        Swal.showLoading();
                    },
                    allowOutsideClick: false,
                    allowEscapeKey: false
                });
                try {
                    let isprocess = await loanContract.IsLendder();
                    if (isprocess) {
                        let isprocess = await loanContract.AddfundContract(amount);
                        if (isprocess != null) {
                            let balance = await loanContract.getContractBalance();
                            if (balance != null) {
                                document.getElementById('contract-balance').textContent = "Balance : " + balance;
                                document.getElementById('current-balance').textContent = "Balance : " + balance;
                                document.getElementById('takeout-balance').textContent = "Balance : " + balance;
                          } else {
                                document.getElementById('contract-balance').textContent = "Balance : " + 0;
                          }
                            setTimeout(async () => {
                                Swal.close(); // Close any previous Swal instances
                                Swal.fire({
                                    text: `Current Smart Contract Balance is : ` + balance,
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary"
                                    }
                                }).then(async () => {  // Make this async
                                    let formData = new FormData();
                                    formData.set("FormId", "20")
                                    formData.set("Previous Balance", PreviousBalance)
                                    formData.set("Current Balance", balance)
                                   formData.set("MetaMaskID", await loanContract.getAddress())
                                    let actionUrl = "/sadmin/borrower/submit-form"
                                    await DashBoardSubmitData(actionUrl, formData);  // Now it will work
                                    PreviousBalance = balance;
                                });
                            }, 0);
                            $('#addFundModal').modal('hide');
                        } else {
                            setTimeout(() => {
                                Swal.close(); // Close any previous Swal instances
                                Swal.fire({
                                    text: `Error to add fund in smart contract`,
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