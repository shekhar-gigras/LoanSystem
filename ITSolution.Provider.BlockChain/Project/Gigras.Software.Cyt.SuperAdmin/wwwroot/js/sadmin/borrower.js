"use strict";
let KTCategoryList = (function () {
    let table, dataTable;

    return {
        init: function () {
            let tableElement = document.querySelector("#kt_category_table");
            if (tableElement) {
                // Parse date and set data-order attribute for sorting
                let rows = tableElement.querySelectorAll("tbody tr");
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
                let searchInput = document.querySelector('[data-kt-category-table-filter="search"]');
                if (searchInput) {
                    searchInput.addEventListener("keyup", function (event) {
                        dataTable.search(event.target.value).draw();
                    });
                }
            }
        }
    };
})();

let FieldArray = [
    "SystemFormId",
    "UserId",
    "Id",
    "Entity",
    "CreatedBy",
    "CreatedDate",
    "ModifiedBy",
    "ModifiedDate",
    "IsActive",
    "IsDelete",
    "CountryId",
    "StateMaster",
    "CityMaster",
    "StateId",
    "RecordId",
    "IsDeleted",
    "ParentId",
    "MetaMaskID",
    "CreatedAt",
    "UpdatedAt",
    "UpdatedBy"
];
$(document).ready(function () {
    const baseUrl = "/sadmin/Borrower";

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

    // Fetch and display all user data
    function loadUserData() {
        showLoader("Loading Data..."); // Show loader
        $.get(baseUrl + "/get-userdata-list?csc=" + csc, function (data) {
            const tbody = $("#kt_category_table tbody");
            tbody.empty(); // Clear existing rows
            if (data.entity == "smartcontractabi") {
                DisplayABIData(tbody, data);
            }
            else if (data.entity == "smartcontractaddress") {
                DisplayContractAddressData(tbody, data);
            }
            else if (data.entity!= null && data.entity.toLowerCase() == "loandetails") {
                DisplayLoanData(tbody, data);
            }
            else if (data.fallbackData.length > 0) {
                DisplayUserData(tbody, data.fallbackData)
            }
            KTCategoryList.init();

            hideLoader(); // Hide loader after data is loaded
        }).fail(function () {
            hideLoader(); // Hide loader if there's an error
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Failed to load user data!'
            });
        });
    }

    // Fetch JSON data and display in modal
    $(document).on("click", ".view-json", function () {
        const id = $(this).data("id");
        showLoader("Loading Data..."); // Show loader while loading JSON data

        $.get(`${baseUrl}/get-user-data?id=${id}&csc=` + csc, function (data) {
            const jsonData = JSON.parse(data); // Parse JSON data
            let table = "";
            if (csc == "SmartContractABI") {
                table = createAbiTableFromJson(jsonData); // Generate table HTML
            }
            else if (csc == "SmartContractAddress") {
                table = createContractAddressTableFromJson(jsonData); // Generate table HTML
            }
            else if (csc.toLowerCase() == "fixed-adjustable-rate-note") {
                table = createLoanDataTableFromJson(jsonData);
            }
            else {
                table = createTableFromJson(jsonData); // Generate table HTML
            }

            $("#jsonData").html(table); // Inject table into modal body
            $("#jsonModal").modal("show");

            hideLoader(); // Hide loader after JSON data is loaded
        }).fail(function () {
            hideLoader(); // Hide loader if there's an error
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Failed to load User Data!'
            });
        });
    });

    // Initial load
    loadUserData();
});

// Function to handle value formatting
function formatValue(value) {
    if (Array.isArray(value)) {
        // Join array values with a comma for display
        return value.join(", ");
    } else if (typeof value === "object" && value !== null) {
        // Recursively generate a nested table for objects
        return createTableFromJson(value);
    } else {
        // Return the value as-is for primitive types
        return value;
    }
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