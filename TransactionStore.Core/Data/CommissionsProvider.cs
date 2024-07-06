using Serilog;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Settings;

namespace TransactionStore.Core.Data;

public class CommissionsProvider : ICommissionsProvider
{
    private Dictionary<string, decimal> _percent;
    private readonly ILogger _logger = Log.ForContext<CommissionsProvider>();

    public CommissionsProvider(ComissionSettings comission)
    {
        _percent = new Dictionary<string, decimal>()
        {
            { TransactionType.Deposit.ToString().ToUpper(), decimal.Parse(comission.Deposit) },
            { TransactionType.Withdraw.ToString().ToUpper(), decimal.Parse(comission.Withdraw) },
            { TransactionType.Transfer.ToString().ToUpper(), decimal.Parse(comission.Transfer) }
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
            _logger.Information($"Returning percent of commission за {transactionType} - {percent}. / Возврат процента комиссии за {transactionType} – {percent}");
            return percent;
        }
        _logger.Error($"Throwing an error if commission percentage for transaction of type {transaction} not found. / Выдача ошибки, если процент комиссии для транзакции типа {transaction} не найден.");
        throw new ArgumentException($"The commission percentage for transaction of type {transaction} not found. / Процент комиссии для транзакции типа {transaction} не найден.");
    }
}