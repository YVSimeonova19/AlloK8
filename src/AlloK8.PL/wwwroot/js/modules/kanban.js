// Open modal window for editing
async function openTaskModal(element) {
    let taskId = element.id.split("-")[1];

    try {
        let response = await fetch(`/kanban/task/${taskId}`);
        if (response.ok) {
            let task = await response.json();

            // Get modal elements
            const titleInput = document.getElementById("title");
            const descriptionInput = document.getElementById("description");
            const startDateInput = document.getElementById("start-date");
            const dueDateInput = document.getElementById("due-date");
            const modalTitleElement = document.getElementById("modal-title");
            const isPriorityCheckbox = document.getElementById("is-priority");
            const taskIdInput = document.querySelector("input[name='Id']");

            // Safely set values with null checks
            if (titleInput) titleInput.value = task.title || '';
            if (descriptionInput) descriptionInput.value = task.description || '';

            // Handle date inputs with more robust parsing
            if (startDateInput && task.startDate) {
                try {
                    // Convert to local date input format (YYYY-MM-DD)
                    let startDate = new Date(task.startDate);
                    startDateInput.value = task.startDate.slice(0, 10);
                } catch (dateError) {
                    console.warn('Error parsing start date:', dateError);
                    startDateInput.value = '';
                }
            }

            if (dueDateInput && task.dueDate) {
                try {
                    // Convert to local date input format (YYYY-MM-DD)
                    dueDateInput.value = task.dueDate.slice(0, 10);
                } catch (dateError) {
                    console.warn('Error parsing due date:', dateError);
                    dueDateInput.value = '';
                }
            }

            if (modalTitleElement) modalTitleElement.innerText = task.title || 'Edit Task';

            if (isPriorityCheckbox) isPriorityCheckbox.checked = task.isPriority || false;

            if (taskIdInput) taskIdInput.value = task.id || '';

            // Show the modal
            const taskModal = $('#taskModal');
            if (taskModal.length) {
                taskModal.modal("show");
            }
        } else {
            console.error('Failed to fetch task details');
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

// Modify the addTask function to better handle new task creation
async function addTask(columnId) {
    let input = document.getElementById("newTask" + columnId.charAt(0).toUpperCase() + columnId.slice(1));
    let taskText = input.value.trim();
    if (taskText === "") return;

    try {
        let response = await fetch('/kanban/create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                projectId: window['project_id'],
                title: taskText,
                columnId: getColumnIdFromColumnName(columnId)
            })
        });

        if (response.ok) {
            let newTask = await response.json();

            let taskList = document.getElementById(columnId);
            let taskItem = document.createElement('li');
            taskItem.id = `task-${newTask.id}`;
            taskItem.className = 'list-group-item draggable d-flex align-items-center';
            taskItem.draggable = true;
            taskItem.ondragstart = drag;
            taskItem.onclick = function () {
                openTaskModal(this);
            };
            taskItem.innerHTML = `
            <p>${newTask.title}</p>
            <button class="btn btn-icon bg-transparent text-danger ml-auto" onclick="handleRemoveClick(event, this)">
                <i class="remove ti-close"></i>
            </button>`;
            taskList.appendChild(taskItem);
            input.value = '';

            // Show visual feedback
            taskItem.classList.add("task-added");
            setTimeout(() => {
                taskItem.classList.remove("task-added");
            }, 1000);

            // Optional: Pre-populate the modal with new task details
            setTimeout(() => {
                const newTaskElement = document.getElementById(`task-${newTask.id}`);
                if (newTaskElement) {
                    // Prepare modal with minimal details
                    const titleInput = document.getElementById("title");
                    const taskIdInput = document.querySelector("input[name='Id']");
                    const modalTitleElement = document.getElementById("modal-title");

                    if (titleInput) titleInput.value = newTask.title;
                    if (taskIdInput) taskIdInput.value = newTask.id;
                    if (modalTitleElement) modalTitleElement.innerText = newTask.title;

                    // Show the modal
                    $('#taskModal').modal("show");
                }
            }, 100);  // Small delay to ensure DOM is updated
        } else {
            console.error('Failed to add task');
            showNotification("error", "Failed to add task");
        }
    } catch (error) {
        console.error('Error:', error);
        showNotification("error", "Error adding task");
    }
}

