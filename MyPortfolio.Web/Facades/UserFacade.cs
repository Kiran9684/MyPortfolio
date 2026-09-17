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
        private Dictionary<string, List<Byte[]>> _fileSignatures = new Dictionary<string, List<byte[]>>();

        public UserFacade(IConfiguration configuration, ILogger<UserController> logger)
        {
            this._configuration = configuration;
            this._logger = logger;

            _fileSignatures.Add(".png", new List<byte[]>() { new byte[] { 0x89,0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } });
            _fileSignatures.Add(".jpeg", new List<byte[]>() { new byte[] { 0xFF, 0xD8, 0xFF } });
            _fileSignatures.Add(".jpg", new List<byte[]>() { new byte[] { 0xFF, 0xD8, 0xFF } });
        }

        public async Task<(ValidationResult validation, byte[]? imageData, string? imageType)>  ValidateProfilePictureAsync(IFormFile file , ProfilePictureSettings settings)
        {
            bool signatureValid = false;
            var result = new ValidationResult() { Success = true , Messages = new List<string>()};
            byte[]? imageData = null;

            //1.File extension validation
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if(!settings.PermittedExtensions.Any(ext => ext == fileExtension))
            {
                result.Success = false;
                result.Messages.Add("Invalid file extension.");
                return (result,null,null);
            }

            //2.File size validation
            if (file.Length > settings.SizeInBytes)
            {
                result.Success = false;
                result.Messages.Add("File size exceeds the 2MB limit.");
                return (result, null, null);
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                //3.File signature validation
                var signatures = _fileSignatures[fileExtension];

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
            }

            if (!signatureValid)
            {
                result.Success = false;
                result.Messages.Add("Invalid file.");
            }
            return (result,imageData, fileExtension);
        }
    }
}
