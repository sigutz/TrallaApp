// Toggles the visibility of a div
function toggle(id) {
    const form = document.getElementById(`to-toggle-${id}`);
    if (form) {
        form.classList.toggle('d-none');
    }
}

// Toggles the visibility of children comments (Collapse/Expand)
function toggleChildren(commentId) {
    const childrenDiv = document.getElementById(`children-${commentId}`);
    const icon = document.getElementById(`icon-${commentId}`);

    if (childrenDiv && icon) {
        if (childrenDiv.classList.contains('d-none')) {
            // Expand
            childrenDiv.classList.remove('d-none');
            childrenDiv.classList.add('d-block');
            icon.classList.replace('bi-folder-plus', 'bi-folder-minus');
        } else {
            // Collapse
            childrenDiv.classList.remove('d-block');
            childrenDiv.classList.add('d-none');
            icon.classList.replace('bi-folder-minus', 'bi-folder-plus');
        }
    }
}

function voteComment(commentId, isUpVote) {
    let formData = new FormData();
    formData.append('commentId', commentId);
    formData.append('isUpVote', isUpVote);

    fetch('/Comments/Vote/', {
        method: 'POST',
        body: formData,
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            // 1. UPDATE SCORE NUMBER & COLOR
            const scoreElement = document.getElementById('score-' + commentId);
            if (scoreElement) {
                scoreElement.innerText = data.score;

                // Reset colors
                scoreElement.classList.remove('text-primary', 'text-danger', 'text-muted');

                // Apply color based on number value
                if (data.score > 0) {
                    scoreElement.classList.add('text-primary'); // Blue
                } else if (data.score < 0) {
                    scoreElement.classList.add('text-danger');  // Red
                } else {
                    scoreElement.classList.add('text-muted');
                }
            }

            // 2. UPDATE ICONS (Outline vs Fill)
            const iconUp = document.getElementById('icon-up-' + commentId);
            const iconDown = document.getElementById('icon-down-' + commentId);

            // Reset both to default (outline) + secondary color
            iconUp.className = "bi bi-arrow-up-circle text-secondary";
            iconDown.className = "bi bi-arrow-down-circle text-secondary";

            // Apply Filled state based on userStatus
            if (data.userStatus === 1) { // User Upvoted
                iconUp.className = "bi bi-arrow-up-circle-fill text-primary";
            } else if (data.userStatus === -1) { // User Downvoted
                iconDown.className = "bi bi-arrow-down-circle-fill text-danger";
            }
        }
    }).catch(error => console.error('Error:', error));
}

function starProject(projectid) {
    let formData = new FormData();
    formData.append('projectid', projectid);
    console.log('am intraat in functie');

    fetch(
        '/Projects/Add2Favorites/',
        {
            method: 'POST',
            body: formData
        }
    ).then(response => {
        if (response.ok)
            return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            const starCount = document.getElementById('star-count-' + projectid);
            const starIcon = document.getElementById('star-icon-' + projectid);

            if (starCount)
                starCount.innerText = data.stars;


            if (data.follow) {
                starIcon.classList.replace('bi-star', 'bi-star-fill');
            } else {
                starIcon.classList.replace('bi-star-fill', 'bi-star');
            }
        }
    }).catch(error => console.error("Error:", error));
}

function ActionsOnJoin(projectid) {
    let formData = new FormData();
    formData.append('projectid', projectid);
    console.log('Am intrat in fn');
    fetch(
        '/Projects/ActionsMemberJoinProject/',
        {
            method: 'POST',
            body: formData
        }
    ).then(response => {
        if (response.ok)
            return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            const btnJoin = document.getElementById('join-btn');

            if (data.isActionAsk) {
                btnJoin.innerText = 'Cancel';
            } else {
                btnJoin.innerText = 'Ask to join';
            }
        }
    }).catch(error => console.error("Error:", error));
}

function InviteUser(projectId, userId) {
    console.log(projectId);
    console.log(userId);
    let formData = new FormData();
    formData.append('projectId', projectId);
    formData.append('userId', userId);
    fetch(
        '/Projects/InviteUser2Project/',
        {
            method: 'POST',
            body: formData
        }
    ).then(response => {
        if (response.ok)
            return response.json();
        throw Error(response.statusText)
    }).then(data => {
        if (data.success) {
            console.log(projectId);
            console.log(userId);
        }
    }).catch(error => console.error("Error:", error));
}

