window.updateColorPicker = function (color) {
    setTimeout(function () {
        const colorInput = document.getElementById('color');
        const colorPreview = document.querySelector('#labelEditModal .color-preview');
        const colorDisplay = document.getElementById('colorDisplay');
        const colorPicker = document.getElementById('colorPicker');

        if (colorInput && colorPreview && colorDisplay && colorPicker) {
            // For display, ensure the color has a # prefix
            if (color && !color.startsWith('#')) {
                color = '#' + color;
            }

            colorInput.value = color;
            colorPreview.style.backgroundColor = color;
            colorDisplay.value = color;
            colorPicker.value = color;
        }
    }, 100); // Small delay to ensure elements are available
};

// Attach event handlers when document is ready
document.addEventListener("DOMContentLoaded", function () {
    // Initialize data attributes for all label rows for reliable selection
    function initializeLabelRows() {
        document.querySelectorAll('table.table-hover tbody tr').forEach(row => {
            const editButton = row.querySelector('button.btn-outline-secondary');
            if (editButton) {
                const onclickAttr = editButton.getAttribute('onclick') || '';
                const match = onclickAttr.match(/openEditModal\((\d+)/);
                if (match && match[1]) {
                    row.setAttribute('data-label-id', match[1]);
                }
            }
        });
    }

    // Run this initially
    initializeLabelRows();

    // Set up color picker
    const setupColorPicker = function () {
        const colorInput = document.getElementById('color');
        const colorPreview = document.querySelector('#labelEditModal .color-preview');
        const colorDisplay = document.getElementById('colorDisplay');
        const colorPicker = document.getElementById('colorPicker');

        if (colorPreview && colorDisplay && colorPicker) {
            colorPreview.addEventListener('click', function () {
                colorPicker.click();
            });

            colorDisplay.addEventListener('click', function () {
                colorPicker.click();
            });

            colorPicker.addEventListener('input', function (e) {
                const selectedColor = e.target.value;
                colorInput.value = selectedColor;
                colorPreview.style.backgroundColor = selectedColor;
                colorDisplay.value = selectedColor;
            });
        } else {
            setTimeout(setupColorPicker, 200);
        }
    };

    setupColorPicker();

    // Set up form action when modal opens
    $('#labelEditModal').on('shown.bs.modal', function () {
        const form = document.querySelector("#labelEditModal form");
        const labelId = document.getElementById('labelEditModal').getAttribute('data-editing-label-id');
        form.action = `/labels/${labelId}/edit`;
    });

    // Form submission handler
    document.querySelector("#labelEditModal form").addEventListener("submit", async function (e) {
        e.preventDefault();

        const form = e.target;
        const formData = new FormData(form);
        const labelId = document.getElementById('labelEditModal').getAttribute('data-editing-label-id');

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                let updatedLabel;

                try {
                    updatedLabel = await response.json();

                    // Use the color from the response or from the form
                    updatedLabel.color = updatedLabel.color || formData.get('Color');

                    // Find the table row using the data attribute
                    const tr = document.querySelector(`tr[data-label-id="${labelId}"]`);

                    if (tr) {
                        // Update the badge with new color and title
                        const badge = tr.querySelector('.badge');
                        if (badge) {
                            badge.style.backgroundColor = updatedLabel.color;
                            badge.style.color = getTextColor(updatedLabel.color);
                            badge.textContent = updatedLabel.title;
                        }

                        // Update the description cell
                        const descriptionCell = tr.querySelectorAll('td')[1];
                        if (descriptionCell) {
                            descriptionCell.textContent = updatedLabel.description;
                        }

                        // Properly escape values for attributes
                        const safeTitle = updatedLabel.title.replace(/'/g, "\\'");
                        const safeDescription = updatedLabel.description.replace(/'/g, "\\'");
                        const safeColor = updatedLabel.color.replace(/'/g, "\\'");

                        // Update the edit button onclick
                        const editButton = tr.querySelector('button.btn-outline-secondary');
                        if (editButton) {
                            editButton.setAttribute('onclick',
                                `openEditModal(${updatedLabel.id}, '${safeTitle}', '${safeDescription}', '${safeColor}')`);
                        }

                        // Update delete button onclick
                        const deleteButton = tr.querySelector('button.btn-outline-danger');
                        if (deleteButton) {
                            deleteButton.setAttribute('onclick', `openDeleteModal(${updatedLabel.id})`);
                        }

                        // Update the data-label-id if the ID has changed
                        if (updatedLabel.id !== parseInt(labelId)) {
                            tr.setAttribute('data-label-id', updatedLabel.id);
                        }
                    } else {
                        console.error(`Could not find row with data-label-id=${labelId}`);
                        window.location.reload();
                        return;
                    }
                } catch (jsonErr) {
                    console.error('Error parsing JSON response:', jsonErr);
                    window.location.reload();
                    return;
                }

                // Close the modal
                $("#labelEditModal").modal("hide");
            } else {
                alert('Failed to update label. Please try again.');
            }
        } catch (error) {
            console.error('Error updating label:', error);
            alert('An error occurred while updating the label.');
        }
    });
});

// Function to determine text color based on background color
function getTextColor(backgroundColor) {
    if (!backgroundColor) return '#000000';

    const color = backgroundColor.replace('#', '');
    const r = parseInt(color.substr(0, 2), 16);
    const g = parseInt(color.substr(2, 2), 16);
    const b = parseInt(color.substr(4, 2), 16);

    const brightness = (r * 0.299 + g * 0.587 + b * 0.114);

    return brightness > 130 ? '#000000' : '#FFFFFF';
}