function openFieldSectionModal(id) {
    $('#fieldModal').modal('show');
    $("#SectionId").val(id);
    // Load Sections initially
    loadField(id);
}
// Function to load sections
function loadField(id) {
    // Show loading spinner with Swal
    Swal.fire({
        title: 'Loading...',
        text: 'Please wait while we load the Fields.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // Fetch the sections data from the server
    fetch(`/sadmin/dynamic-form-section-field?id=${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to load sections.");
            }
            return response.json();
        })
        .then(sections => {
            // Clear the existing section list
            let sectionList = document.getElementById('fieldListBody');
            sectionList.innerHTML = '';

            // Populate the table with the fetched sections
            sections.forEach(section => {
                let row = document.createElement('tr');
                row.innerHTML = `
                    <td>${section.id}</td>
                    <td>${section.fieldType.fieldName}</td>
                    <td>${section.fieldType.ctrlType}</td>
                   <td>${section.fieldOrder}</td>
                    <td>${section.cssClass}</td>
                     <td class="text-end">
                        <button class="btn btn-icon w-30px h-30px me-3" onclick="editFormField(${section.id})">
                            <i class="fa fa-pencil-alt"></i>
                        </button>
                        <button
                            class="btn btn-icon btn-active-light-primary w-30px h-30px me-3"
                            onclick="toggleDeleteSectionFormField(${section.id}, ${section.isDelete})">
                            <span class="icon">
                                <i class="${!section.isDelete ? 'fas fa-trash text-success' : 'fas fa-trash text-danger'}"></i>
                            </span>
                        </button>
                        <button class="btn btn-icon" onclick="toggleActiveSectionFormField(${section.id})">
                            <i class="${section.isActive ? 'fa fa-check-circle text-success' : 'fa fa-check-circle text-danger'}"></i>
                        </button>
                    </td>
                `;
                sectionList.appendChild(row);
            });

            // Close the loading indicator
            Swal.close();
        })
        .catch(error => {
            console.error('Error loading fields:', error);

            // Show error message
            Swal.fire("Error", "Unable to load fields. Please try again.", "error");
        });
}

// Add Button Click Event
document.getElementById('addFormFieldButton').addEventListener('click', function () {
    // Clear and show the form
    clearFormField();
    loadFieldTypes();
    showForm('Add New Form Field');
});

// Show the form in the modal
function showForm(formHeading) {
    document.getElementById('formFieldContainer').style.display = 'block'; // Show form
    document.getElementById('formFieldHeading').innerText = formHeading;
    document.getElementById('addFormFieldButton').style.display = 'none'; // Hide Add button
}

// Hide the form and show the Add button when the modal is closed or reset
function hideFormFIeld() {
    document.getElementById('formFieldContainer').style.display = 'none'; // Hide form
    document.getElementById('addFormFieldButton').style.display = 'inline-block'; // Show Add button
}

// Clear Form for Adding Section
function clearFormField() {
    document.getElementById('fieldForm').reset();
    document.getElementById('formfieldId').value = '0';
}

// Edit Button - Open modal with pre-filled data
function editFormField(id) {
    // Show loading spinner with Swal
    Swal.fire({
        title: 'Loading...',
        text: 'Please wait while we load the Form Field details.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // Fetch the section data from the server
    fetch(`/sadmin/dynamic-form-section-field-edit?id=${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to load form field details.");
            }
            return response.json();
        })
        .then(data => {
            // Populate form fields with the retrieved data
            document.getElementById('formfieldId').value = data.id;
            document.getElementById('SectionId').value = data.sectionId;
            document.getElementById('FieldTypeId').value = data.fieldTypeId;
            document.getElementById('FieldOrder').value = data.fieldOrder;
            document.getElementById('CssClass').value = data.cssClass;
            document.getElementById('JavaScript').value = data.javaScript;
            loadFieldTypes(data.fieldTypeId);

            // Close the loading indicator
            Swal.close();

            // Show the form
            showForm('Edit Form Field'); // Assuming showForm is defined elsewhere to handle UI visibility
        })
        .catch(error => {
            console.error('Error fetching form field details:', error);
            Swal.fire("Error", "Unable to load form field for editing", "error");
        });
}




// Save Section (Add/Edit)
document.getElementById('fieldForm').addEventListener('submit', function (event) {
    event.preventDefault();

    // Show the loader
    Swal.fire({
        title: 'Processing...',
        text: 'Please wait while we save the form field.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading(); // Show loading spinner
        }
    });

    // Collect form data
    const formData = new FormData(document.getElementById('fieldForm'));
    let sectionid = document.getElementById("SectionId").value;
    let id = document.getElementById("formfieldId").value;
    let method = (id == 0 ? "POST" : "PUT");
    let action = (id == 0 ? `/sadmin/dynamic-form-section-field-add-save` : `/sadmin/dynamic-form-section-field-edit-save?id=${id}`);

    // Send the form data to the server
    fetch(action, {
        method: method,
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                // If the response is not ok (e.g., status 400 or 409), reject with the response text
                return response.text().then(text => {
                    Swal.fire("Error", text, "error");
                });
            }
            else {
                Swal.fire({
                    title: `${id === 0 ? 'Added!' : 'Updated!'}`,
                    text: `The form field has been ${id === 0 ? 'Added!' : 'Updated!'} successfully.`,
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then(() => {
                    const formId = $("#SectionId").val();
                    loadField(formId); // Reload sections to reflect changes
                    // Hide the form and reset the modal
                    hideFormFIeld(); // Hide the form and show Add button again
                });
            }
        })
        .catch(error => {
            // Handle errors and display an error message
            Swal.fire("Error", "Failed to save form field", "error");
            Swal.close(); // Close the loader on error
        });
});


function toggleDeleteSectionFormField(id, isDelete) {
    const action = isDelete ? 'restore' : 'delete';

    Swal.fire({
        title: `Are you sure you want to ${action} this form field?`,
        text: isDelete
            ? "The section will be restored and visible again."
            : "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: `Yes, ${action} it!`,
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: `${action === 'delete' ? 'Deleting...' : 'Restoring...'}`,
                text: 'Please wait...',
                allowOutsideClick: false,
                didOpen: () => Swal.showLoading()
            });

            // Call API to toggle delete/restore
            fetch(`/sadmin/dynamic-form-section-field-toggle-delete/${id}`, {
                method: 'GET'
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Failed to ${action} the form field.`);
                    }
                    return response.json();
                })
                .then(() => {
                    Swal.fire({
                        title: `${action === 'delete' ? 'Deleted!' : 'Restored!'}`,
                        text: `The section has been ${action}d successfully.`,
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        const formId = $("#SectionId").val();
                        loadField(formId); // Reload sections to reflect changes
                    });
                })
                .catch(error => {
                    Swal.fire("Error", `Failed to ${action} the form field. Please try again.`, "error");
                    console.error(error);
                });
        }
    });
}



function toggleActiveSectionFormField(id) {
    // Show the loader
    Swal.fire({
        title: 'Processing...',
        text: 'Please wait while we update the status.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // Make a request to toggle the active state
    fetch(`/sadmin/dynamic-form-section-field-toggle-active/${id}`, {
        method: 'GET'
    })
        .then(data => {
            Swal.fire({
                title: "Success",
                text: "Record active/inactive successfully!",
                icon: "success",
                confirmButtonText: "OK"
            }).then(() => {
                const formId = $("#SectionId").val();
                loadField(formId); // Reload sections to reflect changes
            });
        })
        .catch(error => {
            console.error('Error toggling active status:', error);
            Swal.fire("Error", "Unable to toggle active status. Please try again.", "error");
        });
}


// Function to load Field Types from the server and populate the select element
function loadFieldTypes(id = 0) {
    // Show loading spinner with Swal
    Swal.fire({
        title: 'Loading...',
        text: 'Please wait while we load the Fields.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    fetch('/sadmin/dynamic-fieldtype/list')  // Replace with your actual endpoint URL
        .then(response => response.json())
        .then(data => {
            Swal.close();
            const fieldTypeSelect = document.getElementById('FieldTypeId');
            // Clear existing options except for the default
            fieldTypeSelect.innerHTML = '<option value="">Select a Field</option>';

            // Populate the select options dynamically
            data.forEach(item => {
                const option = document.createElement('option');
                option.value = item.id;  // Set value to the Id
                option.textContent = item.fieldName + "-" + item.fieldDescription;  // Set text to CtrlType
                if (id == item.id) {
                    option.selected = true;
                }
                fieldTypeSelect.appendChild(option);
            });
        })
        .catch(error => {
            Swal.close();
            console.error('Error loading field types:', error);
        });
}

