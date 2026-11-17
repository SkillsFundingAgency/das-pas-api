using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.PAS.Data.Repositories;

namespace SFA.DAS.PAS.Data.UnitTests.Repositories.UserSettingsRepositoryTests;

internal class UpdateTests : UserSettingsRepositoryTestBase
{
    [TestCase(true)]
    [TestCase(false)]
    public async Task WhenUpdating_ThenPersistsChangesWithoutAffectingOtherUsers(bool receiveNotifications)
    {
        var settings = CreateSettings(1, "user-ref", !receiveNotifications);
        var other = CreateSettings(2, "other-user", !receiveNotifications);
        await SeedAsync(settings, other);
        var tracked = await Sut.GetById(settings.UserRef, CancellationToken.None);
        tracked!.ReceiveNotifications = receiveNotifications;

        await Sut.Update(tracked, CancellationToken.None);
        Context.ChangeTracker.Clear();

        var saved = await Context.UserSettings.OrderBy(entity => entity.Id).ToListAsync();
        Assert.That(saved, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(saved[0].Id, Is.EqualTo(settings.Id));
            Assert.That(saved[0].UserId, Is.EqualTo(settings.UserId));
            Assert.That(saved[0].UserRef, Is.EqualTo(settings.UserRef));
            Assert.That(saved[0].ReceiveNotifications, Is.EqualTo(receiveNotifications));
            Assert.That(saved[1].UserRef, Is.EqualTo(other.UserRef));
            Assert.That(saved[1].ReceiveNotifications, Is.EqualTo(other.ReceiveNotifications));
        });
    }

    [Test]
    public async Task WhenUpdating_ThenSavesChangesOnceWithSuppliedToken()
    {
        using var cancellation = new CancellationTokenSource();
        var context = new Mock<IPasDataContext>(MockBehavior.Strict);
        context.Setup(data => data.SaveChangesAsync(cancellation.Token)).ReturnsAsync(1);
        var sut = new UserSettingsRepository(context.Object);

        await sut.Update(CreateSettings(1, "user-ref", true), cancellation.Token);

        context.Verify(data => data.SaveChangesAsync(cancellation.Token), Times.Once);
        context.VerifyNoOtherCalls();
    }

    [TestCase(false)]
    [TestCase(true)]
    public void WhenSaveFails_ThenPropagatesException(bool isCancelled)
    {
        using var cancellation = new CancellationTokenSource();
        Exception failure = isCancelled
            ? new OperationCanceledException(cancellation.Token)
            : new DbUpdateException("Save failed");
        var context = new Mock<IPasDataContext>(MockBehavior.Strict);
        context.Setup(data => data.SaveChangesAsync(cancellation.Token)).ThrowsAsync(failure);
        var sut = new UserSettingsRepository(context.Object);

        Assert.That(async () => await sut.Update(CreateSettings(1, "user-ref", true), cancellation.Token),
            Throws.Exception.SameAs(failure));
    }
}
