namespace TransactionStore.Core.Data;

public class CommissionsProvider : ICommissionsProvider
{
    private readonly Dictionary<string, decimal> _percent;

    public CommissionsProvider()
    {
        _percent = new Dictionary<string, decimal>()
            {
                { "DEPOSIT", 5m },
                { "WITHDRAW", 10m },
                { "TRANSFER", 7.5m }
            };
    }

    private static string ConvertTransactionEnumToString(Enum transactionType)
    {
        return transactionType.ToString().ToUpper();
    }

    public decimal GetPercentForTransaction(Enum transactionType)
    {
        var transaction = ConvertTransactionEnumToString(transactionType);
        if (_percent.TryGetValue(transaction, out var percent))
        {
            return percent;
        }
        throw new ArgumentException($"The commission percentage for transaction of type {transaction} not found. / Процент комиссии для транзакции типа {transaction} не найден.");
    }
}