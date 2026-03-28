using Dev4All.Domain.Common;
using Dev4All.Domain.Enums;
using Dev4All.Domain.Exceptions;

namespace Dev4All.Domain.Entities;

public class Bid : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public string DeveloperId { get; private set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public string ProposalNote { get; set; } = string.Empty;
    public BidStatus Status { get; private set; } = BidStatus.Pending;
    public bool IsAccepted { get; private set; }

    public Project Project { get; set; } = null!;

    public void SetOwnership(Guid projectId, string developerId)
    {
        if (projectId == Guid.Empty)
            throw new BusinessRuleViolationException("Project ID cannot be empty.");
        if (string.IsNullOrWhiteSpace(developerId))
            throw new BusinessRuleViolationException("Developer ID cannot be empty.");

        ProjectId = projectId;
        DeveloperId = developerId;
    }

    public void Accept()
    {
        if (Status != BidStatus.Pending)
            throw new BusinessRuleViolationException("Only pending bids can be accepted.");

        Status = BidStatus.Accepted;
        IsAccepted = true;
        MarkAsUpdated();
    }

    public void Reject()
    {
        if (Status != BidStatus.Pending)
            throw new BusinessRuleViolationException("Only pending bids can be rejected.");

        Status = BidStatus.Rejected;
        MarkAsUpdated();
    }
}
