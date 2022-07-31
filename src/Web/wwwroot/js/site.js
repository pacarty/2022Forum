

function editComment(id) {
    document.getElementById("comment_show_" + id).style.display = 'none';
    document.getElementById("comment_edit_" + id).style.display = 'block';
}

function cancelEdit(id) {
    document.getElementById("comment_show_" + id).style.display = 'block';
    document.getElementById("comment_edit_" + id).style.display = 'none';
}

function deleteComment(id) {
    document.getElementById("comment_delete_" + id).style.display = 'block';
}

function cancelDelete(id) {
    document.getElementById("comment_delete_" + id).style.display = 'none';
}

function deleteCommentManagement(id) {
    document.getElementById("comment_delete_management_" + id).style.display = 'block';
}

function cancelDeleteCommentManagement(id) {
    document.getElementById("comment_delete_management_" + id).style.display = 'none';
}

function deletePostManagement(id) {
    document.getElementById("post_delete_management_" + id).style.display = 'block';
}

function cancelDeletePostManagement(id) {
    document.getElementById("post_delete_management_" + id).style.display = 'none';
}