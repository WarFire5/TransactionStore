using TransactionStore.Core.DTOs;

namespace TransactionStore.Core.Models.Leads.Requests;

public class LeadRequest
{
    public List<AccountDto> Accounts { get; set; }
}