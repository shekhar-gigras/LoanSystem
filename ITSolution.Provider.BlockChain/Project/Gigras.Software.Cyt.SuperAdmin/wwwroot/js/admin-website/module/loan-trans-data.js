﻿function DisplayLoanTransData(tbody, data, isadmin) {
    const thead = $("#kt_category_table thead");
    thead.empty(); // Clear existing rows
    thead.append(`
                        <tr class="text-start text-gray-400 fw-bolder fs-7 text-uppercase gs-0">
                                <th class="min-w-125px">#</th>
                                <th>Status</th>
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
                        <td>
                            <div class="symbol symbol-circle symbol-50px overflow-hidden me-3">
                                <div class="symbol-label" style="background-color: ${user.isApproved ? 'green' : (user.isRejected ? 'red' : 'yellow')};">
                                    <span style="color: ${user.isApproved ? 'white' : (user.isRejected ? 'white' : 'blue')}; font-weight: bold;">
                                        ${user.isApproved ? "A" : (user.isRejected ? "R" : "P")}
                                    </span>
                                </div>
                            </div>
                        </td>
                        <td>${user.borrowerName}</td>
                        <td>${user.lenderName}</td>
                        <td>${parseFloat(user.principalAmount).toFixed(2)}</td>
                        <td>${parseFloat(user.interestRate).toFixed(2)}</td>
                       <td>${user.createdBy}</td>
                        <td>${new Date(user.createdAt).toLocaleString()}</td>
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
}

// Function to create an HTML table from JSON
function createLoanTransDataTableFromJson(jsonData) {
    let table = `
            <div class="row">
        `;

    // Create two columns
    Object.entries(jsonData.data).forEach(([key, value], index) => {
        if (FieldArray.includes(key)) {
            return;
        }
        const formattedKey = key.replace(/_/g, " ").replace(/^\w/, (c) => c.toUpperCase());
        let keydesc = findDescriptionByFieldName(jsonData.formfields, key);
        if (keydesc == null) {
            keydesc = formattedKey;
        }

        // Start a new column every 2 fields (making it two columns)
        //if (index % 2 === 0) {
        //table += `<div class="col-md-6 col-sm-12">`;  // Use 6 columns in medium screens (2 columns) and 12 in small screens (1 column)
        //}

        table += `
                <div class="col-4 mb-4">
                    <label><strong>${keydesc}</strong></label>
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