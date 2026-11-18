using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Data.UnitTests.EntityConfiguration;

public class UkprnMappingTests
{
    private PasDataContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<PasDataContext>()
            .UseSqlServer("Server=(local);Database=Unused;Trusted_Connection=True;")
            .Options;
        _context = new PasDataContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [TestCase(typeof(User))]
    [TestCase(typeof(Provider))]
    public void UkprnIsAnIntInTheModelAndAnInt64InTheDatabase(Type entityType)
    {
        var property = _context.Model.FindEntityType(entityType)!.FindProperty("Ukprn")!;

        Assert.Multiple(() =>
        {
            Assert.That(property.ClrType, Is.EqualTo(typeof(int)));
            Assert.That(property.GetProviderClrType(), Is.EqualTo(typeof(long)));
        });
    }
}
