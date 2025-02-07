function DisplayLoanBuyInterestData(tbody, data, isadmin) {
    const thead = $("#kt_category_table thead");
    thead.empty(); // Clear existing rows
    thead.append(`
                        <tr class="text-start text-gray-400 fw-bolder fs-7 text-uppercase gs-0">
                                <th class="min-w-125px">#</th>
                                <th>Loan Id</th>
                                <th>Lender Name</th>
                                <th>Lender Email</th>
                                <th>Lender Phone</th>
                                <th>Borrower Name</th>
                                <th>Loam Amount</th>
                                <th>CreateBy</th>
                                <th>CreateAt</th>
                                <th class="text-end min-w-50px">Actions</th>
                            </tr>
                     `);
    data.data.forEach(user => {
        // Append the user data to the table
        tbody.append(`
        <tr>
            <td>${user.id}</td>
            <td>${user.loanId}</td>
            <td>${user.lendderName}</td>
            <td>${user.lendderEmail}</td>
            <td>${user.lendderPhone}</td>
            <td>${user.loanDetails.borrowerName}</td>
            <td>${parseFloat(user.loanDetails.principalAmount).toFixed(2)}</td>
            <td>${user.createdBy}</td>
            <td>${new Date(user.createdAt).toLocaleString()}</td>
            <td>
                ${(!isadmin && !user.isSellerApproved) ? `
                    <button style="background-color: green; color: white; font-size: 10px; border: none; border-radius: 5px;" 
                            data-status='${user.isSellerApproved}' 
                            onclick="toggleUserStatus(this,'${user.id}', 'loandetails','sell-buy-interest-approved')">
                        Approve
                    </button>`
                    : ""}
            
                ${(isadmin && user.isSellerApproved) ? `
                    <button style="background-color: blue; color: white; font-size: 10px; border: none; border-radius: 5px;" 
                            data-status='${user.isSellerApproved}' 
                            onclick="toggleUserStatus(this,'${user.id}', 'loandetails','admin-sell-buy-interest-approved')">
                        Approve
                    </button>
                    <button style="background-color: red; color: white; font-size: 10px; border: none; border-radius: 5px;" 
                            data-status='${user.isSellerApproved}' 
                            onclick="toggleUserStatus(this,'${user.id}', 'loandetails','admin-sell-buy-interest-reject')">
                        Reject
                    </button>`
                    : ""}
            </td>
        </tr>
    `);

    });
}
async function LoanBuyInterestApprove(button) {
    try {
        // Display confirmation dialog using SweetAlert2
        const confirmation = await Swal.fire({
            title: "Are you sure?",
            text: "Are you interested to buy this loan",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes,!",
            cancelButtonText: "Cancel"
        });

        if (confirmation.isConfirmed) {
            let loanId = button.getAttribute("data-loan-id");
            const payload = {
                recordid: loanId // Boolean value based on the new status
            };
            $.ajax({
                url: `/api/loandetails/loan-sell-intereset`, // Include ID in the route
                type: 'POST',
                data: JSON.stringify(payload), // Send payload as JSON
                contentType: 'application/json', // Content type is JSON
                success: (response) => {
                    Swal.fire('Updated!', `Thanks for the interest to buy this loan`, 'success').then(() => {
                        // Reload the page only after user clicks "OK"
                        window.location.reload();
                    });
                },
                error: () => {
                    Swal.fire('Error!', 'Failed to buying the loan. Please try again.', 'error');
                },
            });
        }
    } catch (error) {
        // Handle errors
        Swal.close(); // Close the spinner in case of an error
        Swal.fire("Error", "An error occurred while buying the loan.", "error");
        console.error(error); // Log error for debugging
    }
}

async function LoanBuyInterestReject(button) {
    try {
        // Display confirmation dialog using SweetAlert2
        const confirmation = await Swal.fire({
            title: "Are you sure?",
            text: "Are you rejected to buy this loan, After rejected this loan will not be display for buying",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes!",
            cancelButtonText: "Cancel"
        });

        if (confirmation.isConfirmed) {
            let loanId = button.getAttribute("data-loan-id");
            const payload = {
                recordid: loanId // Boolean value based on the new status
            };
            $.ajax({
                url: `/api/loandetails/loan-sell-reject`, // Include ID in the route
                type: 'POST',
                data: JSON.stringify(payload), // Send payload as JSON
                contentType: 'application/json', // Content type is JSON
                success: (response) => {
                    Swal.fire('Updated!', `This loan will not be display on you dashboard`, 'success').then(() => {
                        // Reload the page only after user clicks "OK"
                        window.location.reload();
                    });
                },
                error: () => {
                    Swal.fire('Error!', 'Failed to reject for buying the loan. Please try again.', 'error');
                },
            });
        }
    } catch (error) {
        // Handle errors
        Swal.close(); // Close the spinner in case of an error
        Swal.fire("Error", "An error occurred while reject to buying the loan.", "error");
        console.error(error); // Log error for debugging
    }
}