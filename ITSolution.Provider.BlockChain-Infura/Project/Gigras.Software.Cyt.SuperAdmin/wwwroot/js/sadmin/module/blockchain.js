let web3;
let parsedABI;
let contractAddress;
let contractInstance;
let smartcontractFund;
let metamasaddress;
let loanContract;
let provider;

window.addEventListener('load', function () {
    if (typeof window.ethereum !== 'undefined') {
        console.log('MetaMask is available');

        // Check if the user is already connected
        ethereum.request({ method: 'eth_accounts' })
            .then(accounts => {
                if (accounts.length > 0) {
                    // User is logged in (connected)
                    metamasaddress = accounts[0];
                    document.getElementById('MetaMaskID').value = `${accounts[0]}`;
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

// Handle login button click (connect to MetaMask if not logged in)
document.getElementById('loginMetaMaskButton').addEventListener('click', function () {
    // Request MetaMask to connect (open MetaMask for login)
    ethereum.request({ method: 'eth_requestAccounts' })
        .then(accounts => {
            metamasaddress = accounts[0];
            document.getElementById('MetaMaskID').value = `${accounts[0]}`;
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

async function GetInstance() {
    try {
        if (typeof window.ethereum !== 'undefined') {
            web3 = new Web3(window.ethereum);

            // Request user accounts
            const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
            const userAddress = accounts[0];
            console.log('User Address:', userAddress);

            try {
                // Fetch ABI and contract address concurrently
                const [responseabi, responsecontract] = await Promise.all([
                    $.ajax({ url: '/api/ethereum/abi', method: 'GET' }),
                    $.ajax({ url: '/api/ethereum/contract', method: 'GET' })
                ]);

                contractAddress = responsecontract.contractAddress;
                parsedABI = JSON.parse(responseabi.abi);
                contractInstance = new web3.eth.Contract(parsedABI, contractAddress);
                let address = await web3.eth.getAccounts();
                provider = window.ethereum;
                loanContract = new LoanContract(contractAddress, parsedABI, provider);
                metamasaddress = address[0];
                if (await !Validation()) {
                    return false;
                }
                // Validate
            } catch (error) {
                console.error('Error fetching ABI or contract:', error);
            }
        } else {
            console.error('MetaMask is not available.');
        }
    } catch (error) {
        console.error('Error connecting to the contract:', error instanceof Error ? error.message : error);
    }
}

async function Validation() {
    let entity = $("#Entity").val();
    if (entity.toLowerCase() == "addfund" || entity.toLowerCase() == "addlender" || entity.toLowerCase() == "smartcontractbalance") {
        let isprocess = await loanContract.IsLendder();
        if (isprocess) {
            if (entity.toLowerCase() == "addfund") {
                await AddFund();
            }
            if (entity.toLowerCase() == "smartcontractbalance") {
                await SmartContractBalance();
            }
        }
        return isprocess;
    }
}

async function AddFund() {
    smartcontractFund = await loanContract.getContractBalance();
    $("label[for='AddContractFund']").each(function () {
        // Check if old text is already stored in data attribute
        const oldText = $(this).data('old-text') || $(this).text();  // Get the old text, or fallback to current text if not available

        // Store the old text in the data attribute
        $(this).data('old-text', oldText);

        // Update the label text with the old text and smart contract fund
        $(this).text(`${oldText} (Previous Balance: ${smartcontractFund} Wei)`);  // Append the new balance to it
    });
}

async function SmartContractBalance() {
    smartcontractFund = await loanContract.getContractBalance();
    $('<span>', {
        id: 'balanceValue',
        text: ":" + smartcontractFund+"Wei",
        css: { color: 'green' }
    }).appendTo('h3.mb-1'); // Replace 'h3.mb-1' with the appropriate selector if needed
}

function showLoader(message = "Please wait...") {
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
function hideLoader() {
    Swal.close();
}