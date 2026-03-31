namespace BabyFoodChecklist.Application.Features.Entries.Commands.UpsertEntry;

public class UpsertEntryCommandValidator : AbstractValidator<UpsertEntryCommand>
{
    public UpsertEntryCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Rating).IsInEnum().When(x => x.Rating.HasValue);
    }
}
