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
        if (response.ok)
            return response.json();
        throw Error(response.statusText);
    }).then(data => {
        if (data.success) {
            const scoreElement = document.getElementById('score-' + commentId);
            if (scoreElement)
                scoreElement.innerText = data.score;
            const btnUp = document.getElementById('btn-up-' + commentId);
            const iconUp = document.getElementById('icon-up-' + commentId);

            if (data.userStatus === 1) { // upvote
                btnUp.classList.replace('text-muted', 'text-warning');
                iconUp.classList.replace('bi-caret-up', 'bi-caret-up-fill');
            } else { // not-upvoted
                btnUp.classList.replace('text-warning', 'text-muted');
                iconUp.classList.replace('bi-caret-up-fill', 'bi-caret-up');
            }

            const btnDown = document.getElementById('btn-down-' + commentId);
            const iconDown = document.getElementById('icon-down-' + commentId);

            if (data.userStatus === -1) { // downvote
                btnDown.classList.replace('text-muted', 'text-warning');
                iconDown.classList.replace('bi-caret-down', 'bi-caret-down-fill');
            } else { // not-downvoted
                btnDown.classList.replace('text-warning', 'text-muted');
                iconDown.classList.replace('bi-caret-down-fill', 'bi-caret-down');
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

function InviteUser(projectId, userId){
    console.log(projectId);
    console.log(userId);
    let formData = new FormData();
    formData.append('projectId', projectId);
    formData.append('userId', userId);
    fetch(
        '/Projects/InviteUser2Project/',
        {
            method: 'POST',
            body:formData
        }
    ).then (response => {
        if(response.ok)
            return response.json();
        throw Error(response.statusText)
    }).then(data => {
        if (data.success){
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