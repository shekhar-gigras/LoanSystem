function OpenCopyFormModal(id) {
    showLoader("Loading user data..."); // Show loader

    const formId = id;

    // Show the modal
    $("#copyFormModal").modal("show");

    // Set the hidden input field with the form ID
    $("#currentFormId").val(formId);

    // Clear the name field
    $("#newFormName").val("");
    hideLoader(); // Hide loader after data is loaded
}
$(document).ready(function () {

    $("#saveCopyButton").on("click", function () {
        showLoader("Loading user data..."); // Show loader

        // Get the new name from the input field
        const newName = $("#newFormName").val();

        // Get the current form ID from the hidden input
        const formId = $("#currentFormId").val();

        if (newName.trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Please enter a valid name for the form.!'
            });
            return;
        }
        $.post("/forms/copy", { id: formId, name: newName }, function (response) {
            hideLoader(); // Hide loader after data is loaded
            // Close the modal
            $("#copyFormModal").modal("hide"); // Close the modal (for Bootstrap or similar libraries)

            // Show success message using SweetAlert
            Swal.fire({
                icon: 'success',
                title: 'Form copied successfully!',
                text: 'The form has been successfully duplicated.',
                showConfirmButton: true
            }).then((result) => {
                // If the user clicks "OK" on the success message, reload the page
                if (result.isConfirmed) {
                    window.location.reload();  // Reloads the current page
                }
            });

        }).fail(function () {
            hideLoader(); // Hide loader if there's an error
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Failed to load user data!'
            });
        });

    });

});
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