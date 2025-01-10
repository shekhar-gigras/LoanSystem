// Function to show SweetAlert2 loading spinner
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

$(document).ready(function () {
    if ($("#header-smart-contract-balance").length > 0)
        $("#header-smart-contract-balance").css("display", "none");
    if ($("#header-total-active-borrow").length > 0)
        $("#header-total-active-borrow").css("display", "none");
});