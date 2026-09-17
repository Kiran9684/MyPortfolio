using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyPortfolio.DataAccess.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.DataAccess.EntityConfigs
{
    public class UserProfilePictureEntityConfiguration : IEntityTypeConfiguration<UserProfilePicture>
    {
        public void Configure(EntityTypeBuilder<UserProfilePicture> builder)
        {
            builder.ToTable("userProfilePicture");
            builder.HasKey(p => p.Id);//PK
            builder.HasIndex(p => p.PublicImageId).IsUnique();
            builder.Property(p => p.ImageType).HasColumnType("varchar(25)");
            builder.Property(p => p.ImageData).HasColumnType("varbinary(max)");
            builder.HasOne<User>(p => p.User)
                .WithOne(u => u.ProfilePicture)
                .HasForeignKey<UserProfilePicture>(p => p.UserId);
        }
    }
}
