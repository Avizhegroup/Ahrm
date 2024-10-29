namespace Avhrm.Application.Client.Features;
public class GetWorkReportOfChildUsersQuery : IRequest<GetWorkReportOfChildUsersVm>
{
    [Required]
    public DateTime? Date { get; set; }

    [Required]
    public string UserId { get; set; }
}
