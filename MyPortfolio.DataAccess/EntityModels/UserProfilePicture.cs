using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.DataAccess.EntityModels
{
    public class UserProfilePicture
    {
        public int Id { get; set; }
        public byte[]? ImageData {  get; set; }
        public string? ImageType { get; set; }
        public Guid? PublicImageId { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
