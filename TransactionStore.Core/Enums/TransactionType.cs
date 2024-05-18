namespace TransactionStore.Core.Enums;

public enum TransactionType
{
    Unknown = 0,
    Deposit = 1,
    Withdraw = 2,
    TransferWithdraw = 3,
    TransferDeposit = 4,
    Transfer = 5
}