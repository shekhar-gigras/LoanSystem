let PrevioudLender = "";
$(document).ready(function () {
    // Open the modal when the 'Add' button is clicked
    document.getElementById('changeLendderButton').addEventListener('click', function () {
        $('#changeLenderModal').modal('show');
    });

    // Logic for 'Add Funds' button inside the modal
    document.getElementById('changeLenderButton').addEventListener('click', function () {
        const address = document.getElementById('newLenderAddress').value;
        if (address && address.trim() !== "") {
            // Call the function to add funds to the smart contract
            ChangeSmartContractLendder(address)
        } else {
            setTimeout(() => {
                Swal.close(); // Close any previous Swal instances
                swal.fire({
                    title: "error!",
                    text: 'Please enter New Lendder Address',
                    icon: "error",
                    confirmButtonText: "OK"
                });
            }, 0);
        }
    });
});

async function ChangeSmartContractLendder(address) {
    if (typeof loanContract == 'undefined')
        await checkMetaMaskConnection();
    else {
        if (typeof window.ethereum !== 'undefined') {
            const accounts = await ethereum.request({ method: 'eth_accounts' });
            if (accounts.length > 0) {
                Swal.fire({
                    title: "Please wait.. we are changine lendder",
                    didOpen: () => {
                        Swal.showLoading();
                    },
                    allowOutsideClick: false,
                    allowEscapeKey: false
                });
                try {
                    let isprocess = await loanContract.IsLendder();
                    if (isprocess) {
                        let isprocess = await loanContract.changeLender(address);
                        if (isprocess) {
                            setTimeout(() => {
                                Swal.close(); // Close any previous Swal instances
                                Swal.fire({
                                    text: `Lendder change success fully`,
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary"
                                    }
                                }).then(async () => {  // Make this async
                                    let formData = new FormData();
                                    formData.set("FormId", "24")
                                    formData.set("Previous Lender", PrevioudLender)
                                    formData.set("Change Lender", address)
                                    formData.set("MetaMaskID", await loanContract.getAddress())
                                    let actionUrl = "/sadmin/borrower/submit-form"
                                    await DashBoardSubmitData(actionUrl, formData);  // Now it will work
                                });
                            }, 0);
                            $('#changeLenderModal').modal('hide');
                            let lenderaddress = await loanContract.getlendderAddress();
                            if (lenderaddress != null) {
                                document.getElementById('lendder-address').textContent = "" + lenderaddress;
                            }
                            else {
                                document.getElementById('lendder-address').textContent = "";
                            }
                        } else {
                            document.getElementById('lendder-address').textContent = "";
                            setTimeout(() => {
                                Swal.close(); // Close any previous Swal instances
                                Swal.fire({
                                    text: `Error for change lendder`,
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