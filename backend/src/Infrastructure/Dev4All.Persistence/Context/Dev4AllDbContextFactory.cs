using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dev4All.Persistence.Context;

/// <summary>
/// Design-time DbContext factory for EF Core tooling.
/// Reads connection string from environment variables first, then WebAPI appsettings and user-secrets.
/// </summary>
public sealed class Dev4AllDbContextFactory : IDesignTimeDbContextFactory<Dev4AllDbContext>
{
    /// <inheritdoc />
    public Dev4AllDbContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<Dev4AllDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new Dev4AllDbContext(optionsBuilder.Options);
    }

    private static string GetConnectionString()
    {
        var connectionFromEnv = Environment.GetEnvironmentVariable("DATABASE__CONNECTIONSTRING")
            ?? Environment.GetEnvironmentVariable("Database__ConnectionString");

        if (!string.IsNullOrWhiteSpace(connectionFromEnv))
        {
            return connectionFromEnv;
        }

        var webApiProjectPath = ResolveWebApiProjectPath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(webApiProjectPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddJsonFile(GetUserSecretsFilePath(), optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionFromConfig = configuration["Database:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(connectionFromConfig))
        {
            return connectionFromConfig;
        }

        throw new InvalidOperationException(
            "No database connection string found for EF Core design-time operations. " +
            "Set DATABASE__CONNECTIONSTRING or configure Database:ConnectionString in WebAPI user-secrets.");
    }

    private static string GetUserSecretsFilePath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataPath, "Microsoft", "UserSecrets", "f1209ca4-78ce-429c-bbee-0950cfe7aacd", "secrets.json");
    }

    private static string ResolveWebApiProjectPath()
    {
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "backend", "src", "Presentation", "Dev4All.WebAPI"),
            Path.Combine(Directory.GetCurrentDirectory(), "src", "Presentation", "Dev4All.WebAPI"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "src", "Presentation", "Dev4All.WebAPI")),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Presentation", "Dev4All.WebAPI"))
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(Path.Combine(candidate, "appsettings.json")))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Unable to resolve WebAPI project path for design-time configuration loading.");
    }
}
