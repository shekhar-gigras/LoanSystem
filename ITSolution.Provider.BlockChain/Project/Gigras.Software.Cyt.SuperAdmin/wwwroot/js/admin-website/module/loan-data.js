function DisplayLoanData(tbody, data) {
    const thead = $("#kt_category_table thead");
    thead.empty(); // Clear existing rows
    thead.append(`
                        <tr class="text-start text-gray-400 fw-bolder fs-7 text-uppercase gs-0">
                                <th class="min-w-125px">#</th>
                                <th>Borrower Name</th>
                                 <th>Lender Name</th>
                                 <th>Principal Amount</th>
                                 <th>Interest Rate</th>
                                <th>CreateBy</th>
                                 <th>ModifiedBy</th>
                              <th class="text-end min-w-50px">Actions</th>
                            </tr>
                     `);
    data.data.forEach(user => {
        // Append the user data to the table
        tbody.append(`
                    <tr>
                        <td>${user.id}</td>
                        <td>${user.borrowerName}</td>
                        <td>${user.lenderName}</td>
                        <td>${user.principalAmount}</td>
                        <td>${user.interestRate}</td>
                       <td>${user.createdBy}</td>
                        <td>${new Date(user.createdAt).toLocaleString()}</td>
                        <td>
                            <button class="btn btn-icon btn-active-light-primary w-30px h-30px me-3 view-json" data-id="${user.id}">
                              <span class="icon">
                                <i class="fa fa-eye"></i>
                             </span>
                            </button>
                            <button
                                class="btn btn-icon btn-active-light-primary w-30px h-30px me-3 edit-btn"
                                data-id="${user.id}">
                                <span class="icon">
                                    <i class="fas fa-pencil-alt"></i>
                                </span>
                            </button>
                        </td>
                    </tr>
                `);
    });
    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');
        const baseUrl = `/sadmin/Borrower`;
        let formgroup = $("#kt_category_table").data("formgroup");
        let formid = $("#kt_category_table").data("formid");
        const basedataUrl = `${baseUrl}/${formgroup}/${formid}`;
        location.href = `${basedataUrl}/edit-form/${id}`;
    });
}

// Function to create an HTML table from JSON
function createLoanDataTableFromJson(jsonData) {
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
                <div class="col-4 mb-4">
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