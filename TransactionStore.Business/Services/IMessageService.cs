using TransactionStore.Core.Models.Requests;

namespace TransactionStore.Business.Services;

public interface IMessageService
{
    Task PublishDepositWithdrawTransactionAsync(DepositWithdrawRequest request);
}
