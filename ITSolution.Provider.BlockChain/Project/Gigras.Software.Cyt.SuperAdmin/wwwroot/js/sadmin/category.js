"use strict";
let KTCategoryList = (function () {
    let table, dataTable;

    return {
        init: function () {
            let tableElement = document.querySelector("#kt_category_table");
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
                let searchInput = document.querySelector('[data-kt-category-table-filter="search"]');
                if (searchInput) {
                    searchInput.addEventListener("keyup", function (event) {
                        dataTable.search(event.target.value).draw();
                    });
                }

                // Handle delete row functionality
                let deleteButtons = tableElement.querySelectorAll('[data-kt-category-table-filter="delete_row"]');
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

let KTList = (function () {
    let table, dataTable;

    return {
        init: function (options) {
            let tableSelector = options.tableSelector || "#kt_subcategory_table";
            let searchSelector = options.searchSelector || '[data-kt-kt_subcategory_table-table-filter="search"]';
            let deleteRowSelector = options.deleteRowSelector || '[data-kt-kt_subcategory_table-table-filter="delete_row"]';

            let tableElement = document.querySelector(tableSelector);
            if (tableElement) {
                // Initialize DataTable
                dataTable = $(tableElement).DataTable({
                    info: options.info || false,
                    order: options.order || [],
                    columnDefs: options.columnDefs || [
                        { orderable: true, targets: 1 },
                        { orderable: true, targets: 2 },
                        { orderable: false, targets: 3 }
                    ]
                });

                // Add search functionality
                let searchInput = document.querySelector(searchSelector);
                if (searchInput) {
                    searchInput.addEventListener("keyup", function (event) {
                        dataTable.search(event.target.value).draw();
                    });
                }

                // Handle delete row functionality
                let deleteButtons = tableElement.querySelectorAll(deleteRowSelector);
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
    KTCategoryList.init();

    //let totalCate = $("#totalCate").val();
    //for (let i = 1; i < totalCate; i++) { // Use let i = 1 for initialization
    //    if ($("#kt_subcategory_table_" + i).length > 0) {
    //        KTList.init({
    //            tableSelector: "#kt_subcategory_table_" + i,
    //            searchSelector: '[data-subcategory-filter_' + i + '="search"]',
    //            deleteRowSelector: '[data-subcategory-filter' + i + '="delete_row"]',
    //            info: true,
    //            order: [[1, 'asc']],
    //            columnDefs: [
    //                { orderable: true, targets: 0 },
    //                { orderable: false, targets: 3 }
    //            ]
    //        });
    //    }
    //}
});

// JavaScript function to toggle visibility of child categories
function toggleChildCategories(categoryId) {
    const childRow = document.getElementById('child-' + categoryId);
    if (childRow.style.display === 'none' || childRow.style.display === '') {
        childRow.style.display = 'table-row'; // Show child categories
    } else {
        childRow.style.display = 'none'; // Hide child categories
    }
}

// JavaScript function to toggle visibility of sub-child categories
function toggleSubChildCategories(childId) {
    const subChildRow = document.getElementById('child-child-' + childId);
    if (subChildRow.style.display === 'none' || subChildRow.style.display === '') {
        subChildRow.style.display = 'table-row'; // Show sub-child categories
    } else {
        subChildRow.style.display = 'none'; // Hide sub-child categories
    }
}

function addCategory() {
    document.getElementById("form-wrapper").style.display = "block"; // Show the form
    document.getElementById("form-heading").innerText = "Add New Category"; // Update modal title
    document.getElementById("kt_modal_add_category_form").reset(); // Clear form
    document.getElementById("Id").value = "0";
    const imageContainer = document.getElementById("current-image-container");
    imageContainer.innerHTML = "";
}

function editCategory(id) {
    window.scrollTo({
        top: 0,
        behavior: "smooth" // Adds a smooth scrolling effect
    });
    document.activeElement?.blur();
    // Show loading indicator
    Swal.showLoading();

    // Fetch data from the server
    fetch(`/api/category/${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to fetch data.");
            }
            return response.json();
        })
        .then(data => {
            // Fill the form fields
            document.getElementById("Id").value = data.id || "0";
            document.getElementById("CategoryName").value = data.categoryName || "";
            document.getElementById("CategoryType").value = data.categoryType || "";
            document.getElementById("ParentCategoryId").value = data.parentCategoryId || "";
            document.getElementById("CategoryUrl").value = data.categoryUrl || "";

            // If additional meta fields exist, populate them
            document.getElementById("MetaDescription").value = data.metaDescription || "";
            document.getElementById("MetaKeyword").value = data.metaKeyword || "";
            document.getElementById("Priority").value = data.priority || "";
            document.getElementById("Active").checked = data.active || false;
            if (data.categoryImage) {
                const imageContainer = document.getElementById("current-image-container");
                const imgElement = document.createElement("img");
                imgElement.src = data.categoryImage; // URL from the API
                imgElement.alt = "Current Category Image";
                imgElement.style.width = "200px"; // or any size you prefer
                imgElement.style.marginTop = "10px"; // for spacing
                imageContainer.innerHTML = ''; // clear previous content
                imageContainer.appendChild(imgElement); // append the image
            }
            // Update modal header
            document.getElementById("form-heading").innerText = "Edit " + data.title + " Page";

            // Display the form
            document.getElementById("form-wrapper").style.display = "block";

            // Hide loading indicator
            Swal.close();
        })
        .catch(err => {
            // Handle errors gracefully
            Swal.fire("Error", "Unable to fetch category details.", "error");
        });
}

function deleteCategory(id) {
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
                text: "Please wait while we delete the category.",
                showConfirmButton: false,
                allowOutsideClick: false,
                willOpen: () => {
                    Swal.showLoading(); // Show the loading spinner while the deletion is in progress
                }
            });

            // Send delete request to the server
            fetch(`/api/category/${id}`, { method: "DELETE" })
                .then(response => {
                    // Close the loading spinner
                    Swal.close();

                    if (response.ok) {
                        // Show success message after deletion
                        Swal.fire({
                            title: "Deleted!",
                            text: "category has been deleted.",
                            icon: "success",
                            confirmButtonText: "OK"
                        }).then(() => {
                            // Reload the page only after user clicks "OK"
                            window.location.reload();
                        });
                    } else {
                        // Show error message if deletion failed
                        Swal.fire("Error", "Unable to delete category.", "error");
                    }
                })
                .catch(err => {
                    // Close the loading spinner in case of an error
                    Swal.close();

                    // Show error message if there was a problem with the request
                    Swal.fire("Error", "Unable to delete category.", "error");
                });
        }
    });
}

document.getElementById("kt_modal_add_category_form").addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent default form submission behavior

    Swal.showLoading();

    // Validate form fields (native HTML5 validation)
    if (!this.checkValidity()) {
        Swal.fire("Error", "Please fill out all required fields.", "error");
        return; // Stop further execution
    }

    // Prepare data for submission
    const formData = new FormData(this); // Collect all form data
    const isActive = formData.get("Active") === "on"; // Convert "on" to true/false
    formData.set("Active", isActive); // Update the "active" field in FormData

    let id = document.getElementById("Id").value;
    let method = (id == 0 ? "POST" : "PUT");
    let action = (id == 0 ? `/api/category/` : `/api/category/${id}`);

    // Perform HTTP PUT request
    fetch(action, { // Use the correct endpoint
        method: method,
        body: formData
    })
        .then(response => {
            Swal.close();
            if (response.ok) {
                Swal.fire({
                    title: "Success",
                    text: "Catgeory updated successfully!",
                    icon: "success",
                    confirmButtonText: "OK"
                }).then(() => {
                    window.location.reload(); // Optionally reload the page or navigate
                });
            } else {
                throw new Error("Failed to update the category.");
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire("Error", error.message, "error");
        });
});
function hideForm() {
    document.getElementById("form-wrapper").style.display = "none"; // Show the form
    document.getElementById("kt_modal_add_category_form").reset(); // Clear form
    document.getElementById("Id").value = "0";
    const imageContainer = document.getElementById("current-image-container");
    imageContainer.innerHTML = "";

}

document.getElementById("CategoryImage").addEventListener("change", function (event) {
    const file = event.target.files[0]; // Get the selected file
    const imagePreview = document.getElementById("imagePreview");

    // Clear any existing content in the preview container
    imagePreview.innerHTML = "";

    if (file) {
        // Ensure it's an image file
        if (file.type.startsWith("image/")) {
            const reader = new FileReader();

            // Load the image as a data URL
            reader.onload = function (e) {
                const img = document.createElement("img");
                img.src = e.target.result; // Set the image source
                img.alt = "Selected Image"; // Set alt text
                img.style.maxWidth = "200px"; // Optional: Set image size
                img.style.marginTop = "10px";
                imagePreview.appendChild(img); // Add the image to the preview container
            };

            reader.readAsDataURL(file); // Read the file
        } else {
            // Show an error if the file is not an image
            imagePreview.innerHTML = "<p class='text-danger'>Please select a valid image file.</p>";
        }
    }
});
