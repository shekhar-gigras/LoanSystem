function InitEditor(editorCtrlName) {
    let instance = tinymce.init({
        selector: `#${editorCtrlName}`, // Target the textareas
        plugins: 'image link code', // Add desired plugins
        toolbar: 'undo redo | styleselect | bold italic | link image | alignleft aligncenter alignright | code', // Customize the toolbar
        height: 300, // Set the height for all editors
        images_upload_url: '/File/Upload', // Endpoint for image uploads (optional)
        automatic_uploads: true,
        file_picker_callback: function (callback, value, meta) {
            // Custom file picker (optional)
            if (meta.filetype === 'image') {
                let input = document.createElement('input');
                input.setAttribute('type', 'file');
                input.setAttribute('accept', 'image/*');
                input.onchange = function () {
                    let file = this.files[0];
                    let reader = new FileReader();
                    reader.onload = function () {
                        callback(reader.result, { alt: file.name });
                    };
                    reader.readAsDataURL(file);
                };
                input.click();
            }
        }
    });

    return instance;
}