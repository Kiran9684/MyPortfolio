export const Handlers = {
    validatePasswords: function (eventObj) {
        console.log(eventObj);

        var pass = $(eventObj.data.formData.passwords[0]).val();
        var confirmPass = $(eventObj.data.formData.passwords[1]).val();

        console.log(pass, confirmPass);

        if (pass && confirmPass) {
            if (pass !== confirmPass) {
                $(eventObj.data.formData.passVerificationField).empty();
                $(eventObj.data.formData.passVerificationField).append('Passwords do not match!');
                eventObj.preventDefault(); // stop form submission
            }
            else {
                var regexpd = /^(?=.{8,}$)(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).*$/;
                if (!regexpd.test(pass)) {
                    eventObj.preventDefault(); // stop form submission
                    $(eventObj.data.formData.passVerificationField).empty();
                    $(eventObj.data.formData.passVerificationField).append('Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.');
                }
            }
        }

        if (pass && !confirmPass) {
            $(eventObj.data.formData.passVerificationField).empty();
            $(eventObj.data.formData.passVerificationField).append('Confirm Passwords is empty!');
            eventObj.preventDefault(); // stop form submission
        }
    }
}