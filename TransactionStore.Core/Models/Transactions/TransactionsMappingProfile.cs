using AutoMapper;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.Core.Models.Transactions;

public class TransactionsMappingProfile : Profile
{
    public TransactionsMappingProfile()
    {
        CreateMap<TransactionDto, TransactionsByAccountIdResponse>();
        CreateMap<TransactionDto, TransactionsByLeadIdResponse>();
    }
}