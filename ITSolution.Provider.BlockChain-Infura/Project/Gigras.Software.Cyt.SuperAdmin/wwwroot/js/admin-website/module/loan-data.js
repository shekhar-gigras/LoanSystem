function DisplayLoanData(tbody, data, isadmin) {
    const thead = $("#kt_category_table thead");
    thead.empty(); // Clear existing rows
    thead.append(`
                        <tr class="text-start text-gray-400 fw-bolder fs-7 text-uppercase gs-0">
                                <th class="min-w-125px">#</th>
                                <th>Status</th>
                                <th>Borrower Name</th>
                                 <th>Lender Name</th>
                                 <th>Principal Amount</th>
                                 <th>Interest Rate</th>
                                <th>CreateBy</th>
                                 <th>ModifiedBy</th>
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
                                <div class="symbol-label" style="background-color: ${user.isApproved ? 'green' : (user.isRejected ? 'red' : 'yellow')};">
                                    <span style="color: ${user.isApproved ? 'white' : (user.isRejected ? 'white' : 'blue')}; font-weight: bold;">
                                        ${user.isApproved ? "A" : (user.isRejected ? "R" : "P")}
                                    </span>
                                </div>
                            </div>
                        </td>
                        <td>${user.borrowerName}</td>
                        <td>${user.lenderName}</td>
                        <td>${parseFloat(user.principalAmount).toFixed(2)}</td>
                        <td>${parseFloat(user.interestRate).toFixed(2)}</td>
                       <td>${user.createdBy}</td>
                        <td>${new Date(user.createdAt).toLocaleString()}</td>
                        <td>
                            <button class="btn btn-icon btn-active-light-primary w-30px h-30px me-3 view-json" data-id="${user.id}">
                                  <span class="icon">
                                    <i class="fa fa-eye"></i>
                                 </span>
                        </button>
                           ${((!isadmin && !user.isApproved) || (isadmin && !user.isApproved && user.createdBy == "admin"))
                ? `<button
                                                    class="btn btn-icon btn-active-light-primary w-30px h-30px me-3 edit-btn"
                                                    data-id="${user.id}">
                                                    <span class="icon">
                                                        <i class="fas fa-pencil-alt"></i>
                                                    </span>
                                                </button>`
                : ""
            }
                             ${((!isadmin && !user.isApproved) || (isadmin && !user.isApproved && user.createdBy == "admin"))
                ? `<button class="btn btn-icon btn-active-light-primary w-30px h-30px" onclick="deleteRecord('${user.loanId}','loandetails')">
                                            <span class="icon">
                                                ${user.isDelete
                    ? "<i class='btn-danger fas fa-trash-alt'></i>"
                    : "<i class='btn-success fas fa-trash-alt'></i>"
                }
                                            </span>
                                        </button>`
                : ""
            }
                                 ${isadmin && !user.isApproved && !user.isRejected
                ? `<button class="btn btn-icon btn-active-light-primary w-30px h-30px" onclick="approveLoan('${user.loanId}','loandetails')">
                                                        <span class="icon">
                                                            <i class="fas fa-check"></i>
                                                        </span>
                                                   </button>`
                : ""
            }
                                 ${isadmin && !user.isApproved && !user.isRejected
                ? `<button class="btn btn-icon btn-active-light-primary w-30px h-30px" onclick="rejectLoan('${user.loanId}','loandetails')">
                                                <span class="icon">
                                                    <i class="fas fa-times"></i>
                                                </span>
                                            </button>`
                : ""
            }
                        </td>
                    </tr>
                `);
    });
    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');
        const baseUrl = `/sadmin/Borrower`;
        let formgroup = $("#kt_category_table").data("formgroup");
        let formid = $("#kt_category_table").data("formid");
        const basedataUrl = `${baseUrl}/${formgroup}/${formid}`;
        location.href = `${basedataUrl}/edit-form/${id}`;
    });
}

