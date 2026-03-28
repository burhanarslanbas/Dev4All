namespace Dev4All.Domain.Enums;

public enum ContractStatus
{
    Draft = 0,          // Sistem taslağı oluşturdu
    UnderReview = 1,    // En az bir taraf revize etti
    BothApproved = 2,   // İki taraf da onayladı → Ongoing'e geçiş
    Cancelled = 3       // Bir taraf iptal etti → proje Cancelled
}