function RespondJoinRequest(projectId, memberId, accepted) {
    let formData = new FormData();
    formData.append('projectId', projectId);
    formData.append('memberId', memberId);
    formData.append('accepted', accepted);
    console.log(projectId + ' - ' + memberId + ' ' + accepted);
    fetch(
        '/Projects/RespondJoinRequest/',
        {
            method: 'POST',
            body: formData
        }
    ).then(response => {
        if (response.ok)
            return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            const pendingRequest = document.getElementById('pending-request-' + projectId + '-' + memberId);
            pendingRequest.classList.replace('d-flex', 'd-none');
            console.log(data.nrAnyPendReqLeft)
            if (data.nrAnyPendReqLeft == 0) {
                const btnShowPendReq = document.getElementById('btn-show-pend-req');
                btnShowPendReq.classList.add('d-none');
                console.log('fara oameni');
            } else if (data.nrAnyPendReqLeft == 1) {
                const btnShowPendReq = document.getElementById('btn-show-pend-req');
                btnShowPendReq.innerText = 'request';

            }
        }
    }).catch(error => console.error('Eroare:', error));
}

function KickUserFromProject(projectId, memberId) {
    let formData = new FormData();
    formData.append("projectId", projectId);
    formData.append("memberId", memberId);
    fetch(
        "/Projects/KickUserFromProject/",
        {
            method: 'POST',
            body: formData
        }
    ).then(response => {
        if (response.ok)
            return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            const kickDiv = document.getElementById('kick-' + projectId + '-' + memberId);
            kickDiv.classList.replace('d-flex', 'd-none');
        }
    }).catch(error => console.error('Eroare:', error));
}

function getContrastColor(hexColor) {
    const r = parseInt(hexColor.substr(1, 2), 16);
    const g = parseInt(hexColor.substr(3, 2), 16);
    const b = parseInt(hexColor.substr(5, 2), 16);

    const yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;

    return (yiq >= 128) ? '#000000' : '#FFFFFF';
}

function RemoveField(id, element) {
    // Remove Visual Badge
    element.parentElement.remove();

    // Remove Hidden Input
    const input = document.getElementById('input-' + id);
    if (input) input.remove();
}

// Updated function with 'fromLoad' parameter (defaults to false)
function AttachFieldToForm(id, title, color, fromLoad = false) {

    // Prevent duplicates
    if (document.getElementById('input-' + id)) {
        console.log("Field already added.");
        return;
    }

    // --- Visual: Create Badge ---
    const badgeContainer = document.getElementById('selectedFieldsBadges');
    const badge = document.createElement('div');
    badge.className = 'badge p-2 me-2 mb-2 d-flex align-items-center';

    const finalColor = color || '#563d7c';
    badge.style.backgroundColor = finalColor;

    const contrast = getContrastColor(finalColor);
    badge.style.color = contrast;
    badge.style.border = '1.8px solid ' + contrast;


    badge.innerHTML = `
            ${title} 
            <span class="ms-2" style="cursor:pointer; font-weight:bold;" onclick="RemoveField('${id}', this)">&times;</span>
        `;
    badgeContainer.appendChild(badge);

    // --- Logic: Create Hidden Input ---
    const inputsContainer = document.getElementById('hiddenInputsContainer');
    const input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'selectedFieldIds';
    input.value = id;
    input.id = 'input-' + id;
    inputsContainer.appendChild(input);

    if (!fromLoad) {
        $('#fieldModal').modal('hide');
    }
}


function CreateAndAttachField() {
    var titleVal = document.getElementById('newFieldName').value;
    var colorVal = document.getElementById('newFieldColor').value;

    if (!titleVal) {
        alert("Please enter a name");
        return;
    }

    let formData = new FormData();
    formData.append("Title", titleVal);
    formData.append("HexColor", colorVal);

    fetch("/Fields/New", {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            document.getElementById('addField').classList.add('d-none');
            document.getElementById('searchInput').value = "";

            AttachFieldToForm(data.id, data.title, data.color);

        } else {
            alert("Error creating field");
        }
    }).catch(error => console.error('Error:', error));
}

