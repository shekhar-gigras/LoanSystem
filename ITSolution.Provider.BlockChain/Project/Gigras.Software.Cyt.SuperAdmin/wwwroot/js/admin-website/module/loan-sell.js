async function loanBuyerInterest(button) {
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
            Swal.fire({
                title: "",
                text: "Please wait ....",
                showConfirmButton: false,
                allowOutsideClick: false,
                willOpen: () => {
                    Swal.showLoading(); // Show the loading spinner while the deletion is in progress
                }
            });
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
                    Swal.close();
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

async function loanRejectInterest(button) {
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
            Swal.fire({
                title: "",
                text: "Please wait ....",
                showConfirmButton: false,
                allowOutsideClick: false,
                willOpen: () => {
                    Swal.showLoading(); // Show the loading spinner while the deletion is in progress
                }
            });
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
                    Swal.close();
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