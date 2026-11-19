using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Data.Repositories;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Data.UnitTests.Repositories.UserRepositoryTests;

internal class GetByUserRefTests
{
    private PasDataContext _context = null!;
    private UserRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<PasDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _context = new PasDataContext(options);
        _sut = new UserRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    [RecursiveMoqInlineAutoData(false)]
    [RecursiveMoqInlineAutoData(true)]
    public async Task WhenUserExists_ThenReturnsMatchingUser(bool isDeleted, User user)
    {
        user.IsDeleted = isDeleted;
        user.UserSettings = null;
        await SeedAsync(user);

        var result = await _sut.GetUserByUserRef(user.UserRef, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.UserRef, Is.EqualTo(user.UserRef));
            Assert.That(result.IsDeleted, Is.EqualTo(isDeleted));
        });
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenUserDoesNotExist_ThenReturnsNull(User user, string missingUserRef)
    {
        user.IsDeleted = false;
        user.UserSettings = null;
        await SeedAsync(user);

        var result = await _sut.GetUserByUserRef(missingUserRef, CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    [RecursiveMoqInlineAutoData(false)]
    [RecursiveMoqInlineAutoData(true)]
    public async Task WhenUserSettingsExist_ThenIncludesUserSettings(bool isDeleted, User user, UserSettings settings)
    {
        user.IsDeleted = isDeleted;
        user.UserSettings = LinkSettings(user, settings);
        await SeedAsync(user);

        var result = await _sut.GetUserByUserRef(user.UserRef, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserSettings, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.UserSettings.Id, Is.EqualTo(settings.Id));
            Assert.That(result.UserSettings.UserId, Is.EqualTo(user.Id));
            Assert.That(result.UserSettings.UserRef, Is.EqualTo(user.UserRef));
            Assert.That(result.UserSettings.ReceiveNotifications, Is.EqualTo(settings.ReceiveNotifications));
            Assert.That(_context.Entry(result.UserSettings).State, Is.EqualTo(EntityState.Unchanged));
        });
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenUserSettingsDoNotExist_ThenReturnsUserWithoutSettings(User user)
    {
        user.IsDeleted = false;
        user.UserSettings = null;
        await SeedAsync(user);

        var result = await _sut.GetUserByUserRef(user.UserRef, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserSettings, Is.Null);
    }

    // write test to ensure deleted user us returned as well
    [Test, RecursiveMoqAutoData]
    public async Task WhenUserIsDeleted_ThenReturnsDeletedUser(User user)
    {
        user.IsDeleted = true;
        user.UserSettings = null;
        await SeedAsync(user);

        // Action
        var result = await _sut.GetUserByUserRef(user.UserRef, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsDeleted, Is.True);
    }

    private static UserSettings LinkSettings(User user, UserSettings settings)
    {
        settings.UserId = user.Id;
        settings.UserRef = user.UserRef;
        return settings;
    }

    private async Task SeedAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(CancellationToken.None);
        _context.ChangeTracker.Clear();
    }
}
