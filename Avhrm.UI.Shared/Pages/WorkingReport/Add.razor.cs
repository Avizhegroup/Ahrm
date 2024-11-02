﻿using AutoMapper;
using Avhrm.Identity.UI.Services;
using Avhrm.Infrastructure.Client;
using Microsoft.AspNetCore.Components.Web;

namespace Avhrm.UI.Shared.Pages.WorkingReport;
public partial class Add
{
    public bool IsMessageShown = false;
    public bool IsLoading = false;
    public List<GetAllWorkTypesDto> WorkTypes = new();
    public List<GetAllProjectsDto> Projects = new();
    public List<GetAllCustomersDto> Customers = new();
    public List<GetAllWorkChallengeDto> WorkChallenges = new();
    public InsertWorkReportCommand Command = new();
    public List<string> MessageTexts = new();
    public List<DropDownDualItem<WorkReportTimeOfDay>> TimeOfDays = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Morning,
            Value = WorkReportTimeOfDay.Morning
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Noon,
            Value = WorkReportTimeOfDay.Noon
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Afternoon,
            Value = WorkReportTimeOfDay.Afternoon
        }
    };
    public Severity AlertSeverity = Severity.Error;

    [Parameter] public int? Id { get; set; }

    [Inject] public ApiHandler Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public AvhrmClientAuthenticationStateProvider AuthState { get; set; }

    [CascadingParameter] public ComponentsContext Context { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Context.IsBackButtonShown = true;

        IsLoading = true;

        var user = (await AuthState.GetAuthenticationStateAsync()).User;

        WorkTypes = (await Api.SendJsonAsync<GetAllWorkTypesVm>(HttpMethod.Get, "WorkType/GetAll")).Value.Data;

        Projects = (await Api.SendJsonAsync<GetAllProjectsVm>(HttpMethod.Get, "Project/GetAll")).Value.Data;

        Customers = (await Api.SendJsonAsync<GetAllCustomersVm>(HttpMethod.Get, "Customer/GetAll")).Value.Data;

        WorkChallenges = (await Api.SendJsonAsync<GetAllWorkChallengeVm>(HttpMethod.Get, "WorkChallenge/GetAll"))
                                   .Value
                                   .Data
                                   .Where(p=> p.DepartmentId == int.Parse(user.GetDepartmentId()))
                                   .ToList();

        Command.PersianDate = DateTime.Now.Date;

        if (Id is not null)
        {
            GetWorkReportByIdVm? data = (await Api.SendJsonAsync<GetWorkReportByIdVm>(HttpMethod.Get
                , "WorkReport/Get"
                , new GetWorkReportByIdQuery()
                {
                    Id = Id.Value
                })).Value;

            if (data is not null)
            {
                Command = Mapper.Map<InsertWorkReportCommand>(data);
            }
        }
        else
        {
            Command.PersianDate = DateTime.Now.Date;
        }

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        bool result;

        IsLoading = true;

        if (Id is not null)
        {
            var data = Mapper.Map<UpdateWorkReportCommand>(Command);

            data.Id = Id.Value;

            result = (await Api.SendJsonAsync<UpdateWorkReportVm>(HttpMethod.Put
                , "WorkReport/Update"
                , data)).Value.Result;
        }
        else
        {
            result = (await Api.SendJsonAsync<InsertWorkReportVm>(HttpMethod.Post
                , "WorkReport/Insert"
                , Command)).Value.Result;
        }

        if (result)
        {
            AlertSeverity = Severity.Success;

            MessageTexts = new()
            {
                TextResources.APP_StringKeys_Message_Success
            };
        }
        else
        {
            AlertSeverity = Severity.Error;

            MessageTexts = new()
            {
                TextResources.APP_StringKeys_Message_Failed
            };
        }

        IsLoading = false;

        IsMessageShown = true;
    }

    public async Task OnInvalidSubmit(EditContext context)
    {
        MessageTexts.Clear();

        foreach (var valid in context.GetValidationMessages())
        {
            MessageTexts.Add(valid);
        }

        AlertSeverity = Severity.Error;

        IsMessageShown = true;
    }

    public async Task OnDeleteClick(MouseEventArgs e)
    {
        bool result = (await Api.SendJsonAsync<DeleteWorkReportVm>(HttpMethod.Delete
            , "WorkReport/Delete"
            , new DeleteWorkReportCommand()
            {
                Id = Id.Value
            })).Value.Result;

        if (result)
        {
            NavigationManager.NavigateTo("/workingreport/search");

            return;
        }
    }
}
