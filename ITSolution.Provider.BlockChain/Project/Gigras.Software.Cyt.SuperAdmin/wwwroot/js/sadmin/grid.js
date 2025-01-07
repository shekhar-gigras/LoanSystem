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

// Initialize when DOM is fully loaded
KTUtil.onDOMContentLoaded(function () {
    KTCategoryList.init();
});