# Test Pattern Reference

## Frameworks
- NUnit 3.14 (`[TestFixture]`, `[Test]`, `[SetUp]`)
- FluentAssertions 7.2 (`.Should()`, `.BeOfType<>()`, `.ThrowAsync<>()`)
- Moq 4.20 (`Mock<T>`, `.Setup()`, `.Verify()`)
- MockQueryable.Moq 7.0 (`.BuildMockDbSet()`)

## Handler Test Pattern
```csharp
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;

namespace BabyFoodChecklist.Tests.Features.{Feature};

[TestFixture]
public class {Handler}Tests
{
    private Mock<IApplicationDbContext> _contextMock;
    private IMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        var config = new MapperConfiguration(c => c.AddProfile<{MappingProfile}>());
        _mapper = config.CreateMapper();
    }

    [Test]
    public async Task Handle_Returns{Dto}_When{Entity}Exists()
    {
        // Arrange
        var entity = new {Entity}
        {
            Id = Guid.NewGuid(),
            NameUk = "Тест",
            NameEn = "Test",
            Category = ProductCategory.Other
        };
        var mockDbSet = new List<{Entity}> { entity }.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.{DbSet}).Returns(mockDbSet.Object);

        var handler = new {Handler}(_contextMock.Object, _mapper);

        // Act
        var result = await handler.Handle(new {Query}(entity.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
    }

    [Test]
    public async Task Handle_ThrowsNotFoundException_When{Entity}DoesNotExist()
    {
        // Arrange
        var mockDbSet = new List<{Entity}>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.{DbSet}).Returns(mockDbSet.Object);

        var handler = new {Handler}(_contextMock.Object, _mapper);

        // Act
        var act = async () => await handler.Handle(
            new {Query}(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
```

## Validator Test Pattern
```csharp
[TestFixture]
public class {Validator}Tests
{
    private {Validator} _validator;

    [SetUp]
    public void SetUp() => _validator = new {Validator}();

    [Test]
    public async Task Validate_ReturnsValid_WhenCommandIsValid()
    {
        var command = new {Command} { /* valid properties */ };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Validate_ReturnsInvalid_When{Property}IsEmpty()
    {
        var command = new {Command} { /* invalid properties */ };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "{Property}");
    }
}
```

## Controller Test Pattern
```csharp
[TestFixture]
public class {Controller}Tests
{
    private Mock<ISender> _senderMock;
    private {Controller} _controller;

    [SetUp]
    public void SetUp()
    {
        _senderMock = new Mock<ISender>();
        _controller = new {Controller}(_senderMock.Object);
    }

    [Test]
    public async Task {Action}_ReturnsOk_When{Condition}()
    {
        // Arrange
        var dto = new {Dto} { Id = Guid.NewGuid(), /* ... */ };
        _senderMock.Setup(s => s.Send(It.IsAny<{Query}>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.{Action}(dto.Id, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dto);
    }
}
```

## Naming Convention
`Method_ExpectedResult_WhenCondition`

Examples:
- `Handle_ReturnsProductDto_WhenProductExists`
- `Handle_ThrowsNotFoundException_WhenProductDoesNotExist`
- `Handle_ThrowsForbiddenException_WhenProductIsDefault`
- `Validate_ReturnsInvalid_WhenNameUkIsEmpty`
- `Create_ReturnsCreatedAtAction_WhenCommandIsValid`
