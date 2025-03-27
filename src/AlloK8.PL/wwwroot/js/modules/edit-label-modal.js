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

// Attach event handlers to the color picker when document is ready
document.addEventListener("DOMContentLoaded", function () {
    const setupColorPicker = function () {
        // Color picker functionality
        const colorInput = document.getElementById('color');
        const colorPreview = document.querySelector('#labelEditModal .color-preview');
        const colorDisplay = document.getElementById('colorDisplay');
        const colorPicker = document.getElementById('colorPicker');

        if (colorPreview && colorDisplay && colorPicker) {
            // Open color picker when clicking on the preview or text input
            colorPreview.addEventListener('click', function () {
                colorPicker.click();
            });

            colorDisplay.addEventListener('click', function () {
                colorPicker.click();
            });

            // Update all values when color is selected
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

    // Try to set up the color picker
    setupColorPicker();

    // Set up form action when modal opens
    $('#labelEditModal').on('shown.bs.modal', function () {
        const form = document.querySelector("#labelEditModal form");

        // Update the form action with the correct ID
        const labelId = form.querySelector('input[name="Id"]').value;
        form.action = `/labels/${labelId}/edit`;
    });

    // Form submission handler
    document.querySelector("#labelEditModal form").addEventListener("submit", async function (e) {
        e.preventDefault();

        let form = e.target;
        let formData = new FormData(form);
        const labelId = formData.get('Id');

        try {
            // Send the form data as is (with # in color)
            let response = await fetch(form.action, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                let updatedLabel;

                try {
                    updatedLabel = await response.json();

                    // Use the color directly from the response or from the form
                    updatedLabel.color = updatedLabel.color || formData.get('Color');

                    // Find the table row
                    const tr = document.querySelector(`tr:has(button[onclick*="${labelId}"])`);
                    if (tr) {
                        // Update the badge with new color and title
                        const badge = tr.querySelector('.badge');
                        if (badge) {
                            badge.style.backgroundColor = updatedLabel.color;
                            badge.style.color = getTextColor(updatedLabel.color);
                            badge.textContent = updatedLabel.title;
                        }

                        // Update the description
                        const descriptionCell = tr.querySelectorAll('td')[1];
                        if (descriptionCell) {
                            descriptionCell.textContent = updatedLabel.description;
                        }

                        // Update the edit button onclick attributes
                        const editButton = tr.querySelector('button.btn-outline-secondary');
                        if (editButton) {
                            editButton.setAttribute('onclick', `openEditModal(${updatedLabel.id}, '${updatedLabel.title}', '${updatedLabel.description}', '${updatedLabel.color}')`);
                        }

                        // Update delete button onclick attribute 
                        const deleteButton = tr.querySelector('button.btn-outline-danger');
                        if (deleteButton) {
                            deleteButton.setAttribute('onclick', `openDeleteModal(${updatedLabel.id})`);
                        }
                    } else {
                        window.location.reload();
                        return;
                    }
                } catch (jsonErr) {
                    window.location.reload();
                    return;
                }

                // Close the modal
                $("#labelEditModal").modal("hide");
            } else {
                alert('Failed to update label. Please try again.');
            }
        } catch (error) {
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