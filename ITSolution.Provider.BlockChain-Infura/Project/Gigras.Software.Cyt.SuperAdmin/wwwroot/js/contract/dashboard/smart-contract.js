async function CheckSmartContractBalance() {
    let response = await callAjaxWithSwal({
        url: "/api/ethereum/wallet-balance", // Replace with your URL
        method: 'GET',
        data: {}, // Replace with your data
        confirmationText: 'Do you really want to proceed?',
        successMessage: 'The operation was completed successfully!',
        errorMessage: 'Failed to complete the operation.'
    })
    if (response.status) {
        document.getElementById('contract-balance').textContent = "Balance : " + response.final;
    }
}