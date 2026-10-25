using MyPortfolio.Business.Utilities;
using MyPortfolio.Models.AppModels;
using MyPortfolio.Models.Common;
using MyPortfolio.Web.Controllers;
using System.IO;

namespace MyPortfolio.Web.Facades
{
    public class UserFacade : IUserFacade
    {
        private IConfiguration _configuration;
        private ILogger<UserController> _logger;
        public UserFacade(IConfiguration configuration, ILogger<UserController> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        public async Task<(ValidationResult validation, byte[]? imageData, string? imageType)>  ValidateProfilePictureAsync(IFormFile file , ProfilePictureSettings settings)
        {
            bool signatureValid = false;
            var result = new ValidationResult() { Success = true , Messages = new List<string>()};
            byte[]? imageData = null;
            string imageType = string.Empty;

            //1.File extension validation
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if(!settings.PermittedExtensions.Any(ext => ext == fileExtension))
            {
                result.Success = false;
                result.Messages.Add(AppMessage.ERROR_PROFILE_PICTURE_INVALID_EXTENSION);
                _logger.LogWarning("[ValidateProfilePictureAsync] Validation failed. Unsupported file extension detected. Extension: {Extension}", fileExtension);
                return (result,null,null);
            }

            //2.File size validation
            if (file.Length > settings.SizeInBytes)
            {
                result.Success = false;
                result.Messages.Add(AppMessage.ERROR_PROFILE_PICTURE_SIZE_EXCEEDED);
                
                _logger.LogWarning("[ValidateProfilePictureAsync] Validation failed. File size exceeds configured limit. FileSize: {FileSize} bytes. Limit: {Limit} bytes.", file.Length,settings.SizeInBytes);

                return (result, null, null);
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                //3.File signature validation
                var signatures = ProfilePictureFormatMetadata.SupportedFormats[fileExtension].Signatures;

                using (var reader = new BinaryReader(memoryStream))
                {
                   memoryStream.Position = 0;
                   var headerBytes = reader.ReadBytes(signatures.Max(s => s.Length));
                   signatureValid = signatures.Any(s => s.SequenceEqual(headerBytes.Take(s.Length)));

                    if (signatureValid)
                    {
                        memoryStream.Position = 0;
                        imageData = reader.ReadBytes((int)file.Length);
                    }
                }

                imageType = ProfilePictureFormatMetadata.SupportedFormats[fileExtension].MimeType;
            }

            if (!signatureValid)
            {
                result.Success = false;
                result.Messages.Add(AppMessage.ERROR_PROFILE_PICTURE_INVALID_CONTENT);
                _logger.LogWarning("[ValidateProfilePictureAsync] Validation failed. File signature verification unsuccessful.");
            }
            _logger.LogInformation("[ValidateProfilePictureAsync] Validation completed successfully. ContentType: {ImageType}", imageType);
            return (result,imageData, imageType);
        }
    }
}
