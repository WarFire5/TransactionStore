using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Responses;

public class TransactionWithAccountIdResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}