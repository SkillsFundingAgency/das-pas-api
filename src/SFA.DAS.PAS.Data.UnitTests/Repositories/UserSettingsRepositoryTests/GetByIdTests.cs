using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.PAS.Data.UnitTests.Repositories.UserSettingsRepositoryTests;

internal class GetByIdTests : UserSettingsRepositoryTestBase
{
    [Test]
    public async Task WhenGettingById_ThenReturnsSettingsForRequestedUserRef()
    {
        var expected = CreateSettings(2, "requested-user", false);
        await SeedAsync(CreateSettings(1, "other-user", true), expected);

        var result = await Sut.GetById(expected.UserRef, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(expected.Id));
            Assert.That(result.UserId, Is.EqualTo(expected.UserId));
            Assert.That(result.UserRef, Is.EqualTo(expected.UserRef));
            Assert.That(result.ReceiveNotifications, Is.False);
            Assert.That(Context.Entry(result).State, Is.EqualTo(EntityState.Unchanged));
        });
    }

    [Test]
    public async Task WhenUserRefDoesNotExist_ThenReturnsNull()
    {
        await SeedAsync(CreateSettings(1, "other-user", true));

        var result = await Sut.GetById("missing-user", CancellationToken.None);

        Assert.That(result, Is.Null);
    }
}
