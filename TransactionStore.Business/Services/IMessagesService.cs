using TransactionStore.Core.DTOs;

namespace TransactionStore.Business.Services;

public interface IMessagesService
{
    Task PublishTransactionAsync(List<TransactionDto> transactions, decimal comissionAmount);
}
