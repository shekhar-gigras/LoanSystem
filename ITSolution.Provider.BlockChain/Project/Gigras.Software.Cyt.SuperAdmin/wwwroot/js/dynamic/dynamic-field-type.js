$(document).ready(function () {
    // Initially check if the checkbox is checked and display options accordingly
    if ($("#hasOptionsCheckbox").prop("checked")) {
        $("#optionsDiv").show();
    } else {
        $("#optionsDiv").hide();
    }

    // Event handler for checkbox change
    $("#hasOptionsCheckbox").change(function () {
        if ($(this).prop("checked")) {
            // Show dropdown and fetch options from the API
            $("#optionsDiv").show();
            fetchOptions();  // Call function to fetch options
        } else {
            // Hide dropdown and reset the selection
            $("#optionsDiv").hide();
            $("#FieldTypeOptionId").empty().append('<option value="">Select an option</option>'); // Reset dropdown options
        }
    });

    // Function to fetch options from the API
    function fetchOptions() {
        $.ajax({
            url: '/sadmin/api/getoptions', // Replace with your API endpoint
            method: 'GET',
            success: function (data) {
                var optionsHtml = '<option value="">Select an option</option>';
                data.forEach(function (item) {
                    optionsHtml += '<option value="' + item.id + '">' + item.optionName + '</option>';
                });
                $("#FieldTypeOptionId").html(optionsHtml);
            },
            error: function (error) {
                console.error("Error fetching options:", error);
            }
        });
    }

    // Initially check if the checkbox is checked and display options accordingly
    if ($("#requiresScriptCheckbox").prop("checked")) {
        $("#validationDiv").show();
    } else {
        $("#validationDiv").hide();
    }

    // Event handler for checkbox change
    $("#requiresScriptCheckbox").change(function () {
        if ($(this).prop("checked")) {
            // Show dropdown and fetch validation options from the API
            $("#validationDiv").show();
            fetchValidationOptions();  // Call function to fetch options
        } else {
            // Hide dropdown and reset the selection
            $("#validationDiv").hide();
            $("#FieldValidations").empty(); // Reset the multi-select dropdown
        }
    });

    // Function to fetch validation options from the API
    function fetchValidationOptions() {
        $.ajax({
            url: '/sadmin/api/getvalidations', // Replace with your actual API endpoint
            method: 'GET',
            success: function (data) {
                var optionsHtml = '';
                data.forEach(function (item) {
                    optionsHtml += '<option value="' + item.id + '">' + item.validationName + '</option>';
                });
                $("#FieldValidations").html(optionsHtml);
            },
            error: function (error) {
                console.error("Error fetching validation options:", error);
            }
        });
    }
});