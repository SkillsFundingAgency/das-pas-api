using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Data.Repositories;
using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Data.UnitTests.Repositories.UserSettingsRepositoryTests;

internal abstract class UserSettingsRepositoryTestBase
{
    protected PasDataContext Context = null!;
    protected UserSettingsRepository Sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<PasDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new PasDataContext(options);
        Sut = new UserSettingsRepository(Context);
    }

    [TearDown]
    public void TearDown()
    {
        Context.Dispose();
    }

    protected async Task SeedAsync(params UserSettings[] settings)
    {
        Context.UserSettings.AddRange(settings);
        await Context.SaveChangesAsync(CancellationToken.None);
        Context.ChangeTracker.Clear();
    }

    protected static UserSettings CreateSettings(long id, string userRef, bool receiveNotifications)
    {
        return new UserSettings
        {
            Id = id,
            UserId = id,
            UserRef = userRef,
            ReceiveNotifications = receiveNotifications
        };
    }
}
