let loanContract;
let isContractSetupDone = false;
let IsMetaMaskLoggedIn = false;
let accounts;
async function checkMetaMaskConnection() {
    if (typeof window.ethereum !== 'undefined') {
        console.log('MetaMask is available');
        try {
            accounts = await ethereum.request({ method: 'eth_accounts' });
            if (accounts.length > 0) {
                IsMetaMaskLoggedIn = true;
                $("#meta-container").css("display", "none");
            }
            else {
                await ethereum.request({ method: 'eth_requestAccounts' });
                accounts = await ethereum.request({ method: 'eth_accounts' });
                if (accounts.length > 0) {
                    IsMetaMaskLoggedIn = true;
                }
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
                    window.location.reload();
                } catch (err) {
                    console.log('Error connecting to MetaMask:', err);
                }
            }
        });
    }
});