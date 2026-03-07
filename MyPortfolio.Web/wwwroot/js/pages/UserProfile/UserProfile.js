import { Binders } from "./UserProfileBinders.js";
import { Handlers } from "./UserProfileHandler.js";

$(document).ready(function () {
    Binders.attachEvents(Handlers)
})