// AJAX: Add Comment (Works for both Root and Reply)
function ajaxAddComment(event, form, toggleId = null) {
    event.preventDefault();
    let formData = new FormData(form);

    fetch('/Comments/New', {
        method: 'POST',
        body: formData
    }).then(res => res.json()).then(data => {
        if (data.success) {
            form.reset();
            if (toggleId) toggle(toggleId);

            // Determine where to place the new comment
            let containerId;
            if (data.parentId) {
                containerId = `children-${data.parentId}`;
            } else if (data.taskParentId) {
                containerId = `root-comments-task-${data.taskParentId}`;
            } else {
                containerId = `root-comments-${data.projectParentId}`;
            }

            let container = document.getElementById(containerId);

            if (container) {
                // Remove "No comments" message if exists
                let noCommentsMsg = container.querySelector('.text-center.text-muted');
                if (noCommentsMsg) noCommentsMsg.remove();

                container.classList.remove('d-none');

                // INSERT THE SERVER-SIDE RENDERED HTML
                container.insertAdjacentHTML('afterbegin', data.html);

                // Optional: Re-initialize specific scripts if needed (rarely needed for simple CSS/onclicks)
            }
        } else {
            alert("Error: " + data.message);
        }
    }).catch(err => console.error(err));
}

// AJAX: Delete Comment
function ajaxDeleteComment(id) {
    if (!confirm('Are you sure you want to delete this comment?')) return;

    let formData = new FormData();
    formData.append('id', id);

    fetch('/Comments/Delete', {method: 'POST', body: formData})
        .then(res => res.json()).then(data => {
        if (data.success) {
            document.getElementById('comment-' + id).remove();
        } else {
            alert(data.message);
        }
    }).catch(err => console.error(err));
}

// AJAX: Edit Comment
function ajaxEditComment(event, id) {
    event.preventDefault();
    let content = document.getElementById('textarea-edit-' + id).value;
    let formData = new FormData();
    formData.append('id', id);
    formData.append('content', content);

    fetch('/Comments/Edit', {method: 'POST', body: formData})
        .then(res => res.json()).then(data => {
        if (data.success) {
            // Update text
            document.getElementById('content-' + id).innerText = data.content;
            // Close edit form using your existing toggle logic
            toggle('edit-' + id);
        } else {
            alert(data.message);
        }
    }).catch(err => console.error(err));
}

function ajaxCreateTask(projectId, status) {
    let formData = new FormData();
    formData.append('projectId', projectId);
    formData.append('status', status);

    console.log('am intrat fn');
    fetch('/ProjectTasks/Create', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            // Find the specific container for this status (e.g., 'task-container-ToDo')
            const container = document.getElementById('task-container-' + status);
            if (container) {
                // Append the new task HTML
                container.insertAdjacentHTML('beforeend', data.html);
            }
        }
    }).catch(error => console.error("Error creating task:", error));
}

function ajaxEditTaskTitle(taskId, newTitle) {
    let formData = new FormData();
    formData.append('taskId', taskId);
    formData.append('newTitle', newTitle);

    fetch('/ProjectTasks/EditTitle/', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            toggle(taskId + '-1');
            toggle(taskId + '-2');

            const textSpan = document.getElementById('task-title-text-' + taskId);

            if (textSpan) {
                textSpan.innerText = newTitle;
            }
        }
    }).catch(error => console.error(error));
}

function ajaxEditTaskDescription(taskId, description) {
    let formData = new FormData();
    formData.append('taskId', taskId);
    formData.append('description', description);

    fetch('/ProjectTasks/EditDescription/', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            // Toggle ALL 3 sections
            toggle(taskId + '-desc-1'); // Hide Textarea
            toggle(taskId + '-desc-2'); // Show Text
            toggle(taskId + '-desc-3'); // Hide Save/Cancel Buttons

            // Update the display text
            const textContainer = document.getElementById('task-desc-text-' + taskId);
            if (textContainer) {
                textContainer.innerHTML = description.replace(/\n/g, "<br>");
            }
        }
    }).catch(error => console.error(error));
}

function ajaxEditProjectTitle(projectId) {
    const newTitle = document.getElementById('input-project-title').value;
    let formData = new FormData();
    formData.append('projectId', projectId);
    formData.append('newTitle', newTitle);

    fetch('/Projects/EditTitle/', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            // Update the display text
            document.getElementById('display-project-title-text').innerText = newTitle;
            // Toggle back to display view
            toggle('project-title-edit');
            toggle('project-title-display');
        }
    }).catch(error => console.error(error));
}

