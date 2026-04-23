using Dev4All.Application.Abstractions.Services;
using Dev4All.Domain.Enums;
using Dev4All.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dev4All.IntegrationTests.Infrastructure;

/// <summary>
/// Custom <see cref="WebApplicationFactory{TEntryPoint}"/> that boots the real WebAPI pipeline
/// but swaps the PostgreSQL <see cref="Dev4AllDbContext"/> for EF Core InMemory, disables the
/// Quartz email dispatch job, and replaces the SMTP <see cref="IEmailService"/> with a no-op.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"Dev4AllIntegrationTests-{Guid.NewGuid()}";
    private readonly InMemoryDatabaseRoot _databaseRoot = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = "Host=inmemory;Database=dev4all;Username=dev4all;Password=dev4all",
                ["Database:MaxRetryCount"] = "0",
                ["Jwt:Issuer"] = "Dev4AllTests",
                ["Jwt:Audience"] = "Dev4AllTestUsers",
                ["Jwt:SecretKey"] = "integration-tests-super-secret-signing-key-32+chars",
                ["Jwt:ExpiryInMinutes"] = "60",
                ["Smtp:Host"] = "localhost",
                ["Smtp:Port"] = "2525",
                ["Smtp:SenderEmail"] = "noreply@dev4all.test",
                ["Smtp:UseSsl"] = "false",
                ["Auth:RequireConfirmedEmail"] = "false",
                ["Auth:RefreshTokenLifetimeInDays"] = "7",
                ["EmailDispatch:Enabled"] = "false",
                ["Seed:SeedRolesOnStartup"] = "false",
            });
        });

        builder.ConfigureServices(services =>
        {
            RemoveAll<DbContextOptions<Dev4AllDbContext>>(services);
            RemoveAll<DbContextOptions>(services);
            RemoveAll<Dev4AllDbContext>(services);
            RemoveAll<IDbContextOptionsConfiguration<Dev4AllDbContext>>(services);
            RemoveAll<IDbContextFactory<Dev4AllDbContext>>(services);

            services.AddDbContext<Dev4AllDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName, _databaseRoot);
            });

            RemoveAll<IEmailService>(services);
            services.AddSingleton<IEmailService, NoOpEmailService>();
        });
    }

    /// <summary>Seeds baseline application roles required by the auth flow.</summary>
    public async Task SeedRolesAsync()
    {
        using var scope = Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Enum.GetNames<UserRole>())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static void RemoveAll<TService>(IServiceCollection services)
    {
        for (var i = services.Count - 1; i >= 0; i--)
        {
            if (services[i].ServiceType == typeof(TService))
            {
                services.RemoveAt(i);
            }
        }
    }
}
