namespace TransactionStore.Core.Data;

public interface ICommissionsProvider
{
    decimal GetPercentForTransaction(Enum transactionType);
}