using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Options;
using Dev4All.Infrastructure.Auth;
using Dev4All.Infrastructure.Email;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Security.Claims;
using System.Text;

namespace Dev4All.Infrastructure;

/// <summary>Registers Infrastructure-layer services (JWT, Email, CurrentUser, Quartz).</summary>
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));
        services.Configure<EmailDispatchOptions>(configuration.GetSection(EmailDispatchOptions.SectionName));
        services.Configure<RefreshTokenCleanupOptions>(configuration.GetSection(RefreshTokenCleanupOptions.SectionName));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((jwtBearer, jwtAccessor) =>
            {
                var jwtOptions = jwtAccessor.Value;
                jwtBearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

        services.AddHttpContextAccessor();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        services.AddSingleton<TemplateRenderer>();

        services.AddQuartz(q =>
        {
            var emailDispatchOptions = configuration.GetSection(EmailDispatchOptions.SectionName)
                .Get<EmailDispatchOptions>() ?? new EmailDispatchOptions();

            if (emailDispatchOptions.Enabled)
            {
                var jobKey = new JobKey("EmailDispatchJob");
                q.AddJob<EmailDispatchJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("EmailDispatchJob-trigger")
                    .WithCronSchedule(emailDispatchOptions.CronExpression));
            }

            var cleanupOptions = configuration.GetSection(RefreshTokenCleanupOptions.SectionName)
                .Get<RefreshTokenCleanupOptions>() ?? new RefreshTokenCleanupOptions();

            if (cleanupOptions.Enabled)
            {
                var cleanupKey = new JobKey("RefreshTokenCleanupJob");
                q.AddJob<RefreshTokenCleanupJob>(opts => opts.WithIdentity(cleanupKey));
                q.AddTrigger(opts => opts
                    .ForJob(cleanupKey)
                    .WithIdentity("RefreshTokenCleanupJob-trigger")
                    .WithCronSchedule(cleanupOptions.CronExpression));
            }
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
