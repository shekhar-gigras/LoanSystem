function DisplayUserData(tbody, data) {
    data.forEach(user => {
        // Parse JSON data and dynamically select fields, excluding specific ones
        const jsonSubsetData = {};
        const jsonData = JSON.parse(user.userData || "{}"); // Parse JSON string
        const keys = Object.keys(jsonData)
            .filter(key => key !== "SystemFormId" && key !== "UserId") // Exclude unwanted keys
            .slice(0, 3); // Limit to first 3 keys after filtering

        keys.forEach(key => {
            const formattedKey = key.replace(/_/g, " ").replace(/^\w/, (c) => c.toUpperCase());
            jsonSubsetData[formattedKey] = jsonData[key];
        });

        const jsonSubset = JSON.stringify(jsonSubsetData);

        // Append the user data to the table
        tbody.append(`
                    <tr>
                        <td>${user.id}</td>
                        <td>${user.form?.formName || 'N/A'}</td>
                        <td>${new Date(user.createdAt).toLocaleString()}</td>
                        <td>${user.isActive ? 'Active' : 'Inactive'}</td>
                        <td>${jsonSubset}</td>
                        <td>
                            <button class="btn btn-icon btn-active-light-primary w-30px h-30px me-3 view-json" data-id="${user.id}">
                              <span class="icon">
                                <i class="fa fa-eye"></i>
                             </span>
                            </button>

                        </td>
                    </tr>
                `);
    });
    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');
        location.href = `/sadmin/borrower/edit-form/${id}/` + csc;
    });
}

// Function to create an HTML table from JSON
function createTableFromJson(jsonData) {
    let table = `
            <div class="row">
        `;

    // Create two columns
    Object.entries(jsonData).forEach(([key, value], index) => {
        if (FieldArray.includes(key)) {
            return;
        }
        // Replace underscores with spaces and capitalize the first letter
        const formattedKey = key.replace(/_/g, " ").replace(/^\w/, (c) => c.toUpperCase());

        // Start a new column every 2 fields (making it two columns)
        //if (index % 2 === 0) {
        //table += `<div class="col-md-6 col-sm-12">`;  // Use 6 columns in medium screens (2 columns) and 12 in small screens (1 column)
        //}

        table += `
                <div class="col-3 mb-3">
                    <label><strong>${formattedKey}</strong></label>
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
