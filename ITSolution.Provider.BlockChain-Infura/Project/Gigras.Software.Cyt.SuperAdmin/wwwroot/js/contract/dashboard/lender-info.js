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
                        await GetContractFund();
                        await GetLenderBalance();
                        await GetTotalActiveBorrower();
                        await GetLenderAddress();
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

async function GetAllUserInfo() {
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
                    await GetContractFund();
                    await GetTotalActiveBorrower();
                    Swal.close();
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
async function GetShortLendderInfo() {
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
                    await GetContractFund();
                    await GetTotalActiveBorrower();
                    Swal.close();
                }
                catch {
                }
            }
        }
    }
}

async function GetContractFund() {
    let balance = await loanContract.getContractBalance();
    balance = balance || 0; // Set to 0 if balance is null or undefined
    const elements = {
        "contract-balance": `Balance : ${balance}`,
        "current-balance": `Balance : ${balance}`,
        "takeout-balance": `Balance : ${balance}`,
        "header-smart-contract-balance": `Smart Contract Balance : ${balance}`
    };

    Object.keys(elements).forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = elements[id];
        }
    });

    // Update PreviousBalance if balance is valid
    PreviousBalance = balance;
    $("#header-smart-contract-balance").css("display", "block");
}

async function GetLenderBalance() {
    let balance = await loanContract.getLenderBalance();
    if (balance != null) {
        document.getElementById('lendder-balance').textContent = "Balance : " + balance;
    }
    else {
        document.getElementById('lendder-balance').textContent = "Balance : " + 0;
    }
}

async function GetRequestBorrower() {
    let totalborrower = await loanContract.getRequestedBorrowers();
    if (totalborrower != null) {
        document.getElementById('requested-borrower').textContent = "Total : " + totalborrower.length;
    }
    else {
        document.getElementById('requested-borrower').textContent = "Total : " + 0;
    }
}

async function GetTotalActiveBorrower() {
    let totalborrower = await loanContract.getLoanIds();
    const count = totalborrower ? totalborrower.length : 0; // Get count or default to 0

    const elements = {
        "active-borrower": `Total : ${count}`,
        "header-total-active-borrow": `Total Active Borrow : ${count}`
    };

    Object.keys(elements).forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = elements[id];
        }
    });
    $("#header-total-active-borrow").css("display", "block");
}

async function GetLenderAddress() {
    let lenderaddress = await loanContract.getlendderAddress();
    if (lenderaddress != null) {
        PrevioudLender = lenderaddress;
        document.getElementById('lendder-address').textContent = "" + lenderaddress;
    }
    else {
        document.getElementById('lendder-address').textContent = "";
    }
}