// Allow dropping
function allowDrop(ev) {
    ev.preventDefault();
}

// Start dragging
function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}

// Handle drop
async function drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    var taskId = data.split("-")[1];
    var targetColumn = ev.target.closest(".task-list");

    if (!targetColumn) {
        console.error("Could not find the target column.");
        return;
    }

    var targetColumnId = targetColumn.id;
    var currentColumnId = document.getElementById(data).closest(".task-list").id;

    // Get all tasks in the target column
    var tasks = Array.from(targetColumn.querySelectorAll(".draggable"));
    console.log("Tasks in Target Column:", tasks);

    // Calculate the position index
    var position = tasks.length; // Default to the end of the list
    var dropTarget = ev.target.closest(".draggable");

    if (dropTarget) {
        // Find the index of the drop target
        position = tasks.indexOf(dropTarget);

        // Check if the task is dropped below the drop target
        var dropTargetRect = dropTarget.getBoundingClientRect();
        console.log("Drop Target Rect:", dropTargetRect);
        console.log("Mouse Y:", ev.clientY);

        if (ev.clientY > dropTargetRect.top + dropTargetRect.height / 2) {
            position++;
        }
        console.log("Adjusted Position:", position);
    } else {
        // If no drop target is found, append the task to the end
        console.log("Appending to End. Position:", position);
    }

    // Send the move request to the server
    try {
        let response = await fetch('/kanban/move', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                id: taskId,
                columnId: getColumnIdFromColumnName(targetColumnId),
                position: position
            })
        });

        if (response.ok) {
            var taskElement = document.getElementById(data);
            targetColumn.insertBefore(taskElement, tasks[position] || null);
            // Show a subtle success indicator
            taskElement.classList.add("move-success");
            setTimeout(() => {
                taskElement.classList.remove("move-success");
            }, 1000);
        } else {
            console.error('Failed to move task');
            showNotification("error", "Failed to move task");
        }
    } catch (error) {
        console.error('Error:', error);
        showNotification("error", "Error moving task");
    }
}

// Open delete modal
function handleRemoveClick(event, element) {
    event.stopPropagation();
    const taskItem = element.closest('li[id^="task-"]');
    const taskId = taskItem.id.split("-")[1];
    const modal = document.getElementById('taskDeleteModal');
    modal.setAttribute('data-task-id', taskId);

    // Clear any previous error messages
    const errorMessageContainer = document.getElementById("deleteErrorMessage");
    errorMessageContainer.textContent = "";
    errorMessageContainer.classList.add("d-none");

    $('#taskDeleteModal').modal('show');
    return false;
}

// Remove a task
document.addEventListener("DOMContentLoaded", function () {
    // Delete confirmation handler
    document.getElementById("confirmDeleteTask").addEventListener("click", async function () {
        const modal = document.getElementById("taskDeleteModal");
        let taskId = modal.getAttribute("data-task-id");
        let taskItem = document.getElementById("task-" + taskId);
        const errorMessageContainer = document.getElementById("deleteErrorMessage");

        if (!taskId) {
            console.log("Task ID not set.");
            return;
        }

        await fetch(`/kanban/delete`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                id: taskId
            })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Failed to delete task.");
                }
                return Promise.resolve({});
            })
            .then(() => {
                // Hide the modal
                $('#taskDeleteModal').modal('hide');

                // Fade out the task before removing
                taskItem.style.transition = "opacity 0.3s";
                taskItem.style.opacity = "0";

                setTimeout(() => {
                    taskItem.remove();
                }, 300);

                // Show success notification
                const successNotification = document.getElementById("successNotification");
                successNotification.style.display = "block";

                // Auto-hide notification after 3 seconds
                setTimeout(() => {
                    successNotification.style.display = "none";
                }, 3000);
            })
            .catch(error => {
                console.error("Error:", error);
                // Display error message in the modal
                errorMessageContainer.textContent = "@Html.Raw(@T.FailedToDeleteTaskErrorMessage)";
                errorMessageContainer.classList.remove("d-none");
            });
    });
});

