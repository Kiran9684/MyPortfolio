export const Handlers = {
    toggleOnSave: function (eveObj) {
        $(eveObj.data.saveBtn).prop("disabled", false)
    }
}