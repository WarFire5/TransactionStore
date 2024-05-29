﻿using AutoMapper;

namespace TransactionStore.Core.Models.Transactions;

public class TransactionsMappingProfile : Profile
{
    public TransactionsMappingProfile()
    {
        CreateMap<TransactionDto, TransactionsByAccountIdResponse>();
        CreateMap<TransactionDto, TransactionsByLeadIdResponse>();
    }
}