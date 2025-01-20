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
async function GetInstance() {
    try {
        if (typeof window.ethereum !== 'undefined') {
            try {
                // Fetch ABI and contract address concurrently
                const [responseabi, responsecontract] = await Promise.all([
                    $.ajax({ url: '/api/ethereum/abi', method: 'GET' }),
                    $.ajax({ url: '/api/ethereum/contract', method: 'GET' })
                ]);

                let contractAddress = responsecontract.contractAddress;
                let parsedABI = JSON.parse(responseabi.abi);
                let provider = window.ethereum;
                loanContract = new LoanContract(contractAddress, parsedABI, provider);
                let isprocess = await loanContract.IsLendder();
                if (isprocess) {
                    await RequestBorrows();
                }
            } catch (error) {
                console.error('Error fetching ABI or contract:', error);
            }
        } else {
            console.error('MetaMask is not available.');
        }
    } catch (error) {
        console.error('Error connecting to the contract:', error instanceof Error ? error.message : error);
    }
}

async function RequestBorrows() {
    try {
        let requestedBorrowers = await loanContract.fetchRequestedBorrowers();
        console.log("Requested Borrowers:", requestedBorrowers);

        if (requestedBorrowers.length === 0) {
            Swal.fire({
                text: "No requested borrowers found.",
                icon: "info",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
        } else {
            const borrowers = JSON.stringify({ requestedBorrowers })
            const apiUrl = "/api/ethereum/borrower-lists";
            const response = await fetch(apiUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: borrowers // Pass the array in the request body
            }).then(response => {
                // Check if response is ok
                if (!response.ok) {
                    throw new Error(`API Error: ${response.status} - ${response.statusText}`);
                }
                // Parse the response body as JSON
                return response.json();
            }).then(parsedResponse => {
                // Now you can use parsedResponse which is the parsed JavaScript object
                console.log(parsedResponse);  // Access your data
                const tbody = $("#kt_category_table tbody");
                tbody.empty(); // Clear existing rows
                DisplayData(tbody, parsedResponse);
            }).catch(error => {
                // Handle errors
                console.error(error);
            });
        }
    } catch (error) {
        console.error("Error fetching requested borrowers:", error.message);
        Swal.fire({
            text: `Error fetching requested borrowers: ${error.message}`,
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        });
    }
}

function DisplayData(tbody, data) {
    const thead = $("#kt_category_table thead");
    thead.empty(); // Clear existing rows
    thead.append(`
                        <tr class="text-start text-gray-400 fw-bolder fs-7 text-uppercase gs-0">
                                <th class="min-w-125px">#</th>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Loan Amt</th>
                                <th>Duration</th>
                                <th>InterestRate</th>
                                <th class="text-end min-w-50px">Actions</th>
                            </tr>
                     `);
    let i = 1;
    data.forEach(lookup => {
        let obj = JSON.parse(lookup.UserData);
        tbody.append(`
                            <tr>
                                <td>${lookup.Id}</td>
                                <td>${obj.LenderName}</td>
                                <td>${obj.LenderEmail}</td>
                                <td>${obj.LoanAmount}</td>
                                <td>${obj.Duration}</td>
                                <td>${obj.InterestRate}</td>
                                <td>
                                    <!-- Approve Button -->
                                    <!-- Approve Button -->
                                    <button class="btn btn-success me-3"
                                            onclick="ApproveBorrowLoan('${lookup.MetaMaskID}')">
                                        Approve
                                    </button>

                                    <!-- Reject Button -->
                                    <button class="btn btn-danger" 
                                            onclick="RejectBorrowLoan('${lookup.MetaMaskID}')">
                                        Reject
                                    </button>
                                </td>
                            </tr>
                        `);
        i++;
    });
    KTCategoryList.init();
}

async function ApproveBorrowLoan(borrowerId) {
    try {
        // Display confirmation dialog with SweetAlert2
        const willApprove = await Swal.fire({
            title: "Are you sure?",
            text: `Do you want to approve the loan for ${borrowerId}?`,
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Approve",
            cancelButtonText: "Cancel",
            dangerMode: true,
        });

        if (willApprove.isConfirmed) {
            console.log(`Loan approval in progress for ${borrowerId}`);

            // Call the smart contract method to approve the loan
            const status = await loanContract.approveLoan(borrowerId);

            if (status) {
                // Success alert
                await Swal.fire({
                    title: "Loan Approved!",
                    text: `The loan for ${borrowerId} has been successfully approved.`,
                    icon: "success",
                });

                // Reload the page
                window.location.reload();
            } else {
                // Error alert for failure
                await Swal.fire({
                    title: "Error!",
                    text: `Failed to approve the loan for ${borrowerId}.`,
                    icon: "error",
                });
            }
        } else {
            console.log("Loan approval cancelled by user.");
        }
    } catch (error) {
        // Handle any errors
        await Swal.fire({
            title: "Error!",
            text: `Failed to approve loan: ${error.message}`,
            icon: "error",
        });
        console.error("Error approving loan:", error.message);
    }
}



async function RejectBorrowLoan(borrowerId) {
    try {
        const willApprove = await Swal.fire({
            title: "Are you sure?",
            text: `Do you want to reject the loan for ${borrowerId}?`,
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Reject",
            cancelButtonText: "Cancel",
            dangerMode: true,
        });

        if (willApprove.isConfirmed) {
            // Call the smart contract method to approve the loan
            const status = await loanContract.rejectLoanRequest(borrowerId);

            // Reload the page if the loan was successfully approved
            if (status) {
                // Success alert
                swal({
                    title: "Loan rejected!",
                    text: `The loan for ${borrowerAddress} has been successfully rejected.`,
                    icon: "success",
                });
                window.location.reload();
            }
            swal({
                title: "Error!",
                text: `Failed to reject loan: ${error.message}`,
                icon: "error",
            });
        }
        else {
            console.log('Loan reject canceled.');
            return; // Exit if the user cancels
        }

    } catch (error) {
        swal({
            title: "Error!",
            text: `Failed to reject loan: ${error.message}`,
            icon: "error",
        });
        console.error('Error rejecting loan:', error.message);
    }
}