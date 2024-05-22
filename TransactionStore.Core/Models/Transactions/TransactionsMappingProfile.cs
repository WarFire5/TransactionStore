using AutoMapper;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.Core.Models.Transactions;

public class TransactionsMappingProfile : Profile
{
    public TransactionsMappingProfile()
    {
        CreateMap<DepositWithdrawRequest, TransactionDto>();
        CreateMap<TransferRequest, TransactionDto>();

        CreateMap<TransactionDto, TransactionResponse>();
    }
}