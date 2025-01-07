let loanContract;

window.addEventListener('load', async function () {
    await InitMeatMask();

    // Button click to log in to MetaMask
    document.getElementById('loginMetaMaskButton').addEventListener('click', async () => {
        if (typeof window.ethereum !== 'undefined') {
            try {
                await ethereum.request({ method: 'eth_requestAccounts' });
                location.reload(); // Refresh to show connected status
            } catch (err) {
                console.log('Error connecting to MetaMask:', err);
            }
        }
    });
});

async function InitMeatMask() {
    if (typeof window.ethereum !== 'undefined') {
        console.log('MetaMask is available');

        // Check if the user is already connected
        ethereum.request({ method: 'eth_accounts' })
            .then(accounts => {
                if (accounts.length > 0) {
                    // User is logged in (connected)
                    $("#meta-container").css("display", "none");
                    $("#main-container").css("display", "block");
                    GetInstance();
                    // Initialize contract interaction
                } else {
                    // User is not logged in
                    $("#meta-container").css("display", "block");
                    $("#main-container").css("display", "none");
                }
            })
            .catch(err => {
                console.log('Error fetching accounts:', err);
            });
    } else {
        console.log('MetaMask is not installed');
        $("#meta-container").css("display", "block");
        $("#main-container").css("display", "none");
    }
}
// Handle login button click (connect to MetaMask if not logged in)
document.getElementById('loginMetaMaskButton').addEventListener('click', function () {
    // Request MetaMask to connect (open MetaMask for login)
    ethereum.request({ method: 'eth_requestAccounts' })
        .then(accounts => {
            $("#meta-container").css("display", "none");
            $("#main-container").css("display", "block");
            GetInstance();
        })
        .catch(err => {
            console.log('User rejected the connection request:', err);
            $("#meta-container").css("display", "block");
            $("#main-container").css("display", "none");
        });
});

async function CheckLenderBalance() {
    let isprocess = await loanContract.IsLendder();
    if (isprocess) {
        let balance = await loanContract.getLenderBalance();
    }
    return true;
}