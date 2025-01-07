let ckEditorInstance;
"use strict";
let KTCmsList = (function () {
    let table, dataTable;

    return {
        init: function () {
            let tableElement = document.querySelector("#kt_cms_table");
            if (tableElement) {
                // Parse date and set data-order attribute for sorting
                let rows = tableElement.querySelectorAll("tbody tr");
                //Array.prototype.forEach.call(rows, function (row) {
                //    let cells = row.querySelectorAll("td");
                //    let formattedDate = moment(cells[2].innerHTML, "DD MMM YYYY, LT").format();
                //    cells[2].setAttribute("data-order", formattedDate);
                //});

                // Initialize DataTable
                dataTable = $(tableElement).DataTable({
                    info: false,
                    order: [],
                    columnDefs: [
                        { orderable: true, targets: 1 },
                        { orderable: true, targets: 2 },
                        { orderable: false, targets: 3 }
                    ]
                });

                // Add search functionality
                let searchInput = document.querySelector('[data-kt-cms-table-filter="search"]');
                if (searchInput) {
                    searchInput.addEventListener("keyup", function (event) {
                        dataTable.search(event.target.value).draw();
                    });
                }

                // Handle delete row functionality
                let deleteButtons = tableElement.querySelectorAll('[data-kt-cms-table-filter="delete_row"]');
                Array.prototype.forEach.call(deleteButtons, function (deleteButton) {
                    deleteButton.addEventListener("click", function (event) {
                        event.preventDefault();

                        let row = event.target.closest("tr");
                        let itemName = row.querySelectorAll("td")[0].innerText;

                        Swal.fire({
                            text: "Are you sure you want to delete " + itemName + "?",
                            icon: "warning",
                            showCancelButton: true,
                            buttonsStyling: false,
                            confirmButtonText: "Yes, delete!",
                            cancelButtonText: "No, cancel",
                            customClass: {
                                confirmButton: "btn fw-bold btn-danger",
                                cancelButton: "btn fw-bold btn-active-light-primary"
                            }
                        }).then(function (result) {
                            if (result.value) {
                                Swal.fire({
                                    text: "You have deleted " + itemName + "!",
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary"
                                    }
                                }).then(function () {
                                    dataTable.row($(row)).remove().draw();
                                });
                            } else if (result.dismiss === "cancel") {
                                Swal.fire({
                                    text: itemName + " was not deleted.",
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary"
                                    }
                                });
                            }
                        });
                    });
                });
            }
        }
    };
})();

// Initialize when DOM is fully loaded
KTUtil.onDOMContentLoaded(function () {
    KTCmsList.init();
});

// Initialize CKEditor
ClassicEditor
    .create(document.querySelector('#body'))
    .then(editor => {
        ckEditorInstance = editor; // Store the instance for later use
    })
    .catch(error => {
        console.error(error);
    });

// Handle reset button click
document.getElementById('reset-button').addEventListener('click', function () {
    if (ckEditorInstance) {
        ckEditorInstance.setData(''); // Clear the CKEditor content
    }
});

function addPage() {
    document.getElementById("form-wrapper").style.display = "block"; // Show the form
    document.getElementById("form-heading").innerText = "Add New Static Page"; // Update modal title
    document.getElementById("kt_modal_add_cms_form").reset(); // Clear form
    document.getElementById("id").value = "0";
    if (ckEditorInstance) {
        ckEditorInstance.setData(""); // Set the data for CKEditor 5
    } else {
        console.error("CKEditor instance is not initialized.");
    }
}

