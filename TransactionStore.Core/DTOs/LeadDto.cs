namespace TransactionStore.Core.DTOs;

public class LeadDto : IdContainer
{
    public bool Status { get; set; }
    public List<AccountDto> Accounts { get; set; }
}