document.addEventListener("DOMContentLoaded", async function () {
    await checkMetaMaskConnection();
});

async function DashBoardSubmitData(actionUrl, formData) {
    $.ajax({
        url: actionUrl, // Use the form's action attribute
        type: "POST", // Form's method (POST in this case)
        data: formData,
        processData: false, // Prevent jQuery from processing the data
        contentType: false, // Prevent jQuery from setting the Content-Type header
        success: function (response) {
        },
        error: function (xhr, status, error) {
            // Handle the failure response
        }
    });
}

async function callAjaxWithSwal({
    url,
    method = 'POST',
    data = {},
    confirmationText = 'Are you sure?',
    successMessage = 'Operation successful!',
    errorMessage = 'Something went wrong!'
}) {
    // Show confirmation alert
    const result = await Swal.fire({
        title: 'Confirmation',
        text: confirmationText,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, proceed!',
        cancelButtonText: 'Cancel'
    });

    if (result.isConfirmed) {
        try {
            // Make AJAX request and return a Promise
            const response = await $.ajax({
                url: url,
                method: method,
                data: data,
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            // Show success message
            //Swal.fire('Success', successMessage, 'success');
            return response; // Return the response
        } catch (error) {
            // Show error message
            Swal.fire('Error', errorMessage, 'error');
            console.error(error); // Log the error
            throw error; // Rethrow the error for further handling if needed
        }
    } else {
        return { status: false }
            ; // Return null if the user cancels
    }
}