// Map column names to IDs
function getColumnIdFromColumnName(columnName) {
    switch (columnName) {
        case 'todo':
            return 1;
        case 'doing':
            return 2;
        case 'done':
            return 3;
        default:
            return 0;
    }
}

// Toggle priority sorting
function togglePrioritySort(checkbox) {
    const sortByPriority = checkbox.checked;
    const projectId = window['project_id'];

    // Get the current URL parameters
    const urlParams = new URLSearchParams(window.location.search);
    // Preserve the onlyMine parameter if it exists
    const onlyMine = urlParams.get('onlyMine');

    let url = `/projects/${projectId}/kanban?sortByPriority=${sortByPriority}`;
    if (onlyMine) {
        url += `&onlyMine=${onlyMine}`;
    }

    // Reload the page with the new parameters
    window.location.href = url;
}

// Toggle only mine filter
function toggleOnlyMine(checkbox) {
    const onlyMine = checkbox.checked;
    const projectId = window['project_id'];

    // Get the current URL parameters
    const urlParams = new URLSearchParams(window.location.search);
    // Preserve the priority sort parameter if it exists
    const sortByPriority = urlParams.get('sortByPriority');

    let url = `/projects/${projectId}/kanban?onlyMine=${onlyMine}`;
    if (sortByPriority) {
        url += `&sortByPriority=${sortByPriority}`;
    }

    // Reload the page with the new filtering parameter
    window.location.href = url;
}

// Initialize the toggle state based on the query parameter
document.addEventListener("DOMContentLoaded", function () {
    const urlParams = new URLSearchParams(window.location.search);
    const sortByPriority = urlParams.get('sortByPriority') === 'true';
    const onlyMine = urlParams.get('onlyMine') === 'true';

    document.getElementById('priorityCheckbox').checked = sortByPriority;
    document.getElementById('onlyMineCheckbox').checked = onlyMine;

    // Enable or disable drag-and-drop based on the toggle state
    if (sortByPriority) {
        disableDragAndDrop();
    } else {
        enableDragAndDrop();
    }

    // Add event listeners for task input fields to support pressing Enter
    const todoInput = document.getElementById("newTaskTodo");
    if (todoInput) {
        todoInput.addEventListener("keypress", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();
                addTask("todo");
            }
        });
    }
});

// Disable drag-and-drop functionality
function disableDragAndDrop() {
    const draggableTasks = document.querySelectorAll('.draggable');
    draggableTasks.forEach(task => {
        task.draggable = false;
        task.removeAttribute('ondragstart');
        task.style.cursor = 'default';
    });

    const taskLists = document.querySelectorAll('.task-list');
    taskLists.forEach(list => {
        list.removeAttribute('ondrop');
        list.removeAttribute('ondragover');
    });
}

// Enable drag-and-drop functionality
function enableDragAndDrop() {
    const draggableTasks = document.querySelectorAll('.draggable');
    draggableTasks.forEach(task => {
        task.draggable = true;
        task.setAttribute('ondragstart', 'drag(event)');
        task.style.cursor = 'grab';
    });

    const taskLists = document.querySelectorAll('.task-list');
    taskLists.forEach(list => {
        list.setAttribute('ondrop', 'drop(event)');
        list.setAttribute('ondragover', 'allowDrop(event)');
    });
}

// Show notification
function showNotification(type, message) {
    const notificationEl = document.createElement("div");
    notificationEl.className = `alert alert-${type === "error" ? "danger" : "success"} notification`;
    notificationEl.textContent = message;
    document.body.appendChild(notificationEl);

    setTimeout(() => {
        notificationEl.style.opacity = "0";
        setTimeout(() => {
            notificationEl.remove();
        }, 300);
    }, 3000);
}