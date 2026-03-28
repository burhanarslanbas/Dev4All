using Dev4All.Domain.Common;
using Dev4All.Domain.Enums;
using Dev4All.Domain.Exceptions;

namespace Dev4All.Domain.Entities;

public class Contract : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public int RevisionNumber { get; private set; } = 1;
    public string LastRevisedById { get; private set; } = string.Empty;
    public ContractStatus Status { get; private set; } = ContractStatus.Draft;
    public bool IsCustomerApproved { get; private set; }
    public bool IsDeveloperApproved { get; private set; }
    public DateTime? CustomerApprovedAt { get; private set; }
    public DateTime? DeveloperApprovedAt { get; private set; }

    public Project Project { get; set; } = null!;
    public ICollection<ContractRevision> Revisions { get; set; } = [];

    public void SetProjectId(Guid projectId)
    {
        if (projectId == Guid.Empty)
            throw new BusinessRuleViolationException("Project ID cannot be empty.");

        ProjectId = projectId;
    }

    /// <summary>Revises the contract content; resets the other party's approval.</summary>
    public void Revise(string newContent, string revisedById, bool isCustomer)
    {
        if (Status is ContractStatus.BothApproved or ContractStatus.Cancelled)
            throw new BusinessRuleViolationException("Approved or cancelled contracts cannot be revised.");

        if (string.IsNullOrWhiteSpace(newContent))
            throw new BusinessRuleViolationException("Contract content cannot be empty.");

        Content = newContent;
        RevisionNumber++;
        LastRevisedById = revisedById;
        Status = ContractStatus.UnderReview;

        if (isCustomer)
            IsDeveloperApproved = false;
        else
            IsCustomerApproved = false;

        MarkAsUpdated();
    }

    /// <summary>Approves the contract for the given party. If both approve, status becomes BothApproved.</summary>
    public void Approve(bool isCustomer)
    {
        if (Status is ContractStatus.BothApproved or ContractStatus.Cancelled)
            throw new BusinessRuleViolationException("Approved or cancelled contracts cannot be approved again.");

        if (isCustomer)
        {
            IsCustomerApproved = true;
            CustomerApprovedAt = DateTime.UtcNow;
        }
        else
        {
            IsDeveloperApproved = true;
            DeveloperApprovedAt = DateTime.UtcNow;
        }

        if (IsCustomerApproved && IsDeveloperApproved)
            Status = ContractStatus.BothApproved;

        MarkAsUpdated();
    }

    public void Cancel()
    {
        if (Status is ContractStatus.BothApproved or ContractStatus.Cancelled)
            throw new BusinessRuleViolationException("Approved or already cancelled contracts cannot be cancelled.");

        Status = ContractStatus.Cancelled;
        MarkAsUpdated();
    }

    public void SetInitialContent(string content, string createdById)
    {
        Content = content;
        LastRevisedById = createdById;
    }
}
