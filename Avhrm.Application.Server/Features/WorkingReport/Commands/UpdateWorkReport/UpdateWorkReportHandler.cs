using AutoMapper;
using Avhrm.Domains;
using Avhrm.Persistence.Services;
using Microsoft.AspNetCore.Http;

namespace Avhrm.Application.Server.Features;
public class UpdateWorkReportHandler(IMapper mapper
    , AvhrmDbContext context
    , IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateWorkReportCommand, UpdateWorkReportVm>
{
    public async Task<UpdateWorkReportVm> Handle(UpdateWorkReportCommand request, CancellationToken cancellationToken)
    {
        var workReport = await context.WorkingReports
                                                .Include(p=> p.WorkChallenges)
                                                .FirstOrDefaultAsync(p=>p.Id == request.Id, cancellationToken);

        ManageWorkChallenges(workReport, request);

        workReport.WorkReportTimeOfDay = (int)request.WorkReportTimeOfDay;

        workReport.WorkDayType = (int)request.WorkDayType;

        workReport.WorkTypeId = request.WorkTypeId;

        workReport.CustomerId = request.CustomerId;

        workReport.ProjectId = request.ProjectId;

        workReport.SpentHours = request.SpentHours;

        workReport.Desc = request.Desc;

        workReport.PersianDate = PersianCalendarTools.GregorianToPersian(request.PersianDate);

        context.Update(workReport);

        return new()
        {
            Result = await context.SaveChangesAsync() > 0
        };
    }

    private void ManageWorkChallenges(WorkReport workReport, UpdateWorkReportCommand request)
    {
        var existingWorkChallenges = workReport.WorkChallenges.ToList();

        foreach (var existingWorkChallenge in existingWorkChallenges)
        {
            if (!request.WorkChallengesIds.Contains(existingWorkChallenge.Id))
            {
                workReport.WorkChallenges.Remove(existingWorkChallenge);
            }
        }

        foreach (var workChallengeId in request.WorkChallengesIds)
        {
            if (!workReport.WorkChallenges.Any(wc => wc.Id == workChallengeId))
            {
                var workChallenge = context.WorkChallenges.Local.FirstOrDefault(wc => wc.Id == workChallengeId)
                                    ?? new WorkChallenge { Id = workChallengeId };

                if (!context.WorkChallenges.Local.Any(wc => wc.Id == workChallengeId))
                {
                    context.WorkChallenges.Attach(workChallenge);
                }

                workReport.WorkChallenges.Add(workChallenge);
            }
        }
    }
}
