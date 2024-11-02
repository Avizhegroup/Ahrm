namespace Avhrm.Application.Client.Features;
public class UpdateWorkReportCommand :IRequest<UpdateWorkReportVm>
{
    public int? Id { get; set; }

    public DateTime? PersianDate { get; set; }

    [StringLength(512)]
    public string? Desc { get; set; }

    [Required]
    public decimal SpentHours { get; set; }

    public int? ProjectId { get; set; }

    public int? CustomerId { get; set; }

    [Required]
    public int WorkTypeId { get; set; }

    [Required]
    public WorkDayType WorkDayType { get; set; }

    [Required]
    public WorkReportTimeOfDay WorkReportTimeOfDay { get; set; }

    public IEnumerable<int> WorkChallengesIds { get; set; }
}
