using Dev4All.Domain.Common;
using Dev4All.Domain.Exceptions;

namespace Dev4All.Domain.Entities;

public class ContractRevision : BaseEntity
{
    public Guid ContractId { get; private set; }
    public string RevisedById { get; private set; } = string.Empty;
    public string ContentSnapshot { get; private set; } = string.Empty;
    public int RevisionNumber { get; private set; }
    public string? RevisionNote { get; private set; }

    public Contract Contract { get; set; } = null!;

    public static ContractRevision CreateSnapshot(
        Guid contractId, string revisedById, string contentSnapshot, int revisionNumber, string? note = null)
    {
        if (contractId == Guid.Empty)
            throw new BusinessRuleViolationException("Contract ID cannot be empty.");

        return new ContractRevision
        {
            ContractId = contractId,
            RevisedById = revisedById,
            ContentSnapshot = contentSnapshot,
            RevisionNumber = revisionNumber,
            RevisionNote = note
        };
    }
}
