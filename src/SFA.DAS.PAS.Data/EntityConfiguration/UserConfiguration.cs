using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Data.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User", "dbo");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Ukprn)
            .HasConversion<long>();

        builder.HasOne(user => user.UserSettings)
            .WithOne()
            .HasForeignKey<UserSettings>(settings => settings.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
