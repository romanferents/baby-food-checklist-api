using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Common.Interfaces;
using BabyFoodChecklist.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var product = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id && (p.UserId == null || p.UserId == userId), cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        return mapper.Map<ProductDto>(product);
    }
}
