using AutoMapper;
using Avhrm.Persistence.Services;

namespace Avhrm.Application.Server.Features;
public class GetWorkReportOfChildUsersHandler(AvhrmDbContext context
    , IMapper mapper) : IRequestHandler<GetWorkReportOfChildUsersQuery, GetWorkReportOfChildUsersVm>
{
    public async Task<GetWorkReportOfChildUsersVm> Handle(GetWorkReportOfChildUsersQuery request, CancellationToken cancellationToken)
    => new()
    {
        Data = mapper.Map<List<GetWorkReportOfChildUsersDto>>(context.WorkingReports
                                                                     .Where(x => x.CreatorUserId == request.UserId && x.PersianDate == PersianCalendarTools.GregorianToPersian(request.Date)))
    };
}
