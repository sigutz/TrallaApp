// Toggles the visibility of the reply form
function toggleReplyForm(id) {
    const form = document.getElementById(`reply-form-${id}`);
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