document.addEventListener("DOMContentLoaded", function () {
    const userSearchInput = document.getElementById("userSearchInput");
    const userResults = document.getElementById("userResults");
    const selectedUsersContainer = document.getElementById("selectedUsers");
    const assignedUsersContainer = document.getElementById("assignedUsers");
    const validationError = document.getElementById("validationError");
    const addUsersButton = document.getElementById("addUsersButton");
    let selectedUsers = []; // New users to be added
    let assignedUsers = []; // Currently assigned users

    // Label variables
    const selectedLabelsContainer = document.getElementById("selectedLabels");
    const availableLabelsContainer = document.getElementById("availableLabels");
    let availableLabels = []; // All labels for the project
    let selectedLabels = []; // Selected labels for the task

    // Helper function to extract email from different user object structures
    function getUserEmail(user) {
        if (user.email) return user.email;
        if (user.applicationUser?.email) return user.applicationUser.email;
        if (user.userName) return user.userName;
        return "Unknown email";
    }

    // Function to get the project ID from the URL
    function getProjectIdFromUrl() {
        // Try to extract project ID from URL, assuming it follows the pattern /projects/{projectId}/...
        const urlParts = window.location.pathname.split('/');
        const projectIdIndex = urlParts.indexOf('projects') + 1;

        if (projectIdIndex > 0 && projectIdIndex < urlParts.length) {
            return urlParts[projectIdIndex];
        }

        // Fallback: Look for a hidden input with project ID
        const projectIdInput = document.querySelector('input[name="ProjectId"]');
        if (projectIdInput) {
            return projectIdInput.value;
        }

        // Second fallback: Try to get from the kanban view model
        return document.querySelector('#projectId')?.value || '';
    }

    // Function to determine text color based on background color
    function getTextColor(backgroundColor) {
        if (!backgroundColor) return '#000000';

        // Add # if it's not already there
        if (!backgroundColor.startsWith('#')) {
            backgroundColor = '#' + backgroundColor;
        }

        const color = backgroundColor.replace('#', '');
        const r = parseInt(color.substr(0, 2), 16);
        const g = parseInt(color.substr(2, 2), 16);
        const b = parseInt(color.substr(4, 2), 16);

        const brightness = (r * 0.299 + g * 0.587 + b * 0.114);

        return brightness > 130 ? '#000000' : '#FFFFFF';
    }

    // Load available labels for the project
    async function loadAvailableLabels() {
        try {
            const projectId = getProjectIdFromUrl();
            if (!projectId) {
                availableLabelsContainer.innerHTML = "<div class='text-center text-muted'>Cannot determine project ID</div>";
                return;
            }

            const response = await fetch(`/projects/${projectId}/labels/all`);
            if (response.ok) {
                availableLabels = await response.json();
                renderAvailableLabels();
            } else {
                availableLabelsContainer.innerHTML = "<div class='text-center text-muted'>Error loading labels</div>";
            }
        } catch (error) {
            availableLabelsContainer.innerHTML = "<div class='text-center text-muted'>Error loading labels</div>";
        }
    }

    // Load labels for the task
    async function loadTaskLabels(taskId) {
        try {
            const response = await fetch(`/kanban/task/${taskId}/labels`);
            if (response.ok) {
                selectedLabels = await response.json();
                renderSelectedLabels();
            }
        } catch (error) {
            // Silently fail - we'll just show no labels
            selectedLabels = [];
            renderSelectedLabels();
        }
    }

    // Render available labels
    function renderAvailableLabels() {
        availableLabelsContainer.innerHTML = "";

        if (availableLabels.length === 0) {
            availableLabelsContainer.innerHTML = "<div class='text-center text-muted'>No labels available</div>";
            return;
        }

        const labelsList = document.createElement("div");
        labelsList.classList.add("available-labels");

        availableLabels.forEach(label => {
            // Skip if the label is already selected
            if (selectedLabels.some(l => l.id === label.id)) {
                return;
            }

            const labelItem = document.createElement("div");
            labelItem.classList.add("available-label");
            labelItem.dataset.id = label.id;

            const labelBadge = document.createElement("span");
            labelBadge.classList.add("label-badge");
            labelBadge.style.backgroundColor = label.color;
            labelBadge.style.color = getTextColor(label.color);
            labelBadge.textContent = label.title;

            labelItem.appendChild(labelBadge);
            labelItem.addEventListener("click", () => {
                selectLabel(label);
            });

            labelsList.appendChild(labelItem);
        });

        availableLabelsContainer.appendChild(labelsList);
    }

    // Render selected labels
    function renderSelectedLabels() {
        selectedLabelsContainer.innerHTML = "";

        if (selectedLabels.length === 0) {
            const emptyMessage = document.createElement("div");
            emptyMessage.classList.add("text-muted", "small");
            emptyMessage.textContent = "No labels selected";
            selectedLabelsContainer.appendChild(emptyMessage);
            return;
        }

        selectedLabels.forEach(label => {
            const labelItem = document.createElement("div");
            labelItem.classList.add("selected-label");
            labelItem.dataset.id = label.id;

            const labelBadge = document.createElement("span");
            labelBadge.classList.add("label-badge");
            labelBadge.style.backgroundColor = label.color;
            labelBadge.style.color = getTextColor(label.color);
            labelBadge.textContent = label.title;

            const removeButton = document.createElement("button");
            removeButton.classList.add("remove-label-btn");
            removeButton.innerHTML = "×";
            removeButton.addEventListener("click", function(e) {
                e.preventDefault();
                removeLabel(label.id);
            });

            const hiddenInput = document.createElement("input");
            hiddenInput.type = "hidden";
            hiddenInput.name = "LabelIds[]";
            hiddenInput.value = label.id;

            labelItem.appendChild(labelBadge);
            labelItem.appendChild(removeButton);
            labelItem.appendChild(hiddenInput);
            selectedLabelsContainer.appendChild(labelItem);
        });

        // Update available labels display
        renderAvailableLabels();
    }

    // Select a label
    function selectLabel(label) {
        // Check if already selected
        if (selectedLabels.some(l => l.id === label.id)) {
            return;
        }

        // Add to selected labels
        selectedLabels.push(label);

        // Update UI
        renderSelectedLabels();
    }

    // Remove a label
    async function removeLabel(labelId) {
        // Find the label object before removing it from selectedLabels
        const removedLabel = selectedLabels.find(label => label.id === labelId);

        // Remove from selected labels
        selectedLabels = selectedLabels.filter(label => label.id !== labelId);

        // Update UI for selected labels
        renderSelectedLabels();

        // If we found the removed label, add it back to availableLabels
        if (removedLabel) {
            // Make sure it's not already in availableLabels
            if (!availableLabels.some(label => label.id === removedLabel.id)) {
                availableLabels.push(removedLabel);
            }

            // Re-render the available labels to show the newly available label
            renderAvailableLabels();
        }

        // If the task is already saved, also remove the association in the backend
        const taskId = document.querySelector('#taskModal input[name="Id"]').value;
        if (taskId) {
            try {
                const response = await fetch(`/kanban/task/${taskId}/labels/${labelId}/remove`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error(`Failed to remove label: ${response.status}`);
                }
            } catch (error) {
                console.error('Error removing label:', error);
            }
        }
    }

    // Load assigned users from the task data
    async function loadAssignedUsers(taskId) {
        try {
            // First get the task data using the GetTask endpoint
            const response = await fetch(`/kanban/task/${taskId}`);
            if (response.ok) {
                const taskData = await response.json();
                // Assuming the task data includes users
                assignedUsers = taskData.users || [];
                renderAssignedUsers();
            } else {
                assignedUsersContainer.innerHTML = "<div class='text-center text-muted'>" + window['__T__']['ErrorLoadingUsersErrorMessageText'] + "</div>";
            }
        } catch (error) {
            assignedUsersContainer.innerHTML = "<div class='text-center text-muted'>" + window['__T__']['ErrorLoadingUsersErrorMessageText'] + "</div>";
        }
    }

    // Function to assign a user to a task
    async function assignUserToTask(taskId, userId) {
        try {
            // Use the dedicated AssignUserToTask endpoint instead of EditTask
            const response = await fetch('/kanban/assign-user', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    taskId: taskId,
                    userId: userId
                })
            });

            if (!response.ok) {
                throw new Error(`Failed to assign user: ${response.status}`);
            }

            return true;
        } catch (error) {
            console.error('Error assigning user:', error);
            throw error;
        }
    }

    // Function to remove a user from a task
    async function removeAssignedUser(userId) {
        const taskId = document.querySelector('#taskModal input[name="Id"]').value;
        if (!taskId) {
            const errorMessage = document.createElement("div");
            errorMessage.classList.add("alert", "alert-danger", "mb-3");
            errorMessage.innerHTML = window['__T__']['CannotRemoveUserUnsavedError'];
            form.prepend(errorMessage);
            
            return;
        }

        try {
            // Fix POST request by properly sending it with fetch
            const response = await fetch(`/kanban/${taskId}/remove-user/${userId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                // Empty body or add anti-forgery token if needed
                body: JSON.stringify({})
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Server response:', errorText);
                throw new Error(`Failed to remove user: ${response.status}`);
            }

            // Remove from our local list
            assignedUsers = assignedUsers.filter(user => user.id !== userId);

            // Update UI
            renderAssignedUsers();

            // Show temporary success message
            const successMessage = document.createElement("div");
            successMessage.classList.add("alert", "alert-success", "mb-3");
            successMessage.innerHTML = window['__T__']['UsersRemovedSuccessfullyMessageText'];
            assignedUsersContainer.insertAdjacentElement('beforebegin', successMessage);

            // Remove the message after 3 seconds
            setTimeout(() => {
                successMsg.remove();
            }, 3000);
        } catch (error) {
            console.error('Error removing user:', error);
            const errorMessage = document.createElement("div");
            errorMessage.classList.add("alert", "alert-danger", "mb-3");
            errorMessage.innerHTML = window['__T__']['FailedToRemoveUserFromTaskErrorMessageText'];
            form.prepend(errorMessage);
        }
    }

    // Render assigned users
    function renderAssignedUsers() {
        assignedUsersContainer.innerHTML = "";

        if (assignedUsers.length === 0) {
            assignedUsersContainer.innerHTML = "<div class='text-center text-muted'>" + window['__T__']['NoUsersAssignedText'] + "</div>";
            return;
        }

        assignedUsers.forEach(user => {
            const userItem = document.createElement("div");
            userItem.classList.add("task-user-item");
            userItem.dataset.userId = user.id;

            const userEmail = document.createElement("span");
            userEmail.classList.add("user-email");
            userEmail.textContent = getUserEmail(user);

            // Create hidden input for form submission
            const hiddenInput = document.createElement("input");
            hiddenInput.type = "hidden";
            hiddenInput.name = "Users[]";
            hiddenInput.value = user.id;

            // Add remove button
            const removeButton = document.createElement("button");
            removeButton.classList.add("remove-task-user-btn");
            removeButton.innerHTML = "×";
            removeButton.title = "Remove user";
            removeButton.addEventListener("click", function (e) {
                e.preventDefault(); // Prevent form submission
                removeAssignedUser(user.id);
            });

            userItem.appendChild(userEmail);
            userItem.appendChild(removeButton);
            userItem.appendChild(hiddenInput);
            assignedUsersContainer.appendChild(userItem);
        });
    }

    // Search for users as the user types
    userSearchInput.addEventListener("input", async function () {
        const searchTerm = userSearchInput.value.trim();
        hideValidationError();

        if (searchTerm.length >= 1) {
            try {
                // Get the project ID from the form or the URL
                const projectId = getProjectIdFromUrl();

                // Use your new endpoint for searching users
                const response = await fetch(`/kanban/users/search?email=${searchTerm}&projectId=${projectId}`);
                if (response.ok) {
                    const users = await response.json();
                    renderUserResults(users);
                } else {
                    userResults.innerHTML = "<li class='list-group-item'>" + window['__T__']['NoUsersFoundMessageText'] + "</li>";
                }
            } catch (error) {
                console.error('Error searching users:', error);
                userResults.innerHTML = "<li class='list-group-item'>" + window['__T__']['ErrorSearchingUsers'] + "</li>";
            }
        } else {
            userResults.innerHTML = "";
        }
    });

    // Render the search results
    function renderUserResults(users) {
        userResults.innerHTML = "";

        // Add scrollbar if more than 5 users
        if (users.length > 5) {
            userResults.classList.add("scrollable");
        } else {
            userResults.classList.remove("scrollable");
        }

        users.forEach(user => {
            const userItem = document.createElement("li");
            userItem.classList.add("list-group-item");
            userItem.textContent = getUserEmail(user);
            userItem.dataset.id = user.id;

            // Check if user is already assigned or selected
            const isAssigned = assignedUsers.some(u => u.id === user.id);
            const isSelected = selectedUsers.some(u => u.id === user.id);

            if (isAssigned || isSelected) {
                userItem.classList.add("disabled");
                userItem.title = "users already assigned";
            } else {
                userItem.addEventListener("click", function () {
                    selectUser(user);
                });
            }

            userResults.appendChild(userItem);
        });
    }

    // Add a user to the selected list
    function selectUser(user) {
        // Check if already selected
        if (selectedUsers.some(u => u.id === user.id)) {
            showValidationError(getUserEmail(user) + " " + window['__T__']['IsAlreadyInYourSelectionText']);
            return;
        }

        // Check if already assigned
        if (assignedUsers.some(u => u.id === user.id)) {
            showValidationError(getUserEmail(user) + " " + window['__T__']['IsAlreadyAssignedText']);
            return;
        }

        // Add to selected users
        selectedUsers.push({
            id: user.id,
            email: getUserEmail(user)
        });

        // Create user bubble
        const userBubble = document.createElement("div");
        userBubble.classList.add("selected-user");
        userBubble.dataset.id = user.id;

        const userName = document.createElement("span");
        userName.classList.add("user-name");
        userName.textContent = getUserEmail(user);

        const removeButton = document.createElement("button");
        removeButton.classList.add("remove-user-btn");
        removeButton.textContent = "×";
        removeButton.addEventListener("click", function () {
            removeSelectedUser(user.id);
        });

        userBubble.appendChild(userName);
        userBubble.appendChild(removeButton);
        selectedUsersContainer.appendChild(userBubble);

        // Clear search
        userResults.innerHTML = "";
        userSearchInput.value = "";
    }

    // Remove a user from the selected list
    function removeSelectedUser(userId) {
        selectedUsers = selectedUsers.filter(user => user.id !== userId);
        const userBubble = selectedUsersContainer.querySelector(`.selected-user[data-id='${userId}']`);
        if (userBubble) {
            selectedUsersContainer.removeChild(userBubble);
        }
    }

    // Handle adding users without submitting the entire form
    addUsersButton.addEventListener("click", async function () {
        if (selectedUsers.length === 0) {
            showValidationError(window['__T__']['PleaseSelectAtLeastOneUserText']);
            return;
        }

        const taskId = document.querySelector('#taskModal input[name="Id"]').value;
        if (!taskId) {
            showValidationError(window['__T__']['ErrorAddingUserToAnUnsavedTask']);
            return;
        }

        try {
            // Show a loading indicator
            const loadingIndicator = document.createElement("div");
            loadingIndicator.className = "spinner-border spinner-border-sm text-primary ml-2";
            loadingIndicator.setAttribute("role", "status");
            addUsersButton.appendChild(loadingIndicator);
            addUsersButton.disabled = true;

            // For each selected user, make a direct call to the assign endpoint
            for (const user of selectedUsers) {
                // Check if user is not already in assigned users
                if (!assignedUsers.some(u => u.id === user.id)) {
                    await fetch('/kanban/assign-user', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            taskId: parseInt(taskId),
                            userId: parseInt(user.id)
                        })
                    });

                    // Add to our local list
                    assignedUsers.push({
                        id: user.id,
                        applicationUser: {
                            email: user.email
                        }
                    });
                }
            }

            // Clear selected users
            selectedUsers = [];
            selectedUsersContainer.innerHTML = "";

            // Update UI
            renderAssignedUsers();

            // Show success message
            const successMessage = document.createElement("div");
            successMessage.classList.add("alert", "alert-success", "mb-3");
            successMessage.innerHTML = window['__T__']['UsersAddedSuccessfullyMessageText'];
            selectedUsersContainer.appendChild(successMessage);

            // Remove success message after 3 seconds
            setTimeout(() => {
                if (selectedUsersContainer.contains(successMessage)) {
                    selectedUsersContainer.removeChild(successMessage);
                }
            }, 3000);
        } catch (error) {
            console.error("Error assigning users:", error);
            showValidationError("Failed to assign users to the task: " + error.message);
        } finally {
            // Remove loading indicator and re-enable button
            addUsersButton.disabled = false;
            const loadingIndicator = addUsersButton.querySelector(".spinner-border");
            if (loadingIndicator) {
                addUsersButton.removeChild(loadingIndicator);
            }
        }
    });

    // Show validation error
    function showValidationError(message) {
        validationError.textContent = message;
        userSearchInput.classList.add("is-invalid");
        validationError.style.display = "block";
    }

    // Hide validation error
    function hideValidationError() {
        userSearchInput.classList.remove("is-invalid");
        validationError.style.display = "none";
    }

    // Initialize when modal is shown
    $('#taskModal').on('shown.bs.modal', function () {
        const taskId = document.querySelector('#taskModal input[name="Id"]').value;

        // Load labels for the project
        loadAvailableLabels();

        // Only load users and task labels if we have a valid task ID
        if (taskId) {
            loadAssignedUsers(taskId);
            loadTaskLabels(taskId);
        } else {
            // For new tasks, there are no assigned users or labels
            assignedUsersContainer.innerHTML = "<div class='text-center text-muted'>" + window['__T__']['NoUsersAssignedText'] + "</div>";
            selectedLabels = [];
            renderSelectedLabels();
        }

        // Reset selected users
        selectedUsers = [];
        selectedUsersContainer.innerHTML = "";
        userSearchInput.value = "";
        userResults.innerHTML = "";
    });

    // Comprehensive date handling function
    function convertToServerDate(inputDate) {
        if (!inputDate) return null;

        try {
            // Log the raw input
            console.log('Raw Input Date:', inputDate);

            // Parse the input date with specific mm/dd/yyyy format
            const [month, day, year] = inputDate.split('/').map(Number);

            console.log('Parsed Components:', { year, month, day });

            // Create a date object at midnight in local time
            const parsedDate = new Date(year, month - 1, day);

            console.log('Parsed Date Object:', parsedDate);
            console.log('Parsed Date Timestamp:', parsedDate.getTime());
            console.log('Is Valid Date:', !isNaN(parsedDate.getTime()));

            if (isNaN(parsedDate.getTime())) {
                console.error('Invalid date format:', inputDate);
                return null;
            }

            // Convert to ISO string at midnight
            const isoDate = parsedDate.toISOString().split('T')[0] + 'T00:00:00Z';

            console.log('Final ISO Date:', isoDate);

            return isoDate;
        } catch (error) {
            console.error('Date conversion error:', error);
            return null;
        }
    }

    // Handle the form submission with fetch
    document.querySelector("#taskModal form").addEventListener("submit", async function (e) {
        e.preventDefault();

        let form = e.target;
        let formData = new FormData(form);

        // Get date inputs
        const startDateInput = document.getElementById("start-date");
        const dueDateInput = document.getElementById("due-date");

        // Create a new FormData to replace the original
        let adjustedFormData = new FormData();

        // Copy all existing form data
        for (let [key, value] of formData.entries()) {
            // Special handling for start and due dates
            if (key === "StartDate" && startDateInput) {
                const startDate = new Date(startDateInput.value).toISOString();
                if (startDate) {
                    console.log('Converted Start Date:', startDate);
                    adjustedFormData.append(key, startDate);
                } else {
                    console.error('Failed to convert start date');
                }
            } else if (key === "DueDate" && dueDateInput) {
                const dueDate = new Date(dueDateInput.value).toISOString()
                if (dueDate) {
                    console.log('Converted Due Date:', dueDate);
                    adjustedFormData.append(key, dueDate);
                } else {
                    console.error('Failed to convert due date');
                }
            } else {
                adjustedFormData.append(key, value);
            }
        }

        try {
            let response = await fetch(form.action, {
                method: 'POST',
                body: adjustedFormData
            });

            if (response.ok) {
                let updatedTask = await response.json();

                // Update the task in the UI
                let taskElement = document.getElementById(`task-${updatedTask.id}`);
                if (taskElement) {
                    taskElement.querySelector("p").innerText = updatedTask.title;

                    if (updatedTask.isPriority) {
                        taskElement.classList.add("priority-task");
                    } else {
                        taskElement.classList.remove("priority-task");
                    }
                }

                // Show success message
                const successMessage = document.createElement("div");
                successMessage.classList.add("alert", "alert-success", "mb-3");
                successMessage.innerHTML = window['__T__']['TaskUpdatedSuccessfullyMessageText'];
                form.prepend(successMessage);

                // Remove message after 3 seconds
                setTimeout(() => {
                    successMessage.remove();
                    // Close the modal
                    $("#taskModal").modal("hide");
                }, 3000);
            } else {
                let errorData = await response.text();
                console.error('Failed to update task:', errorData);
                const errorMessage = document.createElement("div");
                errorMessage.classList.add("alert", "alert-danger", "mb-3");
                errorMessage.innerHTML = window['__T__']['FailedToUpdateTaskText'];
                form.prepend(errorMessage);
            }
        } catch (error) {
            console.error('Error:', error);
            const errorMessage = document.createElement("div");
            errorMessage.classList.add("alert", "alert-danger", "mb-3");
            errorMessage.innerHTML = window['__T__']['ErrorWhileUpdatingTaskMessageText'];
            form.prepend(errorMessage);
        }
    });
});