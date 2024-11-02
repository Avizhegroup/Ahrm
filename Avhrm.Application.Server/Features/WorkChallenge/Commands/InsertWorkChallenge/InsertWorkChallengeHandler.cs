using AutoMapper;
using Avhrm.Domains;
using Avhrm.Persistence.Services;
using Microsoft.AspNetCore.Http;

namespace Avhrm.Application.Server.Features;
public class InsertWorkChallengeHandler(AvhrmDbContext context
    , IMapper mapper
    , IHttpContextAccessor httpContextAccessor) : IRequestHandler<InsertWorkChallengeCommand, InsertWorkChallengeVm>
{
    public async Task<InsertWorkChallengeVm> Handle(InsertWorkChallengeCommand request, CancellationToken cancellationToken)
    {
        var workChallenge= mapper.Map<WorkChallenge>(request);

        await context.AddAsync(workChallenge, cancellationToken);

        return new()
        {
            Result = (await context.SaveChangesAsync(cancellationToken)) > 0
        };
    }
}
