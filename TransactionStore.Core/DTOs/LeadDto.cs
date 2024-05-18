namespace TransactionStore.Core.DTOs;

public class LeadDto : IdContainer
{
    public List<AccountDto> Accounts { get; set; }
}