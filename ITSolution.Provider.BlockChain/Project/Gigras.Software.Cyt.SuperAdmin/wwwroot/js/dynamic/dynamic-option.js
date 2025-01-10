// Function to toggle SourceTable visibility
function toggleSourceTable() {
    let isDynamicChecked = document.getElementById('IsDynamic').checked;
    let optionsTableContainer = document.getElementById('optionsTableContainer');
    let sourceTableContainer = document.getElementById('sourceTableContainer');
    let source = document.getElementById('SourceTable');
    let condition = document.getElementById('Condition');
    clearOptionTable();

    if (isDynamicChecked) {
        // Show the option table and source table fields
        optionsTableContainer.style.display = 'none';
        sourceTableContainer.style.display = 'block';
        source.setAttribute("required", "true");
        condition.setAttribute("required", "true");
        source.value = "";  // Clear the value of SourceTable
        condition.value = ""; // Clear the value of Condition
    } else {
        // Hide the option table and source table fields, and clear options
        optionsTableContainer.style.display = 'block';
        sourceTableContainer.style.display = 'none';
        source.removeAttribute("required");
        condition.removeAttribute("required");
        source.value = "";  // Clear the value of SourceTable
        condition.value = ""; // Clear the value of Condition
    }
}

// Function to show modal for adding options
function addOption() {
    const tableBody = document.querySelector("#optionTable tbody");

    const newRow = document.createElement("tr");

    newRow.innerHTML = `
                <td>
                    <input type="text" class="form-control" name="OptionLabel[]" placeholder="Option Label" required />
                </td>
                <td>
                    <input type="text" class="form-control" name="OptionValue[]" placeholder="Option Value" required />
                </td>
                <td>
                    <button type="button" class="btn btn-danger" onclick="removeOption(this)">Remove</button>
                </td>
            `;

    tableBody.appendChild(newRow);
}

function addOptionToTable() {
    let optionLabel = document.getElementById("OptionLabel").value;
    let optionValue = document.getElementById("OptionValue").value;

    if (optionLabel && optionValue) {
        let table = document.getElementById("optionTable").getElementsByTagName('tbody')[0];
        let row = table.insertRow(table.rows.length);

        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);

        cell1.innerHTML = optionLabel;
        cell2.innerHTML = optionValue;
        cell3.innerHTML = '<button class="btn btn-danger" onclick="removeOption(this)">Remove</button>';

        // Clear input fields after adding option
        document.getElementById("OptionLabel").value = '';
        document.getElementById("OptionValue").value = '';

        // Close the modal after adding the option
        let myModal = bootstrap.Modal.getInstance(document.getElementById('addOptionModal'));
        myModal.hide();
    }
}

// Function to remove an option from the table
function removeOption(button) {
    const row = button.closest("tr");
    row.remove();
}

function preserveTable() {
    // This function is called to ensure the table does not get cleared when IsDynamic is clicked
    let optionRows = document.getElementById("optionTable").getElementsByTagName('tbody')[0].rows;
    if (optionRows) {
        for (i = 0; i < optionRows.length - 1; i++) {
            removeOption(optionRows[i]);
        }
    }
    let isDynamic = document.getElementById("IsDynamic").checked;

    if (isDynamic) {
        document.getElementById("sourceTableContainer").style.display = "block";
    } else {
        document.getElementById("sourceTableContainer").style.display = "none";
    }
}

function clearOptionTable() {
    let table = document.getElementById("optionTable").getElementsByTagName('tbody')[0];
    table.innerHTML = '';  // Clear all rows
}