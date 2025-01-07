$('#frmCreateAccount').on("submit", async function (e) {
    e.preventDefault(); // Prevent the default form submission
    let entity = $("#Entity").val();
    if (entity.toLowerCase() == "borrowbalance") {
        let isprocess = await loanContract.IsLendder();
        if (isprocess) {
            let borrowerID = $("#BorrowerID").val();
            let balance = await loanContract.getBorrowerBalance(borrowerID);
        }
        return true;
    }

    if (entity.toLowerCase() == "lenderbalance") {
        let isprocess = await loanContract.IsLendder();
        if (isprocess) {
            let balance = await loanContract.getLenderBalance();
        }
        return true;
    }

    if (entity.toLowerCase() == "smartcontractbalance") {
        let isprocess = await loanContract.IsLendder();
        if (isprocess) {
            let balance = await loanContract.getContractBalance();
        }
        return true;
    }

    if (entity.toLowerCase() == "addfund") {
        let isprocess = await loanContract.IsLendder();
        if (isprocess) {
            let fundValue = $("#AddContractFund").val();
            await loanContract.AddfundContract(fundValue);
        }
        return true;
    }

    if (entity.toLowerCase() == "takeoutcontractfund") {
        let isprocess = await loanContract.IsLendder();
        if (isprocess) {
            await loanContract.takeOutFunds();
        }
        return true;
    }

    if (entity.toLowerCase() == "addfund") {
        let fundValue = $("#AddContractFund").val();
        let isprocess = await loanContract.AddfundContract(fundValue);
        if (!isprocess) {
            return false;
        }
    }
    else if (entity.toLowerCase() == "addlender") {
        let clientId = $("#ClientId").val();
        let isprocess = await loanContract.changeLender(clientId);
        if (!isprocess) {
            return false;
        }
    }
    else if (entity.toLowerCase() == "borrowbalance") {
        let borrowerID = $("#BorrowerID").val();
        let balance = await loanContract.getBorrowerBalance(borrowerID);
        if (balance == null) {
            return false;
        } else {
            $("#BorrowBalance").val(balance);
        }
    }
    else if (entity.toLowerCase() == "lenderbalance") {
        let balance = await loanContract.getLenderBalance();
        if (balance == null) {
            return false;
        } else {
            $("#LenderBalance").val(balance);
        }
    }
    else if (entity.toLowerCase() == "smartcontractbalance") {
        let balance = await loanContract.getContractBalance();
        if (balance == null) {
            return false;
        } else {
            $("#SmartContractBalance").val(balance);
        }
    }
    else if (entity.toLowerCase() == "borrowduedate") {
        let borrowerID = $("#BorrowerID").val();
        let loanDetails = await loanContract.getLoanDetails(borrowerID);
        if (loanDetails == null) {
            return false;
        } else {
            $("#BorrowDueDate").val(loanDetails["RepaymentDueDate"]);
        }
    }
    else if (entity.toLowerCase() == "borrowinterestrate") {
        let borrowerID = $("#BorrowerID").val();
        let loanDetails = await loanContract.getLoanDetails(borrowerID);
        if (loanDetails == null) {
            return false;
        } else {
            $("#BorrowInterestrate").val(loanDetails["InterestRate"]);
        }
    }
    else if (entity.toLowerCase() == "borrowloanamount") {
        let borrowerID = $("#BorrowerID").val();
        let loanDetails = await loanContract.getLoanDetails(borrowerID);
        if (loanDetails == null) {
            return false;
        } else {
            $("#BorrowBalance").val(loanDetails["LoanAmount"]);
        }
    }
    else if (entity.toLowerCase() == "borrow-form") {
        let MetaMaskID = $("#MetaMaskID").val();
        let LoanAmount = $("#LoanAmount").val();
        let Duration = $("#Duration").val();
        let InterestRate = $("#InterestRate").val();
        let dur = parseInt(Duration) * 30 * 24 * 60 * 60;
        let status = await loanContract.requestLoan(LoanAmount, InterestRate, dur, MetaMaskID);
        if (status == false) {
            return false;
        }
    }
    // Validate form fields (native HTML5 validation)
    if (!this.checkValidity()) {
        Swal.fire("Error", "Please fill out all required fields.", "error");
        return; // Stop further execution
    }

    // Prepare form data, including file inputs
    const formData = new FormData(this); // Collect all form data, including files
    const actionUrl = $(this).attr("action"); // Get the form's action URL
    showLoader();

    // Perform the AJAX request
    $.ajax({
        url: actionUrl, // Use the form's action attribute
        type: "POST", // Form's method (POST in this case)
        data: formData,
        processData: false, // Prevent jQuery from processing the data
        contentType: false, // Prevent jQuery from setting the Content-Type header
        success: function (response) {
            hideLoader();
            // Handle the success response
            Swal.fire("Success", "Form submitted successfully.", "success")
                .then(() => {
                    // Reset the form and optionally reload the page
                    //window.location.reload(); // Reload the page (optional)
                    let entityname = ($("#Entity").length > 0 ? $("#Entity").val() : "");
                    if (entity.toLowerCase() == "smartcontractbalance" || entity.toLowerCase() == "lenderbalance" || entity.toLowerCase() == "borrowbalance" || entity.toLowerCase() == "borrowduedate" || entity.toLowerCase() == "borrowinterestrate" || entity.toLowerCase() == "borrowloanamount") {
                    } else {
                        $('#frmCreateAccount')[0].reset(); // Reset the form
                        history.back();
                    }
                });
        },
        error: function (xhr, status, error) {
            // Handle the failure response
            hideLoader();
            Swal.fire("Error", xhr.responseText || "An error occurred during submission.", "error");
        }
    });
});

// Function to validate TinyMCE editors and set form data
function validateAndSetTinyMCEEditors(formData) {
    let isValid = true;

    // Loop through all TinyMCE editors
    tinymce.editors.forEach(editor => {
        const editorId = editor.id; // Get the ID of the TinyMCE instance
        const content = editor.getContent().trim(); // Get the trimmed content
        const editorElement = document.getElementById(editorId); // Get the original textarea or input

        // Check if the editor has the 'required' attribute
        if (editorElement.hasAttribute("required")) {
            const errorMessage = editorElement.getAttribute("data-error") || `Please enter a value for ${editorId}`;

            if (!content) {
                Swal.fire("Error", errorMessage, "error");
                isValid = false; // Mark as invalid
                return; // Skip further processing for this editor
            }
        }

        // If valid, add content to formData
        formData.set(editorId, content);
    });

    return isValid; // Return validation status
}

$('#btnRegister').on("keydown", function (e) {
    if (e.key === "Enter") {
        e.preventDefault(); // Prevent default form submission behavior
        $("#frmCreateAccount").trigger('submit'); // Trigger form submission
    }
});

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