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