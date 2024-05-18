using TransactionStore.Core.DTOs;

namespace TransactionStore.Core.Models.Leads.Responses;

public class LeadResponse
{
    public List<AccountDto> Accounts { get; set; }
}