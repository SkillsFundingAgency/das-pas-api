using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.PAS.Data.UnitTests.Repositories.UserSettingsRepositoryTests;

internal class InsertTests : UserSettingsRepositoryTestBase
{
    [TestCase(true)]
    [TestCase(false)]
    public async Task WhenInserting_ThenPersistsSettings(bool receiveNotifications)
    {
        var settings = CreateSettings(1, "new-user", receiveNotifications);

        await Sut.Insert(settings, CancellationToken.None);
        Context.ChangeTracker.Clear();

        var saved = await Context.UserSettings.SingleAsync();
        Assert.Multiple(() =>
        {
            Assert.That(saved.Id, Is.EqualTo(settings.Id));
            Assert.That(saved.UserId, Is.EqualTo(settings.UserId));
            Assert.That(saved.UserRef, Is.EqualTo(settings.UserRef));
            Assert.That(saved.ReceiveNotifications, Is.EqualTo(receiveNotifications));
        });
    }
}
