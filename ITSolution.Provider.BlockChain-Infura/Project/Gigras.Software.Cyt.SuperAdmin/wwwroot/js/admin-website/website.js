document.addEventListener("DOMContentLoaded", async function () {
    await checkMetaMaskConnection();
    if (isContractSetupDone) {
        let address = await loanContract.getAddress();
        if (address != null) {
            $("#meta-container").css("display", "none");
            await GetShortLendderInfo();
        }
        else {
            $("#meta-container").css("display", "block");
            isContractSetupDone = false;
        }
    }
});