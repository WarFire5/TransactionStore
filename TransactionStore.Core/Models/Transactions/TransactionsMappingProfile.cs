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
        //CreateMap<TransferRequest, TransactionDto>()
        //    .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountFromId))
        //    .ForMember(dest => dest.CurrencyType, opt => opt.MapFrom(src => src.CurrencyFromType))
        //    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
        //    .ForMember(dest => dest.TransactionType, opt => opt.Ignore())
        //    .ForMember(dest => dest.Id, opt => opt.Ignore())
        //    .ReverseMap();
        //CreateMap<TransferRequest, TransactionDto>()
        //    .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountToId))
        //    .ForMember(dest => dest.CurrencyType, opt => opt.MapFrom(src => src.CurrencyToType))
        //    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
        //    .ForMember(dest => dest.TransactionType, opt => opt.Ignore())
        //    .ForMember(dest => dest.Id, opt => opt.Ignore())
        //    .ReverseMap();

        CreateMap<TransactionDto, AccountBalanceResponse>()
            .ForMember(d => d.Balance, opt => opt.MapFrom(s =>s.Amount));
        CreateMap<TransactionDto, TransactionResponse>();
    }
}