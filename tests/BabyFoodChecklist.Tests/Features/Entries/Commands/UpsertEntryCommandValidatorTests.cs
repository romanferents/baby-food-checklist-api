using BabyFoodChecklist.Application.Features.Entries.Commands.UpsertEntry;

namespace BabyFoodChecklist.Tests.Features.Entries.Commands;

[TestFixture]
public class UpsertEntryCommandValidatorTests
{
    private UpsertEntryCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new UpsertEntryCommandValidator();

    [Test]
    public async Task Validate_ReturnsValid_WhenCommandIsValid()
    {
        var command = new UpsertEntryCommand { ProductId = Guid.NewGuid(), Tried = true };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Validate_ReturnsInvalid_WhenProductIdIsEmpty()
    {
        var command = new UpsertEntryCommand { ProductId = Guid.Empty };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }
}
