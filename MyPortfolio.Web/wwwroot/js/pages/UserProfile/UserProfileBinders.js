export const Binders = {
    attachEvents: function (handlers) {
        $("#userProfileDetails").on('input', { saveBtn: "#userProfileDetails button" }, handlers.toggleOnSave);
        $("#btnUploadProfilePic").on('click', { profilePicformId: "#profilePicForm", profileFormDetailsId: "#userProfileDetails"}, handlers.processProfilePicture);
    }
}