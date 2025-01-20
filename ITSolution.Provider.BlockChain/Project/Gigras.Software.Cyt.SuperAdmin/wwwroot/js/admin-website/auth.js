$('#frmLogin').on("submit", async function (e) {
    debugger;
    e.preventDefault(); // Prevent the default form submission
    const formData = new FormData(this); // Collect all form data, including files
    if (!this.checkValidity()) {
        Swal.fire("Error", "Please fill out all required fields.", "error");
        return; // Stop further execution
    }
    showLoader();
    const baseUrl = `/sauth`;
    const basedataUrl = `${baseUrl}/{Login}`;
    const actionUrl = `${basedataUrl}`; // Get the form's action URL
    $.ajax({
        url: actionUrl, // Use the form's action attribute
        type: "POST", // Form's method (POST in this case)
        data: formData,
        processData: false, // Prevent jQuery from processing the data
        contentType: false, // Prevent jQuery from setting the Content-Type header
        success: function (response) {
            hideLoader();
            // Handle the success response
            Swal.fire("Success", "Login successfully.", "success")
                .then(() => {
                    const formDataArray = Array.from(formData.entries());
                    console.log(formDataArray);
                    $('#frmLogin')[0].reset(); // Reset the form
                    history.back();
                });
        },
        error: function (xhr, status, error) {
            // Handle the failure response
            hideLoader();
            Swal.fire("Error", xhr.responseText || "An error occurred during submission.", "error");
        }
});