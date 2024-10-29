using Avhrm.Infrastructure.Client;

namespace Avhrm.UI.Shared.Pages.WorkingReport;
public partial class Child
{
    public bool IsMessageShown = false;
    public bool IsLoading = false;
    public List<string> MessageTexts = new();
    public List<GetChildUsersDto> ChildUsers = new();
    public GetWorkReportOfChildUsersQuery Request = new();
    public List<GetWorkReportOfChildUsersDto> Reports;
    public Severity AlertSeverity = Severity.Error;

    [Inject] public ApiHandler Api { get; set; }

    [CascadingParameter] public ComponentsContext Context { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Context.IsBackButtonShown = true;

        IsLoading = true;

        ChildUsers = (await Api.SendJsonAsync<GetChildUsersVm>(HttpMethod.Get
            , "Account/GetChildUsers"
            )).Value.Data;

        Request.Date = DateTime.Now;

        IsLoading = false;
    }

    public async Task OnInvalidSubmit(EditContext context)
    {
        MessageTexts.Clear();

        foreach (var valid in context.GetValidationMessages())
        {
            MessageTexts.Add(valid);
        }

        IsMessageShown = true;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        Reports = (await Api.SendJsonAsync<GetWorkReportOfChildUsersVm>(HttpMethod.Get
            , "WorkReport/GetByChildDate"
            , Request)).Value.Data;

        IsLoading = false;
    }
}
