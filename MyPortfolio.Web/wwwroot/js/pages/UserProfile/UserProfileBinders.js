export const Binders = {
    attachEvents: function (handlers) {
        $("#userProfileDetails").on('input', { saveBtn: "#userProfileDetails button" },handlers.toggleOnSave)
    }
}