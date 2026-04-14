export const Handlers = {
    setActiveNavItem: function (eventObj) {
        var container = $(eventObj.currentTarget).closest(eventObj.data.menuItemsContainer);

        //console.log(eventObj, container)
        //console.log($(container).find(eventObj.data.menuItem));

        var items = $(container).find(eventObj.data.menuItem);

        $.each(items, function (key, value) {
            //console.log(key, value);
            if ($(value).hasClass(eventObj.data.activeFlag)){
                $(value).removeClass(eventObj.data.activeFlag)
                $(value).addClass(eventObj.data.textColor)
            }
        })

        var clickedMenuOption = $(eventObj.currentTarget).find(eventObj.data.menuItem)
        //console.log(clickedMenuOption)
        $(clickedMenuOption).addClass(eventObj.data.activeFlag)
        $(clickedMenuOption).removeClass(eventObj.data.textColor)
    }
}