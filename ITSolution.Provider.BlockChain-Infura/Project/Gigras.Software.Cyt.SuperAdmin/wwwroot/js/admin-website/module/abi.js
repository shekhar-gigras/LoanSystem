function DisplayABIData(tbody, data) {
    const thead = $("#kt_category_table thead");
    thead.empty(); // Clear existing rows
    thead.append(`
                        <tr class="text-start text-gray-400 fw-bolder fs-7 text-uppercase gs-0">
                                <th class="min-w-125px">#</th>
                                <th>Version</th>
                                 <th>CreateBy</th>
                                 <th>ModifiedBy</th>
                              <th class="text-end min-w-50px">Actions</th>
                            </tr>
                     `);
    data.data.forEach(lookup => {
        tbody.append(`
                            <tr>
                                <td>${lookup.id}</td>
                                <td>${lookup.version}</td>
                                <td>${lookup.createdBy}</td>
                                <td>${lookup.updatedBy}</td>
                                <td>
                                    <button class="btn btn-icon btn-active-light-primary w-30px h-30px me-3 view-json" data-id="${lookup.id}">
                                      <span class="icon">
                                        <i class="fa fa-eye"></i>
                                     </span>
                                    </button>
                                    <button class="btn btn-icon btn-active-light-primary w-30px h-30px" onclick="deleteRecord(${lookup.id},'abi')">
                                        <span class="icon">
                                           ${lookup.isDelete
                ? "<i class='btn-danger fas fa-trash-alt'></i>"
                : "<i class='btn-success fas fa-trash-alt'></i>"}
                                        </span>
                                    </button>
                                    <button class="btn btn-icon btn-active-light-primary w-30px h-30px me-3"
                                            data-status="${lookup.isActive}"
                                            onclick="toggleStatus(this, ${lookup.id},'abi')">
                                        <span class="icon">
                                            <i class="${lookup.isActive ? "fas fa-check text-success" : "fas fa-times text-danger"}"></i>
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
function createAbiTableFromJson(jsonData) {
    let table = `
            <div class="row">
        `;

    // Create two columns
    Object.entries(jsonData).forEach(([key, value], index) => {
        if (FieldArray.includes(key)) {
            return;
        }
        //if (key === "SystemFormId" || key === "UserId" || key === "Id" || key === "Entity") return; // Skip displaying SystemFormId
        // Replace underscores with spaces and capitalize the first letter
        const formattedKey = key.replace(/_/g, " ").replace(/^\w/, (c) => c.toUpperCase());

        // Start a new column every 2 fields (making it two columns)
        //if (index % 2 === 0) {
        //table += `<div class="col-md-6 col-sm-12">`;  // Use 6 columns in medium screens (2 columns) and 12 in small screens (1 column)
        //}

        table += `
                <div class="col-12 mb-12">
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