using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyPortfolio.DataAccess.EntityModels;

namespace MyPortfolio.DataAccess.EntityConfigs
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entityBuilder)
        {
            entityBuilder.ToTable("user"); //custom table name mapping
            entityBuilder.HasKey(e => e.Id); //primary key
            //entityBuilder.HasAlternateKey(e => e.Email); //unique key constraint
            entityBuilder.HasIndex(e=>e.Email).IsUnique(); //unique index on Email
            entityBuilder.Property(e => e.FirstName).HasColumnType("varchar(100)").IsRequired(); //custom column type and not null constraint
            entityBuilder.Property(e => e.LastName).HasColumnType("varchar(100)").IsRequired(); //custom column type and not null constraint
            entityBuilder.Property(e => e.Email).HasColumnType("varchar(255)").IsRequired(); //custom column type and not null constraint
            entityBuilder.Property(e => e.PasswordHash).HasColumnType("varchar(500)").IsRequired();//custom column type and not null constraint
            entityBuilder.Property(e => e.PasswordSalt).HasColumnType("varchar(500)").IsRequired();//custom column type and not null constraint
            entityBuilder.Property(e => e.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();//custom column type, default value and not null constraint
            entityBuilder.Property(e => e.LastLoginDate).HasColumnType("datetime").IsRequired(required: false);//custom column type and nullable
            entityBuilder.Property(e => e.IsActive).HasColumnType("bit").HasDefaultValue(true).IsRequired();// 
            entityBuilder.Property(e => e.Role).HasColumnType("int").IsRequired(required: false);//custom column type and nullable
            entityBuilder.Property(e => e.ProfilePictureUrl).HasColumnType("varchar(500)").IsRequired(required: false);//custom column type and nullable
        }
    }
}
