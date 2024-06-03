using FluentValidation;
using TransactionStore.Core.Models.Requests;

namespace Backend.Core.Validators;

public class AddDepositWithdrawValidator : AbstractValidator<DepositWithdrawRequest>
{
    public AddDepositWithdrawValidator()
    {
        RuleFor(t => t.AccountId)
            .NotEmpty().WithMessage("Введите корректный GUID.");

        RuleFor(t => t.CurrencyType)
            .NotEmpty().WithMessage("Поле не может быть пустым. Укажите тип валюты.")
            .NotNull().WithMessage("Поле не может быть null. Укажите тип валюты.");

        RuleFor(t => t.Amount)
            .GreaterThanOrEqualTo(1).WithMessage("Сумма не должна быть меньше 1.")
            .LessThanOrEqualTo(1000000).WithMessage("Превышен максимальный лимит на операцию.");
    }
}