function editPage(id) {
    window.scrollTo({
        top: 0,
        behavior: "smooth" // Adds a smooth scrolling effect
    });
    document.activeElement?.blur();
    // Show loading indicator
    Swal.showLoading();

    // Fetch data from the server
    fetch(`/api/cms/${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to fetch data.");
            }
            return response.json();
        })
        .then(data => {
            // Fill the form fields
            document.getElementById("id").value = data.id || "";
            document.getElementById("title").value = data.title || "";
            document.getElementById("description").value = data.description || "";

            if (ckEditorInstance) {
                ckEditorInstance.setData(data.body || ""); // Set the data for CKEditor 5
            } else {
                console.error("CKEditor instance is not initialized.");
            }

            // If additional meta fields exist, populate them
            document.getElementById("meta_description").value = data.metaDescription || "";
            document.getElementById("meta_keyword").value = data.metaKeyword || "";

            // Update modal header
            document.getElementById("form-heading").innerText = "Edit " + data.title + " Page";

            // Display the form
            document.getElementById("form-wrapper").style.display = "block";

            // Hide loading indicator
            Swal.close();
        })
        .catch(err => {
            // Handle errors gracefully
            Swal.fire("Error", "Unable to fetch page details.", "error");
        });
}

function deletePage(id) {
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
                text: "Please wait while we delete the page.",
                showConfirmButton: false,
                allowOutsideClick: false,
                willOpen: () => {
                    Swal.showLoading(); // Show the loading spinner while the deletion is in progress
                }
            });

            // Send delete request to the server
            fetch(`/api/cms/${id}`, { method: "DELETE" })
                .then(response => {
                    // Close the loading spinner
                    Swal.close();

                    if (response.ok) {
                        // Show success message after deletion
                        Swal.fire({
                            title: "Deleted!",
                            text: "page has been deleted.",
                            icon: "success",
                            confirmButtonText: "OK"
                        }).then(() => {
                            // Reload the page only after user clicks "OK"
                            window.location.reload();
                        });
                    } else {
                        // Show error message if deletion failed
                        Swal.fire("Error", "Unable to delete page.", "error");
                    }
                })
                .catch(err => {
                    // Close the loading spinner in case of an error
                    Swal.close();

                    // Show error message if there was a problem with the request
                    Swal.fire("Error", "Unable to delete page.", "error");
                });
        }
    });
}


document.getElementById("kt_modal_add_cms_form").addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent default form submission behavior

    Swal.showLoading();

    // Synchronize CKEditor content with the hidden textarea
    if (ckEditorInstance) {
        const editorData = ckEditorInstance.getData();
        if (!editorData.trim()) {
            event.preventDefault(); // Prevent form submission
            Swal.fire("Error", "Body content is required.", "error");
            return false;
        }
    }

    // Validate form fields (native HTML5 validation)
    if (!this.checkValidity()) {
        Swal.fire("Error", "Please fill out all required fields.", "error");
        return; // Stop further execution
    }

    // Prepare data for submission
    const formData = new FormData(this); // Collect all form data
    const jsonData = Object.fromEntries(formData.entries()); // Convert to JSON format

    const cytWebCm = {
        id: jsonData.id, // Pass the ID if editing, otherwise null or undefined
        title: jsonData.title,
        description: jsonData.description,
        body: jsonData.body,
        metaDescription: jsonData.meta_description,
        metaKeyword: jsonData.meta_keyword
    };


    let id = document.getElementById("id").value;
    let method = (id == 0 ? "POST" : "PUT");
    let action = (id == 0 ? `/api/cms/` : `/api/cms/${jsonData.id}`);

    // Perform HTTP PUT request
    fetch(action, { // Use the correct endpoint
        method: method,
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(cytWebCm) // Send JSON payload
    })
        .then(response => {
            Swal.close();
            if (response.ok) {
                Swal.fire({
                    title: "Success",
                    text: "Page updated successfully!",
                    icon: "success",
                    confirmButtonText: "OK"
                }).then(() => {
                    window.location.reload(); // Optionally reload the page or navigate
                });
            } else {
                throw new Error("Failed to update the page.");
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire("Error", error.message, "error");
        });
});

function hideForm() {
    document.getElementById("form-wrapper").style.display = "none"; // Show the form
    document.getElementById("kt_modal_add_cms_form").reset(); // Clear form
    document.getElementById("id").value = "0";

    if (ckEditorInstance) {
        ckEditorInstance.setData(""); // Set the data for CKEditor 5
    } else {
        console.error("CKEditor instance is not initialized.");
    }
}

