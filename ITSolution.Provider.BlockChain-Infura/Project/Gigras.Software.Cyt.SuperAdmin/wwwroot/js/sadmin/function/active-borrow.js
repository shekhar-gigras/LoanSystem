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
                    await ActiveBorrows();
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

async function ActiveBorrows() {
    try {
        let activeBorrowers = await loanContract.getActiveBorrowers();
        console.log("Active Borrowers:", activeBorrowers);

        if (activeBorrowers.length === 0) {
            Swal.fire({
                text: "No active borrowers found.",
                icon: "info",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
        } else {
            const borrowers = JSON.stringify({ activeBorrowers })
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
                                            onclick="BorrowBalance('${lookup.MetaMaskID}')">
                                        Balance
                                    </button>

                                    <!-- Reject Button -->
                                    <button class="btn btn-danger" 
                                            onclick="BorrowDueDate('${lookup.MetaMaskID}')">
                                        Loan Info
                                    </button>
                                </td>
                            </tr>
                        `);
        i++;
    });
    KTCategoryList.init();
}

async function BorrowBalance(borrowerID) {
    let balance = await loanContract.getBorrowerBalance(borrowerID);
    if (balance == null) {
        return false;
    }
}

async function BorrowDueDate(borrowerID) {
    try {
        const loanDetails = await loanContract.getLoanDetails(borrowerID);

        if (!loanDetails) {
            await swal({
                title: "No Loan Found",
                text: "The borrower does not have any loan details available.",
                icon: "warning",
                buttons: "OK",
            });
            return false;
        }
    } catch (error) {
        // Handle errors gracefully with a SweetAlert error message
        await swal({
            title: "Error",
            text: `An error occurred while fetching loan details: ${error.message}`,
            icon: "error",
            buttons: "OK",
        });

        console.error('Error fetching loan details:', error.message);
    }
}