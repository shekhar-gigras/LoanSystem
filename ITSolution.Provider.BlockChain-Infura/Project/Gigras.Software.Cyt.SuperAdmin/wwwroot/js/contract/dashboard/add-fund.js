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
    if (!IsMetaMaskLoggedIn)
        await checkMetaMaskConnection();
    else {
        try {
            const amountInEther = Web3.utils.fromWei(amount.toString(), "ether");
            let response = await callAjaxWithSwal({
                url: "/api/ethereum/prepare-transaction", // Replace with your URL
                method: 'POST',
                data: JSON.stringify({ SenderAddress: accounts[0], AmountInEther: amountInEther, FunctionName: "fundContract" }), // Replace with your data
                confirmationText: 'Do you really want to proceed?',
                successMessage: 'The operation was completed successfully!',
                errorMessage: 'Failed to complete the operation.'
            })
            await sendTransactionToMetaMask(response.result);
        }
        catch (error) {
            alert(`Transaction failed: ${error.message}`);
        }
    }
}

async function sendTransactionToMetaMask(transactionData) {
    if (!window.ethereum) {
        console.error("MetaMask is not installed!");
        return;
    }

    try {
        let web3 = new Web3(window.ethereum);

        // Extract and prepare the transaction data
        const transaction = {
            from: transactionData.from,       // Sender's address
            to: transactionData.to,           // Recipient's address
            gas: web3.utils.toHex(transactionData.gas), // Gas limit in hex
            gasPrice: web3.utils.toHex(transactionData.gasPrice), // Gas price in hex
            value: web3.utils.toHex(transactionData.value), // Value in Wei (in hex)
            data: transactionData.data,
        };

        console.log("Sending transaction to MetaMask:", transaction);

        // Request MetaMask to send the transaction
        const txHash = await window.ethereum.request({
            method: "eth_sendTransaction",
            params: [transaction],
        });

        console.log("Transaction sent! Hash:", txHash);
        alert(`Transaction sent! Hash: ${txHash}`);
    } catch (error) {
        console.error("Error sending transaction:", error);
        alert(`Transaction failed: ${error.message}`);
    }
}