function ajaxEditProjectDescription(projectId) {
    // Note: If you are using Summernote, you need to get the code from there. 
    // Assuming standard textarea for inline task-like edit:
    const description = document.getElementById('input-project-desc').value;

    let formData = new FormData();
    formData.append('projectId', projectId);
    formData.append('description', description);

    fetch('/Projects/EditDescription/', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            // Update display (handle newlines if simple text, or HTML if rich)
            document.getElementById('display-project-desc').innerHTML = description;
            toggle('project-desc-edit');
            toggle('project-desc-display');
        }
    }).catch(error => console.error(error));
}

function ajaxUpdateProjectFields(projectId) {
    // Collect all hidden inputs created by AttachFieldToForm
    const inputs = document.querySelectorAll('input[name="selectedFieldIds"]');
    let formData = new FormData();
    formData.append('projectId', projectId);

    inputs.forEach(input => {
        formData.append('selectedFieldIds', input.value);
    });

    fetch('/Projects/UpdateFields/', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            // Rebuild the Display Badges area
            const displayContainer = document.getElementById('display-project-fields');
            displayContainer.innerHTML = ''; // Clear current

            data.fields.forEach(field => {
                const contrast = getContrastColor(field.hexColor);
                const badge = document.createElement('span');
                badge.className = 'badge me-1';
                badge.style.backgroundColor = field.hexColor;
                badge.style.color = contrast;
                badge.style.border = '1.8px solid ' + contrast;
                badge.innerText = field.title;
                displayContainer.appendChild(badge);
            });

            // Toggle views
            toggle('project-fields-edit');
            toggle('project-fields-display');
        }
    }).catch(error => console.error(error));
}


function ajaxEditTaskStatus(taskId, statusValue) {
    let formData = new FormData();
    formData.append('taskId', taskId);
    // The controller expects 'statusEnum', which can parse the string name of the enum
    formData.append('statusEnum', statusValue);

    fetch('/ProjectTasks/EditStatus/', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            // Close edit mode
            toggle(taskId + '-status-1');
            toggle(taskId + '-status-2');

            // Refresh the whole page or update the specific badge text/color visually
            // For simplicity, we just reload to reflect the icon changes in the header as well
            location.reload();

            // OR if you want to avoid reload, you must manually update the text:
            // document.querySelector(`#to-toggle-${taskId}-status-2 .badge`).innerText = statusValue;
        }
    }).catch(error => console.error(error));
}

function ajaxEditTaskDeadline(taskId, rawDateValue) {
    // rawDateValue comes from datetime-local like "2023-10-25T14:30"
    if(!rawDateValue) {
        alert("Please select a date and time");
        return;
    }

    let formData = new FormData();
    formData.append('taskId', taskId);
    formData.append('deadline', rawDateValue);

    fetch('/ProjectTasks/EditDeadLine/', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            // Close edit mode
            toggle(taskId + '-deadline-1');
            toggle(taskId + '-deadline-2');

            // Format date for display (Client side formatting is a bit tricky, simple approach:)
            const dateObj = new Date(rawDateValue);
            const options = { day: 'numeric', month: 'short', year: 'numeric', hour: '2-digit', minute:'2-digit' };
            const prettyDate = dateObj.toLocaleDateString('en-GB', options); // Adjust locale as needed

            // Update the display text
            const displaySpan = document.getElementById('display-deadline-' + taskId);
            if (displaySpan) {
                // Keep the icon when updating text
                displaySpan.innerHTML = `${prettyDate} <i class="bi bi-pencil-fill ms-1 text-muted" style="font-size: 0.7em;"></i>`;
            }
        }
    }).catch(error => console.error(error));
}

function ajaxUploadMedia(taskId, inputElement) {
    if (inputElement.files.length === 0) return;

    let file = inputElement.files[0];
    let formData = new FormData();
    formData.append('taskId', taskId);
    formData.append('mediaFile', file);

    // UI: Show loading
    const container = document.getElementById('media-container-' + taskId);
    const spinner = document.getElementById('media-spinner-' + taskId);
    container.style.opacity = '0.3';
    spinner.classList.remove('d-none');

    fetch('/ProjectTasks/UploadMedia', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            // UI: Reset loading
            spinner.classList.add('d-none');
            container.style.opacity = '1';

            if (data.success) {
                const ext = data.url.split('.').pop().toLowerCase();
                let html = '';

                // Check if video or image
                if (['mp4', 'webm', 'ogg'].includes(ext)) {
                    html = `<video controls class="img-fluid rounded" style="max-height: 300px; width: 100%;">
                            <source src="${data.url}" type="video/${ext}">
                        </video>`;
                } else {
                    html = `<img src="${data.url}" class="img-fluid rounded" style="max-height: 300px; object-fit: contain;" />`;
                }

                container.innerHTML = html;
                document.getElementById('btn-delete-media-' + taskId).classList.remove('d-none');
            } else {
                alert('Upload failed: ' + (data.message || 'Unknown error'));
            }
        })
        .catch(error => {
            console.error('Error:', error);
            spinner.classList.add('d-none');
            container.style.opacity = '1';
        });
}

