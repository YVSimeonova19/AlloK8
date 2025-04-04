function openDeleteModal(labelId) {
    document.getElementById('labelDeleteModal').setAttribute('data-label-id', labelId);
    $('#labelDeleteModal').modal('show');
}

function openEditModal(labelId, title, description, color) {
    document.querySelector('#labelEditModal input[name="Id"]').value = labelId;
    document.querySelector('#labelEditModal input[name="Title"]').value = title;
    document.querySelector('#labelEditModal textarea[name="Description"]').value = description;
    document.querySelector('#labelEditModal input[name="Color"]').value = color;

    window.updateColorPicker(color);

    document.getElementById("modal-title").innerText = title;

    // Store the current editing label ID in a data attribute on the modal itself
    document.getElementById('labelEditModal').setAttribute('data-editing-label-id', labelId);

    $('#labelEditModal').modal('show');
}