// Function to create an HTML table from JSON
function createLoanDataTableFromJson(jsonData) {
    let table = `
            <div class="row">
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
        //table += `<div class="col-md-6 col-sm-12">`;  // Use 6 columns in medium screens (2 columns) and 12 in small screens (1 column)
        //}

        table += `
                <div class="col-4 mb-4">
                    <label><strong>${keydesc}</strong></label>
                    <p>${formatValue(value)}</p>
                </div>
            `;

        // Close column after every 2 fields
        //if (index % 2 === 1) {
        // table += `</div>`;  // Close the column div
        //}
    });

    // Close last column if necessary
    //if (Object.entries(jsonData).length % 2 !== 0) {
    // table += `</div>`; // Close the last open column div
    //}

    table += `</div>`; // Close the row div
    return table;
}

async function approveLoan(id, module) {
    try {
        // Display confirmation dialog using SweetAlert2
        const confirmation = await Swal.fire({
            title: "Are you sure?",
            text: "Do you want to approve the loan? This action cannot be undone.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, approve it!",
            cancelButtonText: "Cancel"
        });

        if (confirmation.isConfirmed) {
            // Call the MetaMask process
            await MetaMaskProcess(id, module);
        }
    } catch (error) {
        // Handle errors
        Swal.close(); // Close the spinner in case of an error
        Swal.fire("Error", "An error occurred while approving the loan.", "error");
        console.error(error); // Log error for debugging
    }
}

async function rejectLoan(id, module) {
    // Display confirmation dialog using SweetAlert2
    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to reject the loan, This action cannot be undone.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, reject it!",
        cancelButtonText: "Cancel"
    }).then(result => {
        if (result.isConfirmed) {
            // Show the loading spinner
            Swal.fire({
                title: "Approving Loan...",
                text: "Please wait while we rejecting loan.",
                showConfirmButton: false,
                allowOutsideClick: false,
                willOpen: () => {
                    Swal.fire({
                        title: "Loading...",
                        text: "Please wait while we rejecting loan.",
                        allowOutsideClick: false,
                        didOpen: () => {
                            Swal.showLoading();
                        }
                    });
                }
            });
            // Send delete request to the server
            fetch(`/api/${module}/${id}`, { method: "PUT" })
                .then(response => {
                    // Close the loading spinner
                    Swal.close();

                    if (response.ok) {
                        // Show success message after deletion
                        Swal.fire({
                            title: "Rejected!",
                            text: (response.message ?? "Loan has been rejected."),
                            icon: "success",
                            confirmButtonText: "OK"
                        }).then(() => {
                            // Reload the page only after user clicks "OK"
                            window.location.reload();
                        });
                    } else {
                        // Show error message if deletion failed
                        Swal.fire("Error", "Unable to reject it .", "error");
                    }
                })
                .catch(err => {
                    // Close the loading spinner in case of an error
                    Swal.close();

                    // Show error message if there was a problem with the request
                    Swal.fire("Error", "Unable to reject it .", "error");
                });
        }
    });
}

async function MetaMaskProcess(id, module) {
    const confirmation = await Swal.fire({
        title: "Are you sure?",
        text: "Do you want to save the transaction in smart contract and internal database? This action cannot be undone.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, do it!",
        cancelButtonText: "Cancel"
    });

    if (confirmation.isConfirmed) {
        try {
            await checkMetaMaskConnection();
            if (isContractSetupDone) {
                let isprocess = await loanContract.IsLendder();
                if (isprocess) {
                    await loanContract.showLoader();
                    await loanContract.getAddress();
                    let data = await getLoanApiDetails(id);
                    let status = await loanContract.AddUpdateLoanDetail(id, data);
                    let metamaskid = await loanContract.getAddress();
                    let dataconfirmation;
                    if (!status) {
                        await loanContract.hideLoader();
                        await Swal.fire("Error", "Error to submit the data in smart contract", "error")
                        dataconfirmation = await Swal.fire({
                            title: "Confirmation?",
                            text: "Do you want to save the transaction in our internal database? This action cannot be undone.",
                            icon: "warning",
                            showCancelButton: true,
                            confirmButtonText: "Yes, do it!",
                            cancelButtonText: "Cancel"
                        });
                    }
                    if ((dataconfirmation != undefined && dataconfirmation.isConfirmed && !status) || status) {
                        await loanContract.hideLoader();
                        await loanContract.showLoader();
                        const response = await SaveData(id, module, metamaskid);
                        await loanContract.hideLoader();
                        if (response.ok) {
                            // Show success message
                            await Swal.fire({
                                title: "Approved!",
                                text: response.message ?? "Loan has been approved.",
                                icon: "success",
                                confirmButtonText: "OK"
                            });

                            // Delay the reload slightly to ensure spinner disappears
                            setTimeout(() => {
                                window.location.reload();
                            }, 300); // Adjust delay time as needed
                        } else {
                            // Show error message
                            Swal.fire("Error", response.message ?? "Unable to approve the loan.", "error");
                        }
                    }
                }
                else {
                    await Swal.fire({
                        title: "Error!",
                        text: response.message ?? "Please login as a lender",
                        icon: "error",
                        confirmButtonText: "OK"
                    });
                }
            }
            else {
                await Swal.fire("Error", "Please establish or check metamask connection", "error")
            }
        } catch (err) {
            Swal.fire("Error", "An error occurred while processing the loan.", "error");
        }
    }
    else {
        let dataconfirmation;
        dataconfirmation = await Swal.fire({
            title: "Confirmation?",
            text: "Do you want to save the transaction in our internal database? This action cannot be undone.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, do it!",
            cancelButtonText: "Cancel"
        });
        if (dataconfirmation != undefined && dataconfirmation.isConfirmed) {
            await showLoader();
            const response = await SaveData(id, module);
            await hideLoader();
            if (response.ok) {
                // Show success message
                await Swal.fire({
                    title: "Approved!",
                    text: response.message ?? "Loan has been approved.",
                    icon: "success",
                    confirmButtonText: "OK"
                });

                // Delay the reload slightly to ensure spinner disappears
                setTimeout(() => {
                    window.location.reload();
                }, 300); // Adjust delay time as needed
            } else {
                // Show error message
                Swal.fire("Error", response.message ?? "Unable to approve the loan.", "error");
            }
        }
    }
}

async function SaveData(id, module, metamaskid = "0") {
    // Send POST request to the server
    const response = await fetch(`/api/${module}/${id}/${metamaskid}`, { method: "POST" });
    return response; // Return the fetch response for further processing
}