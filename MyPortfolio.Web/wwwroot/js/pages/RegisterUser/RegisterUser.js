
import { Binders } from './RegisterUserBinder.js';
import { Handlers } from './RegisterUserHandler.js';

$(document).ready(function () {
    Binders.attachEvents(Handlers);
});
