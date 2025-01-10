document.addEventListener("DOMContentLoaded", async function () {
    await checkMetaMaskConnection();
    if (isContractSetupDone) {
        let isprocess = await loanContract.IsLendder();
        if (isprocess) {
            $("#meta-container").css("display", "none");
            if ($(".dashboard-admin").length > 0)
                await GetAllLendderInfo();
            else if ($(".dashboard-user").length > 0)
                await GetAllUserInfo();
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