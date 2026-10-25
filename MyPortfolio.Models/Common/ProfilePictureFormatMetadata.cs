using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Models.Common
{
    public sealed class ProfilePictureFormatMetadata
    {
        public string Extension { get; }
        public string MimeType { get; }
        public IReadOnlyList<byte[]> Signatures { get; }

        private ProfilePictureFormatMetadata(string extension, string mimeType, IReadOnlyList<byte[]> signatures) 
        { 
            Extension = extension;
            MimeType = mimeType;
            Signatures = signatures;
        }

        public static IReadOnlyDictionary<string, ProfilePictureFormatMetadata> SupportedFormats { get; } = new ReadOnlyDictionary<string, ProfilePictureFormatMetadata>(
            new Dictionary<string, ProfilePictureFormatMetadata>(StringComparer.OrdinalIgnoreCase)
            {
                [".png"] = new ProfilePictureFormatMetadata(".png", "image/png",new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }}),
                [".jpeg"] = new ProfilePictureFormatMetadata(".jpeg", "image/jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } }),
                [".jpg"] = new ProfilePictureFormatMetadata(".jpg", "image/jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } })
            }
        );
    }
}
