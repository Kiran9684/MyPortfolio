export const Binders = {
    attachEvents: function (handlers) {

        var formFields = {
            passwords  : ["#registerForm input#Password", "#registerForm input#inputPasswordCopy"],
            passVerificationField : "span.passVerification"
        }
        $("#registerForm").on("submit", { formData: formFields }, handlers.validatePasswords);
    }
}