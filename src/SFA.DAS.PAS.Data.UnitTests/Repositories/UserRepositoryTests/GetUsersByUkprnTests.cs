using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Data.Repositories;
using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Data.UnitTests.Repositories.UserRepositoryTests;

public class GetUsersByUkprnTests
{
    private const int Ukprn = 10000001;
    private PasDataContext _context = null!;
    private UserRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<PasDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new PasDataContext(options);
        _sut = new UserRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task WhenGettingUsers_ThenFiltersOutDeletedUsers()
    {
        var firstUser = CreateUser(1, Ukprn);
        var secondUser = CreateUser(2, Ukprn);
        var deletedUser = CreateUser(3, Ukprn, isDeleted: true);
        var otherProviderUser = CreateUser(4, Ukprn + 1);
        _context.Users.AddRange(firstUser, secondUser, deletedUser, otherProviderUser);
        await _context.SaveChangesAsync(CancellationToken.None);
        _context.ChangeTracker.Clear();

        var result = await _sut.GetUsersByUkprn(Ukprn, CancellationToken.None);

        Assert.That(result.Select(user => user.Id), Is.EquivalentTo(new[] { firstUser.Id, secondUser.Id }));
    }

    [Test]
    public async Task WhenThereAreNoUsers_ThenReturnsEmptyList()
    {
        var result = await _sut.GetUsersByUkprn(Ukprn, CancellationToken.None);

        Assert.That(result, Is.Empty);
    }

    [TestCase(Ukprn + 1, false)]
    [TestCase(Ukprn, true)]
    public async Task WhenThereAreNoMatchingActiveUsers_ThenReturnsEmptyList(int userUkprn, bool isDeleted)
    {
        _context.Users.Add(CreateUser(1, userUkprn, isDeleted));
        await _context.SaveChangesAsync(CancellationToken.None);
        _context.ChangeTracker.Clear();

        var result = await _sut.GetUsersByUkprn(Ukprn, CancellationToken.None);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task WhenGettingUsers_ThenIncludesUserSettings()
    {
        var user = CreateUser(1, Ukprn);
        user.UserSettings = new UserSettings
        {
            Id = 1,
            UserId = user.Id,
            UserRef = user.UserRef,
            ReceiveNotifications = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(CancellationToken.None);
        _context.ChangeTracker.Clear();

        var result = await _sut.GetUsersByUkprn(Ukprn, CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].UserSettings, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result[0].UserSettings!.UserId, Is.EqualTo(user.Id));
            Assert.That(result[0].UserSettings!.UserRef, Is.EqualTo(user.UserRef));
            Assert.That(result[0].UserSettings!.ReceiveNotifications, Is.True);
        });
    }

    [Test]
    public async Task WhenGettingUsers_ThenReturnsUserWithoutSettings()
    {
        var user = CreateUser(1, Ukprn);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(CancellationToken.None);
        _context.ChangeTracker.Clear();

        var result = await _sut.GetUsersByUkprn(Ukprn, CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(user.Id));
            Assert.That(result[0].UserSettings, Is.Null);
        });
    }

    private static User CreateUser(long id, int ukprn, bool isDeleted = false)
    {
        return new User
        {
            Id = id,
            UserRef = $"user-{id}",
            DisplayName = $"User {id}",
            Ukprn = ukprn,
            Email = $"user-{id}@example.com",
            IsDeleted = isDeleted
        };
    }
}
