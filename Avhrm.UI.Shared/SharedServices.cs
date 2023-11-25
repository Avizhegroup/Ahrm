﻿using Avhrm.Core.Contracts;
using Avhrm.Identity;
using Avhrm.Identity.Contracts;
using Avhrm.UI.Shared.Extensions;
using Avhrm.UI.Shared.Services;
using Microsoft.Extensions.Configuration;

namespace Avhrm.UI.Shared;

public static class SharedServices
{
    public static void AddSharedServices(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddScoped(sp =>
        {
            HttpClient httpClient = new(sp.GetRequiredService<AvhrmHttpClient>());

            return httpClient;
        });

        services.AddScoped<AvhrmHttpClient>();

        services.AddGrpcService<IVacationRequest>(configuration);

        services.AddGrpcService<IAuthenticationService>(configuration);

        services.AddGrpcService<IWorkTypeService>(configuration);

        services.AddIdentityUIServices();
    }
}
