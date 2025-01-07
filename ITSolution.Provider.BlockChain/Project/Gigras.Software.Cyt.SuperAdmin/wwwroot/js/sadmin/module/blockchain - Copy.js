let web3;
let parsedABI;
let contractAddress;
let contractInstance;
let smartcontractFund;

window.addEventListener('load', function () {
    if (typeof window.ethereum !== 'undefined') {
        console.log('MetaMask is available');

        // Check if the user is already connected
        ethereum.request({ method: 'eth_accounts' })
            .then(accounts => {
                if (accounts.length > 0) {
                    // User is logged in (connected)
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

                const contractAddress = responsecontract.contractAddress;
                const parsedABI = JSON.parse(responseabi.abi);
                const contractInstance = new web3.eth.Contract(parsedABI, contractAddress);

                // Call contract method
                const isoverdue = await contractInstance.methods.isRepaymentOverdue(userAddress).call();
                console.log('Is repayment overdue:', isoverdue);

                // Validate
                if (!Validation()) {
                    return false;
                }
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
    if (entity.toLowerCase() == "addfund" && !CheckLenderLoggedIn()) {
        return false;
    }
    return true;
}

async function CheckLenderLoggedIn() {
    try {
        smartcontractFund = await contractInstance.methods.checkBalanceOfSmartContract().call();
        return true;
    } catch (error) {
        if (error.message.includes("execution reverted")) {
            const errorMessage = extractRevertReason(error.message);
            Swal.fire({
                text: errorMessage,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    //location.href = "/sadmin";
                }
            });
        } else {
            Swal.fire({
                text: error.message,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    //location.href = "/sadmin";
                }
            });
        }
        return false;
    }
}

function extractRevertReason(errorMessage) {
    // Extract the revert reason from the error message
    const match = errorMessage.match(/execution reverted: (.*)/);
    return match ? match[1] : 'Unknown error';
}

async function CallAPI() {
    let abi = '';
    let contract = '';
    if (typeof window.ethereum !== 'undefined') {
        console.log('MetaMask is available');

        // Request user accounts
        window.ethereum.request({ method: 'eth_requestAccounts' })
            .then(accounts => {
                const userAddress = accounts[0];
                console.log('User Address:', userAddress);

                // Fetch ABI and contract address concurrently
                Promise.all([
                    $.ajax({ url: '/api/ethereum/abi', method: 'GET' }),
                    $.ajax({ url: '/api/ethereum/contract', method: 'GET' })
                ])
                    .then(([responseabi, responsecontract]) => {
                        abi = responseabi;
                        contract = responsecontract;
                        connectToContract(abi, contract);
                    })
                    .catch(error => {
                        console.error('Error fetching ABI or contract:', error);
                    });
            })
            .catch(err => {
                console.error('Error connecting to MetaMask:', err);
            });
    } else {
        console.log('MetaMask is not available. Please install it.');
    }
}

async function connectToContract(abi, contract) {
    try {
        if (typeof window.ethereum !== 'undefined') {
            const web3 = new Web3(window.ethereum);
            const parsedABI = JSON.parse(abi);
            const contractInstance = new web3.eth.Contract(parsedABI, contract);

            // Fetch contract details
            const owner = await contractInstance.methods.lender().call();
            //const checkBalBorrower = await contractInstance.methods.checkBalBorrower().call();
            //const checkBalLender = await contractInstance.methods.checkBalLender().call();

            const isLoanRepaid = await contractInstance.methods.isLoanRepaid().call();
            const isLoanFunded = await contractInstance.methods.isLoanFunded().call();

            // Update frontend
        } else {
            console.log('MetaMask is not available');
        }

        // Optionally send data to the backend
        //$.ajax({
        //    url: '/api/contract/save',
        //    method: 'POST',
        //    contentType: 'application/json',
        //    data: JSON.stringify({ owner, isLoanRepaid, isLoanFunded }),
        //    success: (response) => {
        //        console.log('Data saved successfully:', response);
        //    },
        //    error: (xhr, status, error) => {
        //        console.error('Error saving data:', error);
        //    }
        //});
    } catch (error) {
        console.error('Error connecting to the contract:', error instanceof Error ? error.message : error);
    }
}




function testing() {
    const web3 = new Web3(window.ethereum);

    // Example: Sending ETH
    const transaction = {
        from: userAccount,  // Sender's address (retrieved from MetaMask)
        to: '0xReceiverAddress',  // Receiver's address
        value: web3.utils.toWei('0.1', 'ether')  // Amount to send in Wei
    };

    web3.eth.sendTransaction(transaction)
        .then(receipt => {
            console.log("Transaction successful:", receipt);
        })
        .catch(error => {
            console.log("Transaction failed:", error);
        });
}