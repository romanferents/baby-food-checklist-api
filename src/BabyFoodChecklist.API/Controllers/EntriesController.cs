using Asp.Versioning;
using BabyFoodChecklist.Application.Features.Entries.Commands.DeleteEntry;
using BabyFoodChecklist.Application.Features.Entries.Commands.UpsertEntry;

namespace BabyFoodChecklist.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class EntriesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertEntryCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteEntryCommand(id), cancellationToken);
        return NoContent();
    }
}
