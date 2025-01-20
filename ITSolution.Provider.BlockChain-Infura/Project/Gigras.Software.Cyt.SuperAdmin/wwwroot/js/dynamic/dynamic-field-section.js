function openSectionModal(id) {
    $('#sectionModal').modal('show');
    $("#FormId").val(id);
    // Load Sections initially
    loadSections(id);
}
// Function to load sections
function loadSections(id) {
    // Show loading spinner with Swal
    Swal.fire({
        title: 'Loading...',
        text: 'Please wait while we load the sections.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // Fetch the sections data from the server
    fetch(`/sadmin/dynamic-form-section?id=${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to load sections.");
            }
            return response.json();
        })
        .then(sections => {
            // Clear the existing section list
            let sectionList = document.getElementById('sectionListBody');
            sectionList.innerHTML = '';

            // Populate the table with the fetched sections
            sections.forEach(section => {
                let row = document.createElement('tr');
                row.innerHTML = `
                    <td>${section.id}</td>
                    <td>${section.sectionName}</td>
                    <td>${section.sectionDescription}</td>
                    <td>${section.sortOrder}</td>
                    <td class="text-end">
                        <button class="btn btn-icon w-30px h-30px me-3" onclick="openFieldSectionModal(${section.id})">
                            <i class="fa fa-plus"></i> <!-- Form icon -->
                        </button>
                        <button class="btn btn-icon w-30px h-30px me-3" onclick="editSection(${section.id})">
                            <i class="fa fa-pencil-alt"></i>
                        </button>
                        <button
                            class="btn btn-icon btn-active-light-primary w-30px h-30px me-3"
                            onclick="toggleDeleteSection(${section.id}, ${section.isDelete})">
                            <span class="icon">
                                <i class="${!section.isDelete ? 'fas fa-trash text-success' : 'fas fa-trash text-danger'}"></i>
                            </span>
                        </button>
                        <button class="btn btn-icon" onclick="toggleActiveSection(${section.id})">
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
            console.error('Error loading sections:', error);

            // Show error message
            Swal.fire("Error", "Unable to load sections. Please try again.", "error");
        });
}

// Add Button Click Event
document.getElementById('addButton').addEventListener('click', function () {
    // Clear and show the form
    clearFormSection();
    showFormSection('Add New Section');
});

// Edit Button - Open modal with pre-filled data
function editSection(id) {
    // Show loading spinner with Swal
    Swal.fire({
        title: 'Loading...',
        text: 'Please wait while we load the section details.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // Fetch the section data from the server
    fetch(`/sadmin/dynamic-form-section-edit?id=${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to load section details.");
            }
            return response.json();
        })
        .then(data => {
            // Populate form fields with the retrieved data
            document.getElementById('Id').value = data.id;
            document.getElementById('SectionName').value = data.sectionName;
            document.getElementById('SectionDescription').value = data.sectionDescription;
            document.getElementById('SortOrder').value = data.sortOrder;

            // Close the loading indicator
            Swal.close();

            // Show the form
            showFormSection('Edit Section'); // Assuming showForm is defined elsewhere to handle UI visibility
        })
        .catch(error => {
            console.error('Error fetching section details:', error);
            Swal.fire("Error", "Unable to load section for editing", "error");
        });
}

// Show the form in the modal
function showFormSection(formHeading) {
    document.getElementById('formContainer').style.display = 'block'; // Show form
    document.getElementById('formHeading').innerText = formHeading;
    document.getElementById('addButton').style.display = 'none'; // Hide Add button
}

// Hide the form and show the Add button when the modal is closed or reset
function hideFormSection() {
    document.getElementById('formContainer').style.display = 'none'; // Hide form
    document.getElementById('addButton').style.display = 'inline-block'; // Show Add button
}

// Clear Form for Adding Section
function clearFormSection() {
    document.getElementById('SectionForm').reset();
    document.getElementById('Id').value = '0';
}

// Save Section (Add/Edit)
document.getElementById('SectionForm').addEventListener('submit', function (event) {
    event.preventDefault();

    // Show the loader
    Swal.fire({
        title: 'Processing...',
        text: 'Please wait while we save the section.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading(); // Show loading spinner
        }
    });

    // Collect form data
    const formData = new FormData(document.getElementById('SectionForm'));
    let id = document.getElementById("Id").value;
    let method = (id == 0 ? "POST" : "PUT");
    let action = (id == 0 ? `/sadmin/dynamic-form-section-add-save` : `/sadmin/dynamic-form-section-edit-save?id=${id}`);

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
                    text: `The section has been ${id === 0 ? 'Added!' : 'Updated!'} successfully.`,
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then(() => {
                    const formId = $("#FormId").val();
                    loadSections(formId); // Reload sections to reflect changes
                    // Hide the form and reset the modal
                    hideFormSection(); // Hide the form and show Add button again
                });
            }
        })
        .catch(error => {
            // Handle errors and display an error message
            Swal.fire("Error", "Failed to save section", "error");
            Swal.close(); // Close the loader on error
        });
});

function toggleDeleteSection(id, isDelete) {
    const action = isDelete ? 'restore' : 'delete';

    Swal.fire({
        title: `Are you sure you want to ${action} this section?`,
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
            fetch(`/sadmin/dynamic-form-section-toggle-delete/${id}`, {
                method: 'GET'
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Failed to ${action} the section.`);
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
                        const formId = $("#FormId").val();
                        loadSections(formId); // Reload sections to reflect changes
                    });
                })
                .catch(error => {
                    Swal.fire("Error", `Failed to ${action} the section. Please try again.`, "error");
                    console.error(error);
                });
        }
    });
}

function toggleActiveSection(id) {
    // Ask for confirmation
    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to toggle the active status of this record?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, toggle it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
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
            fetch(`/sadmin/dynamic-form-section-toggle-active/${id}`, {
                method: 'GET'
            })
                .then(data => {
                    Swal.fire({
                        title: "Success",
                        text: "Record active/inactive successfully!",
                        icon: "success",
                        confirmButtonText: "OK"
                    }).then(() => {
                        const formId = $("#FormId").val();
                        loadSections(formId); // Reload sections to reflect changes
                    });
                })
                .catch(error => {
                    console.error('Error toggling active status:', error);
                    Swal.fire("Error", "Unable to toggle active status. Please try again.", "error");
                });
        }
    });
}
