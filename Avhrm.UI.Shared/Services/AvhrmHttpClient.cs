﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Net;

namespace Avhrm.UI.Shared.Services;

public class AvhrmHttpClient: HttpClientHandler
{
    private readonly IJSRuntime jsRuntime;
    private readonly NavigationManager navigationManager;

    public AvhrmHttpClient(IJSRuntime jsRuntime
        , NavigationManager navigationManager)
    {
        this.jsRuntime = jsRuntime;
        this.navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token;
        
#if BlazorServer
        token = await jsRuntime.InvokeAsync<string?>("window.localStorage.getItem", "jwt");
#else
        token = Preferences.Get("access_token", null);
#endif

        if (token.HasValue())
        {
            request.Headers.Authorization = new("Bearer", token);
        }
        else
        {
            request.Headers.Remove("Authorization");
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
#if BlazorServer
                    token = await jsRuntime.InvokeAsync<string?>("window.localStorage.getItem", "jwt");
#else
                    token = Preferences.Remove("access_token");
#endif
                    await jsRuntime.InvokeVoidAsync("window.localStorage.removeItem", "jwt");

                    navigationManager.NavigateTo("/account/login", true);
                }
            }
        }

        return response;
    }
}