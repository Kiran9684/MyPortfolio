import { MainMenuBinders } from "./MainMenuBinders.js";
import { Handlers } from "./MainMenuHandler.js";
$(document).ready(function () {
    MainMenuBinders.attachEvents(Handlers)
});