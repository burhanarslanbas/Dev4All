namespace Dev4All.Domain.Enums;

public enum ProjectStatus
{
    Open = 0,
    AwaitingContract = 1,   // Teklif kabul edildi — sözleşme onayı bekleniyor
    Ongoing = 2,
    Completed = 3,
    Expired = 4,
    Cancelled = 5           // Sözleşme sürecinde bir taraf iptal etti
}
