$('input[type="textbox"].form-control').on("keyup", function (e) {
    e.preventDefault();

    let pattern = $(this).data('regex'); // Get the regex pattern from data attribute
    let currentElement = $(this);
    let ctrlName = currentElement.attr("id"); // Get the control ID

    // Find the validation control based on ID
    let validationCtrl = $("[id^='" + ctrlName + "-field-validation-valid-Regex-']");

    if (validationCtrl.length > 0) { // Check if validation control exists
        let inputValue = currentElement.val().trim(); // Get trimmed input value

        if (inputValue !== "") { // If input is not empty
            // Check if value matches the regex pattern
            if (inputValue.match(pattern) !== null) {
                validationCtrl.css('display', 'none'); // Hide validation message if valid
            } else {
                validationCtrl.css('display', 'block'); // Show validation message if invalid
            }
        } else {
            validationCtrl.css('display', 'none'); // Hide validation message if input is empty
        }
    }
});
