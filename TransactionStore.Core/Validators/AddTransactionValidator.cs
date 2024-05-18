using TransactionStore.Core.Models.Transactions.Requests;
using FluentValidation;

namespace Backend.Core.Validators;

public class AddTransactionValidator : AbstractValidator<AddTransactionRequest>
{
    public AddTransactionValidator()
    {
    }
}