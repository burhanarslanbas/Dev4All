using Dev4All.Domain.Common;
using Dev4All.Domain.Enums;
using Dev4All.Domain.Exceptions;

namespace Dev4All.Domain.Entities;

public class Project : BaseEntity
{
    public string CustomerId { get; private set; } = string.Empty;
    public string? AssignedDeveloperId { get; private set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime BidEndDate { get; set; }
    public string? Technologies { get; set; }
    public ProjectStatus Status { get; private set; } = ProjectStatus.Open;

    public ICollection<Bid> Bids { get; set; } = [];
    public ICollection<GitHubLog> GitHubLogs { get; set; } = [];
    public Contract? Contract { get; set; }

    public void SetCustomer(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new BusinessRuleViolationException("Customer ID cannot be empty.");

        CustomerId = customerId;
    }

    public void AssignDeveloper(string developerId)
    {
        if (string.IsNullOrWhiteSpace(developerId))
            throw new BusinessRuleViolationException("Developer ID cannot be empty.");

        AssignedDeveloperId = developerId;
        MarkAsUpdated();
    }

    public void MoveToAwaitingContract()
    {
        if (Status != ProjectStatus.Open)
            throw new BusinessRuleViolationException("Only open projects can move to awaiting contract.");

        Status = ProjectStatus.AwaitingContract;
        MarkAsUpdated();
    }

    public void MoveToOngoing()
    {
        if (Status != ProjectStatus.AwaitingContract)
            throw new BusinessRuleViolationException("Only projects awaiting contract can move to ongoing.");

        Status = ProjectStatus.Ongoing;
        MarkAsUpdated();
    }

    public void Complete()
    {
        if (Status != ProjectStatus.Ongoing)
            throw new BusinessRuleViolationException("Only ongoing projects can be completed.");

        Status = ProjectStatus.Completed;
        MarkAsUpdated();
    }

    public void Expire()
    {
        if (Status != ProjectStatus.Open)
            throw new BusinessRuleViolationException("Only open projects can expire.");

        Status = ProjectStatus.Expired;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        if (Status is ProjectStatus.Completed or ProjectStatus.Expired or ProjectStatus.Cancelled)
            throw new BusinessRuleViolationException("Completed, expired, or already cancelled projects cannot be cancelled.");

        Status = ProjectStatus.Cancelled;
        MarkAsUpdated();
    }
}
