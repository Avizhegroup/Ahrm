using AutoMapper;
using Avhrm.Persistence.Services;
using Microsoft.AspNetCore.Http;

namespace Avhrm.Application.Server.Features;
public class GetChildUsersHandler(AvhrmDbContext context
    , IHttpContextAccessor httpContextAccessor
    , IMapper mapper) : IRequestHandler<GetChildUsersQuery, GetChildUsersVm>
{
    public async Task<GetChildUsersVm> Handle(GetChildUsersQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext.User.GetUserId();
        
        var childUsers = await GetChildUsersRecursively(userId, cancellationToken);
        
        return new ()
        {
            Data = childUsers 
        };
    }

    private async Task<List<GetChildUsersDto>> GetChildUsersRecursively(string userId, CancellationToken cancellationToken)
    {
        var users = await context.Users
                                                   .Where(u => u.ParentId == userId)
                                                   .ToListAsync(cancellationToken);

        List<GetChildUsersDto>? allChildUsers = new(mapper.Map<List<GetChildUsersDto>>(users));
   
        foreach (var user in users)
        {
            List<GetChildUsersDto>? childUsers = await GetChildUsersRecursively(user.Id, cancellationToken);
        
            allChildUsers.AddRange(childUsers);
        }
     
        return allChildUsers;
    }
}
