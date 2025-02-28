function DisplayAdminUserData(tbody, data, isadmin) {
    const thead = $("#kt_category_table thead");
    thead.empty(); // Clear existing rows
    thead.append(`
                        <tr class="text-start text-gray-400 fw-bolder fs-7 text-uppercase gs-0">
                                <th class="min-w-125px">#</th>
                                 <th>Status</th>
                                <th>Lender Name</th>
                                 <th>Lender Email</th>
                                 <th>Lender Phone</th>
                              <th class="text-end min-w-50px">Actions</th>
                            </tr>
                     `);
    data.data.forEach(user => {
        // Append the user data to the table
        tbody.append(`
                    <tr>
                        <td>${user.id}</td>
                        <td>
                            <div class="symbol symbol-circle symbol-50px overflow-hidden me-3">
                                <div class="symbol-label" style="background-color: ${user.isActive ? 'green' : (user.isDelete ? 'red' : 'darkred')};">
                                    <span style="color: ${user.isActive ? 'white' : (user.isDelete ? 'white' : 'white')}; font-weight: bold;">
                                        ${user.isActive ? "A" : (user.isDelete ? "D" : "B")}
                                    </span>
                                </div>
                            </div>
                        </td>
                        <td>${user.userName}</td>
                        <td>${user.email}</td>
                        <td>${user.phone}</td>
                        <td>
                            <button
                                style="background-color: ${user.isActive ? 'green' : 'red'}; color: white; font-size: 10px; border: none; border-radius: 5px;" data-status='${user.isActive}'
                                onclick="toggleUserStatus(this,'${user.userId}', 'portaluser','active')">
                                Active
                            </button>
                            <button
                                style="background-color: ${!user.isDelete ? 'green' : 'red'}; color: white; font-size: 10px; border: none; border-radius: 5px;" data-status='${user.isDelete}'
                                onclick="toggleUserStatus(this,'${user.userId}', 'portaluser','delete')">
                                Delete
                            </button>
                            <button
                                style="background-color: ${!user.isBlock ? 'green' : 'red'}; color: white; font-size: 10px; border: none; border-radius: 5px;"
                                onclick="toggleUserStatus(this,'${user.userId}', 'portaluser','block')">
                                Block
                            </button>
                            <button
                                style="background-color: ${user.isAddLoan ? 'green' : 'red'}; color: white; font-size: 10px; border: none; border-radius: 5px;" data-status='${user.isAddLoan}'
                                onclick="toggleUserStatus(this,'${user.userId}', 'portaluser','addloanstatus')">
                                Permit Add Loan
                            </button>
                            <button
                                style="background-color: ${user.isEditLoan ? 'green' : 'red'}; color: white; font-size: 10px; border: none; border-radius: 5px;" data-status='${user.isEditLoan}'
                                onclick="toggleUserStatus(this,'${user.userId}', 'portaluser','editloanstatus')">
                                Permit Edit Loan
                            </button>
                            <button
                                style="background-color: ${user.isDeleteLoan ? 'green' : 'red'}; color: white; font-size: 10px; border: none; border-radius: 5px;" data-status='${user.isDeleteLoan}'
                                onclick="toggleUserStatus(this,'${user.userId}', 'portaluser','deleteloanstatus')">
                                Permit Delete Loan
                            </button>
                            <button
                                style="background-color: ${user.isVisibleLoanSale ? 'green' : 'red'}; color: white; font-size: 10px; border: none; border-radius: 5px;" data-status='${user.isVisibleLoanSale}'
                                onclick="toggleUserStatus(this,'${user.userId}', 'portaluser','visibleloanstatus')">
                                Permit Visisble Sale Loan
                            </button>
                        </td >
                    </tr >
        `);
    });
    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');
        const baseUrl = `/ sadmin / Borrower`;
        let formgroup = $("#kt_category_table").data("formgroup");
        let formid = $("#kt_category_table").data("formid");
        const basedataUrl = `${baseUrl} / ${formgroup} / ${formid}`;
        location.href = `${basedataUrl} / edit - form / ${id}`;
    });
}

// Function to create an HTML table from JSON
function createAdminUserDataTableFromJson(jsonData) {
    let table = `
    < div class= "row" >
    `;

    // Create two columns
    Object.entries(jsonData.data).forEach(([key, value], index) => {
        if (FieldArray.includes(key)) {
            return;
        }
        const formattedKey = key.replace(/_/g, " ").replace(/^\w/, (c) => c.toUpperCase());
        let keydesc = findDescriptionByFieldName(jsonData.formfields, key);
        if (keydesc == null) {
            keydesc = formattedKey;
        }

        // Start a new column every 2 fields (making it two columns)
        //if (index % 2 === 0) {
        //table += `< div class= "col-md-6 col-sm-12" > `;  // Use 6 columns in medium screens (2 columns) and 12 in small screens (1 column)
        //}

        table += `
    < div class= "col-4 mb-4" >
                    <label><strong>${keydesc}</strong></label>
                    <p>${formatValue(value)}</p>
                </div >
        `;

        // Close column after every 2 fields
        //if (index % 2 === 1) {
        // table += `</div > `;  // Close the column div
        //}
    });

    // Close last column if necessary
    //if (Object.entries(jsonData).length % 2 !== 0) {
    // table += `</div > `; // Close the last open column div
    //}

    table += `</div > `; // Close the row div
    return table;
}

function toggleUserStatus(button, recordId, module, action) {
    const currentStatus = button.getAttribute('data-status');
    let newStatus = currentStatus === 'true' ? 'Inactive' : 'Active';
    if (action == "delete") {
        newStatus = currentStatus === 'true' ? 'UnDelete' : 'Delete';
    }
    else if (action == "block") {
        newStatus = currentStatus === 'true' ? 'UnBlock' : 'Block';
    }
    else if (action == "addloanstatus") {
        newStatus = currentStatus === 'true' ? 'Denied Add Loan' : 'Allow Add Loan';
    }
    else if (action == "editloanstatus") {
        newStatus = currentStatus === 'true' ? 'Denied Edit Loan' : 'Allow Edit Loan';
    }
    else if (action == "deleteloanstatus") {
        newStatus = currentStatus === 'true' ? 'Denied Delete Loan' : 'Allow Delete Loan';
    }
    else if (action == "visibleloanstatus") {
        newStatus = currentStatus === 'true' ? 'Denied Visible Sale Loan' : 'Allow Visible Sale Loan';
    }
    else if (action == "sell") {
        newStatus = currentStatus === 'true' ? 'Loan is In Active for Sell' : 'Loan is Active for Sell';
    }

    Swal.fire({
        title: `Change status to ${newStatus}?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: "",
                text: "Please wait ....",
                showConfirmButton: false,
                allowOutsideClick: false,
                willOpen: () => {
                    Swal.showLoading(); // Show the loading spinner while the deletion is in progress
                }
            });
            const payload = {
                recordid: recordId // Boolean value based on the new status
            };
            // Make an AJAX call to update the status
            $.ajax({
                url: `/api/${module}/${action}`, // Include ID in the route
                type: 'POST',
                data: JSON.stringify(payload), // Send payload as JSON
                contentType: 'application/json', // Content type is JSON
                success: (response) => {
                    Swal.close();
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