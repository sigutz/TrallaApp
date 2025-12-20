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

