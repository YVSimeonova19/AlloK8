document.addEventListener("DOMContentLoaded", function () {
    // Delete confirmation handler
    document.getElementById("confirmDeleteLabel").addEventListener("click", function () {
        const modal = document.getElementById("labelDeleteModal");
        const labelId = modal.getAttribute("data-label-id");
        const errorMessageContainer = document.getElementById("deleteLabelErrorMessage");

        if (!labelId) {
            console.log("Label ID not set.");
            return;
        }

        fetch(`/labels/${labelId}/delete`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json"
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Failed to delete label.");
                }
                return Promise.resolve({});
            })
            .then(() => {
                // Hide the modal
                $('#labelDeleteModal').modal('hide');

                // Remove the label row from the view
                const tr = document.querySelector(`tr:has(button[onclick*="openDeleteModal(${labelId})"])`);
                if (tr) {
                    tr.remove();

                    // Update the label count
                    const countElement = document.querySelector('.card-header .d-flex span');
                    if (countElement) {
                        const currentCount = parseInt(countElement.textContent);
                        if (!isNaN(currentCount)) {
                            const newCount = currentCount - 1;
                            countElement.innerHTML = newCount + " " + window['__T__']['LabelsWordText'];
                        }
                    }
                } else {
                    // If we can't find the element, refresh the page
                    window.location.reload();
                }

                // Show success notification
                const successNotification = document.getElementById("labelSuccessNotification");
                successNotification.style.display = "block";

                // Auto-hide notification after 3 seconds
                setTimeout(() => {
                    successNotification.style.display = "none";
                }, 3000);
            })
            .catch(error => {
                console.error("Error:", error);
                // Display error message in the modal
                errorMessageContainer.textContent = window['__T__']['FailedToDeleteLabelErrorMessage'];
                errorMessageContainer.classList.remove("d-none");
            });
    });
});

// Clear any previous error messages
const errorMessageContainer = document.getElementById("deleteLabelErrorMessage");
errorMessageContainer.textContent = "";
errorMessageContainer.classList.add("d-none");