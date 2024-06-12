namespace TransactionStore.Core.Models.Responses;

public class TransferGuidsResponse
{
    public Guid TransferWithdrawId { get; set; }
    public Guid TransferDepositId { get; set; }
}