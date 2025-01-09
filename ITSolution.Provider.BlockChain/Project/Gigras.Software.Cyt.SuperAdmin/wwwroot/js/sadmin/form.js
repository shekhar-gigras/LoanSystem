﻿$('#frmCreateAccount').on("submit", async function (e) {
    e.preventDefault(); // Prevent the default form submission
    const formData = new FormData(this); // Collect all form data, including files
    let isprocess = await loanContract.IsLendder();
    if (isprocess) {
        Swal.fire("error", "Please login as a lender", "error")
    }
    formData.set("MetaMaskID", await loanContract.getAddress());
    // Validate form fields (native HTML5 validation)
    if (!this.checkValidity()) {
        Swal.fire("Error", "Please fill out all required fields.", "error");
        return; // Stop further execution
    }

    // Prepare form data, including file inputs
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
                    const formDataArray = Array.from(formData.entries());
                    console.log(formDataArray);
                    $('#frmCreateAccount')[0].reset(); // Reset the form
                    history.back();
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

function hideLoader() {
    Swal.close();
}
function deleteRecord(id, module) {
    // Display confirmation dialog using SweetAlert2
    Swal.fire({
        title: "Are you sure?",
        text: "This action cannot be undone.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "Cancel"
    }).then(result => {
        if (result.isConfirmed) {
            // Show the loading spinner
            Swal.fire({
                title: "Deleting...",
                text: "Please wait while we delete.",
                showConfirmButton: false,
                allowOutsideClick: false,
                willOpen: () => {
                    Swal.fire({
                        title: "Loading...",
                        text: "Please wait while we fetch the data.",
                        allowOutsideClick: false,
                        didOpen: () => {
                            Swal.showLoading();
                        }
                    });
                }
            });

            // Send delete request to the server
            fetch(`/api/${module}/${id}`, { method: "DELETE" })
                .then(response => {
                    // Close the loading spinner
                    Swal.close();

                    if (response.ok) {
                        // Show success message after deletion
                        Swal.fire({
                            title: "Deleted!",
                            text: "Record has been deleted.",
                            icon: "success",
                            confirmButtonText: "OK"
                        }).then(() => {
                            // Reload the page only after user clicks "OK"
                            window.location.reload();
                        });
                    } else {
                        // Show error message if deletion failed
                        Swal.fire("Error", "Unable to delete .", "error");
                    }
                })
                .catch(err => {
                    // Close the loading spinner in case of an error
                    Swal.close();

                    // Show error message if there was a problem with the request
                    Swal.fire("Error", "Unable to delete .", "error");
                });
        }
    });
}

function toggleStatus(button, recordId, module) {
    // Get the current status from the button's data attribute
    const currentStatus = button.getAttribute('data-status');
    const newStatus = currentStatus === 'True' ? 'Inactive' : 'Active';

    Swal.fire({
        title: `Change status to ${newStatus}?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
    }).then((result) => {
        if (result.isConfirmed) {
            const payload = {
                active: (newStatus === 'Active') // Boolean value based on the new status
            };
            // Make an AJAX call to update the status
            $.ajax({
                url: `/api/${module}/active/${recordId}`, // Include ID in the route
                type: 'POST',
                data: JSON.stringify(payload), // Send payload as JSON
                contentType: 'application/json', // Content type is JSON
                success: (response) => {
                    // Update the button icon and status
                    const icon = button.querySelector('i');
                    icon.className = newStatus === 'Active'
                        ? 'fas fa-check text-success'
                        : 'fas fa-times text-danger';

                    // Update the button's data-status attribute
                    button.setAttribute('data-status', newStatus === 'Active' ? 'True' : 'False');

                    // Optionally, update the row's status cell
                    const row = button.closest('tr');
                    if (row) {
                        const statusCell = row.querySelector('td.status-column');
                        if (statusCell) {
                            statusCell.textContent = newStatus;
                        }
                    }

                    Swal.fire('Updated!', `Status changed to ${newStatus}.`, 'success').then(() => {
                        // Reload the page only after user clicks "OK"
                        window.location.reload();
                    });
                },
                error: () => {
                    Swal.fire('Error!', 'Failed to update status. Please try again.', 'error');
                },
            });
        }
    });
}