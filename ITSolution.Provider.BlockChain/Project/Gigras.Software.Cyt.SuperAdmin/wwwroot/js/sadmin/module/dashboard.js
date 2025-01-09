let loanContract;
let isContractSetupDone = false;
async function checkMetaMaskConnection() {
    if (typeof window.ethereum !== 'undefined') {
        console.log('MetaMask is available');
        try {
            const accounts = await ethereum.request({ method: 'eth_accounts' });
            if (accounts.length > 0) {
                await GetInstance();
            } else {
                await connectMetaMask();
            }
        } catch {
            swal.fire({
                title: "error!",
                text: "Error fetching meta mask account - ",
                icon: "error",
                confirmButtonText: "OK"
            });
        }
    } else {
        swal.fire({
            title: "error!",
            text: "Please install Meta Mask Pluging",
            icon: "error",
            confirmButtonText: "OK"
        });
    }
}

window.addEventListener('load', function () {
    if ($("#loginMetaMaskButton").length > 0) {
        // Button click to log in to MetaMask
        document.getElementById('loginMetaMaskButton').addEventListener('click', async () => {
            if (typeof window.ethereum !== 'undefined') {
                try {
                    await ethereum.request({ method: 'eth_requestAccounts' });
                    if (isContractSetupDone) {
                        let isprocess = await loanContract.IsLendder();
                        if (isprocess) {
                            $("#meta-container").css("display", "none");
                            await GetAllLendderInfo();
                        }
                        else {
                            $("#meta-container").css("display", "block");
                            isContractSetupDone = false;
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
                } catch (err) {
                    console.log('Error connecting to MetaMask:', err);
                }
            }
        });
    }
});
// Function to prompt user to connect to MetaMask
async function connectMetaMask() {
    try {
        await ethereum.request({ method: 'eth_requestAccounts' });
        await GetInstance();
    } catch {
        swal.fire({
            title: "error!",
            text: "User rejected the connection request:",
            icon: "error",
            confirmButtonText: "OK"
        });
    }
}

// Function to get the contract instance (assuming this function is implemented elsewhere)
async function GetInstance() {
    try {
        if (typeof window.ethereum !== 'undefined') {
            try {
                const [responseabi, responsecontract] = await Promise.all([
                    $.ajax({ url: '/api/ethereum/abi', method: 'GET' }),
                    $.ajax({ url: '/api/ethereum/contract', method: 'GET' })
                ]);

                let contractAddress = responsecontract.contractAddress;
                let parsedABI = JSON.parse(responseabi.abi);
                let provider = window.ethereum;
                loanContract = new LoanContract(contractAddress, parsedABI, provider);
                isContractSetupDone = true;
            } catch {
                swal.fire({
                    title: "error!",
                    text: "Error fetching ABI or contract:",
                    icon: "error",
                    confirmButtonText: "OK"
                });
            }
        } else {
            swal.fire({
                title: "error!",
                text: "MetaMask is not available.",
                icon: "error",
                confirmButtonText: "OK"
            });
        }
    } catch {
        swal.fire({
            title: "error!",
            text: 'Error connecting to the contract:',
            icon: "error",
            confirmButtonText: "OK"
        });
    }
}

document.addEventListener("DOMContentLoaded", async function () {
    await checkMetaMaskConnection();
    if (isContractSetupDone) {
        let isprocess = await loanContract.IsLendder();
        if (isprocess) {
            $("#meta-container").css("display", "none");
            if ($(".dashboard").length > 0)
                await GetAllLendderInfo();
            else
                await GetShortLendderInfo();

        }
        else {
            $("#meta-container").css("display", "block");
            isContractSetupDone = false;
        }
    }
});

async function DashBoardSubmitData(actionUrl, formData) {
    $.ajax({
        url: actionUrl, // Use the form's action attribute
        type: "POST", // Form's method (POST in this case)
        data: formData,
        processData: false, // Prevent jQuery from processing the data
        contentType: false, // Prevent jQuery from setting the Content-Type header
        success: function (response) {
        },
        error: function (xhr, status, error) {
            // Handle the failure response
        }
    });
}