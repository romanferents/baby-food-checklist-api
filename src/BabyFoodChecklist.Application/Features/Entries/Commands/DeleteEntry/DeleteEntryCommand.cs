namespace BabyFoodChecklist.Application.Features.Entries.Commands.DeleteEntry;

public record DeleteEntryCommand(Guid Id) : IRequest;