function ajaxDeleteMedia(taskId) {
    if(!confirm("Are you sure? This cannot be undone.")) return;

    let formData = new FormData();
    formData.append('taskId', taskId);

    fetch('/ProjectTasks/DeleteMedia', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const container = document.getElementById('media-container-' + taskId);
                container.innerHTML = `
                <div class="text-muted py-4 opacity-50">
                    <i class="bi bi-images fs-1"></i>
                    <p class="small m-0">No media uploaded</p>
                </div>`;

                document.getElementById('btn-delete-media-' + taskId).classList.add('d-none');
                document.getElementById('media-upload-' + taskId).value = '';
            }
        })
        .catch(error => console.error('Error:', error));
}


// [site.js] - Updated Assignment Logic & Search

function ajaxAsignOrRemoveUserFromTask(userId, taskId, userName) {
    let formData = new FormData();
    formData.append('userId', userId);
    formData.append('taskId', taskId);

    // Get button in the Modal (if it exists) to update its state
    const modalBtn = document.getElementById(`btn-assign-${taskId}-${userId}`);

    // Get the container in the Task Details view
    const listContainer = document.getElementById(`assigned-users-container-${taskId}`);

    fetch('/ProjectTasks/AsignOrRemoveUserToTask/', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {

            // --- CASE 1: User was ADDED ---
            if (data.isAdded) {
                // 1. Update Modal Button (Visual feedback)
                if (modalBtn) {
                    modalBtn.classList.replace('btn-outline-secondary', 'btn-success');
                    modalBtn.innerText = "Assigned";
                }

                // 2. Add to "Assigned To" List in Task Details
                // We check if it already exists to avoid duplicates
                if (!document.getElementById(`assigned-user-row-${taskId}-${userId}`)) {
                    const newRow = `
                        <div class="d-flex align-items-center justify-content-between mb-2" id="assigned-user-row-${taskId}-${userId}">
                            <div>
                                <i class="bi bi-person-circle fs-6 me-1 text-secondary"></i>
                                <span class="small">${userName}</span>
                            </div>
                            <div>
                                <button class="btn btn-sm btn-outline-danger" 
                                        onclick="ajaxAsignOrRemoveUserFromTask('${userId}', '${taskId}', '${userName}')">
                                    Remove
                                </button>
                            </div>
                        </div>`;

                    // Remove "Unassigned" text if it exists
                    const unassignedMsg = document.getElementById(`unassigned-msg-${taskId}`);
                    if(unassignedMsg) unassignedMsg.remove();

                    listContainer.insertAdjacentHTML('beforeend', newRow);
                }
            }
            // --- CASE 2: User was REMOVED ---
            else {
                // 1. Update Modal Button
                if (modalBtn) {
                    modalBtn.classList.replace('btn-success', 'btn-outline-secondary');
                    modalBtn.innerText = "Assign";
                }

                // 2. Remove from "Assigned To" List
                const rowToRemove = document.getElementById(`assigned-user-row-${taskId}-${userId}`);
                if (rowToRemove) {
                    rowToRemove.remove();
                }

                // 3. If list is empty, show "Unassigned" (Optional cosmetic polish)
                if (listContainer.children.length === 0) {
                    listContainer.innerHTML = `<span class="small text-muted" id="unassigned-msg-${taskId}">Unassigned</span>`;
                }
            }
        }
    }).catch(error => console.error("Error:", error));
}

// Search Function for the Modal
function filterUsers(taskId) {
    const input = document.getElementById(`search-users-${taskId}`);
    const filter = input.value.toLowerCase();
    const container = document.getElementById(`user-list-container-${taskId}`);
    const users = container.getElementsByClassName('user-item');

    for (let i = 0; i < users.length; i++) {
        const userName = users[i].getAttribute('data-username').toLowerCase();
        if (userName.includes(filter)) {
            users[i].classList.remove('d-none');
            users[i].classList.add('d-flex');
        } else {
            users[i].classList.remove('d-flex');
            users[i].classList.add('d-none');
        }
    }
}