using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.Bids;
using Dev4All.Application.Abstractions.Persistence.Repositories.ContractRevisions;
using Dev4All.Application.Abstractions.Persistence.Repositories.Contracts;
using Dev4All.Application.Abstractions.Persistence.Repositories.GitHubLogs;
using Dev4All.Application.Abstractions.Persistence.Repositories.Projects;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Persistence.Repositories.Bids;
using Dev4All.Persistence.Repositories.ContractRevisions;
using Dev4All.Persistence.Repositories.Contracts;
using Dev4All.Persistence.Repositories.GitHubLogs;
using Dev4All.Persistence.Repositories.Projects;
using Dev4All.Persistence.Repositories.RefreshTokens;
using Microsoft.Extensions.DependencyInjection;

namespace Dev4All.Persistence;

/// <summary>Registers Persistence-layer services.</summary>
public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
        services.AddScoped<IProjectWriteRepository, ProjectWriteRepository>();

        services.AddScoped<IBidReadRepository, BidReadRepository>();
        services.AddScoped<IBidWriteRepository, BidWriteRepository>();

        services.AddScoped<IGitHubLogReadRepository, GitHubLogReadRepository>();
        services.AddScoped<IGitHubLogWriteRepository, GitHubLogWriteRepository>();

        services.AddScoped<IRefreshTokenReadRepository, RefreshTokenReadRepository>();
        services.AddScoped<IRefreshTokenWriteRepository, RefreshTokenWriteRepository>();

        services.AddScoped<IContractReadRepository, ContractReadRepository>();
        services.AddScoped<IContractWriteRepository, ContractWriteRepository>();

        services.AddScoped<IContractRevisionReadRepository, ContractRevisionReadRepository>();
        services.AddScoped<IContractRevisionWriteRepository, ContractRevisionWriteRepository>();

        return services;
    }
}
