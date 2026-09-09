import { ApiService } from "../../services/ApiService.js";
import { AppUtilities } from "../../services/AppUtilities.js";

export const Handlers = {
    toggleOnSave: function (eveObj) {
        $(eveObj.data.saveBtn).prop("disabled", false)
    },
    processProfilePicture: function (eveObj) {
        //var form = $(eveObj.data.profilePicformId);
        var fileInput = $(`${eveObj.data.profilePicformId} input:file`); 
        var fileDom = fileInput[0];//to get raw dom element
        var file = fileDom.files[0];
        var userDetailsForm = $(eveObj.data.profileFormDetailsId);
        var url = $(this).data("url"); //this -> button element.
        UserProfilePageHelpers.processUserImage(file, userDetailsForm,url);

    }
}

class UserProfilePageHelpers {
    static maxSize = 2 * 1024 * 1024; //2 MB in bytes

    static processUserImage(file, userProfileForm,url) {

        if (!file) {
            toastr.error("Please select a file")
            return;
        }

        //1.Validate file type
        if (!(file.type === "image/jpeg" || file.type === "image/png")) {
            toastr.error("Invalid file type. Only JPEG or PNG allowed.");
            return;
        }

        //2.Validate file size
        if (file.size > UserProfilePageHelpers.maxSize) {
            toastr.error("File size exceeds 2MB!")
            return;
        }

        //3.Post successfull validation preview the image to user
        var imgTag = userProfileForm.find('img');

        const reader = new FileReader();

        //Define a callback function that runs when the file has been successfully read and preview.
        var fileData = null;
        reader.onload = (e) => {
            imgTag.attr('src', e.target.result);

        };
        reader.readAsDataURL(file); //Starts reading the file you selected.

        //Pack the data for the server
        let formData = new FormData();

        // CRITICAL: The string "ProfilePicture" must perfectly match the property name in your C# ProfilePictureUploadViewModel!
        formData.append("ProfilePicture", file);

        try {
            AppUtilities.enableGlobalSpinner();

            let apiService = new ApiService();
            // Notice we are passing the formData object directly!
            apiService.post(url, formData, {}, "json")
                .done(function (data, textStatus, jqXHR) {
                    console.log("done", data, textStatus, jqXHR);
                    toastr.success("Profile picture uploaded successfully!");
                    $('#profilePicModal').modal('hide');
                    // Update the image src on the UI here //TODO Work
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    console.log("fail", jqXHR, textStatus, errorThrown);
                    toastr.error("Failed to upload image.");
                    console.error(jqXHR);
                    //need to handle the response here
                })
        }
        catch (error) {
            toastr.error("Failed to upload image.");
            console.error(error)
        }
        finally {
            AppUtilities.disableGlobalSpinner();
        }
    }
}