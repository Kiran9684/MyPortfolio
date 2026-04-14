export const MainMenuBinders = {
    attachEvents: function (handlers) {
        $('div.sidebar li').on('click', {
            menuItemsContainer: "div.sidebar",
            menuItem: "a.nav-link",
            activeFlag: "active",
            textColor : "text-white"
        }, handlers.setActiveNavItem)
    }
}