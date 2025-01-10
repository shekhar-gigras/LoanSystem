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

// SweetAlert for Delete/Retrieve Confirmation
function confirmDeleteOrRetrieve(url, isDelete) {
    const deleteAction = isDelete === true; // Convert string to boolean
    Swal.fire({
        title: 'Are you sure?',
        text: deleteAction ? 'Do you really want to retrieve this item?' : 'Do you really want to delete this item?',
        icon: deleteAction ? 'question' : 'warning',
        showCancelButton: true,
        confirmButtonColor: deleteAction ? '#3085d6' : '#d33',
        cancelButtonColor: '#d33',
        confirmButtonText: deleteAction ? 'Yes, retrieve it!' : 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url;
        }
    });
}


// SweetAlert for Toggle Status Confirmation
function confirmToggle(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to toggle the status of this item?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, toggle it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url;
        }
    });
}

