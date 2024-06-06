using FluentValidation;
using TransactionStore.Core.Models.Requests;

namespace Backend.Core.Validators;

public class AddDepositWithdrawValidator : AbstractValidator<DepositWithdrawRequest>
{
    public AddDepositWithdrawValidator()
    {
        RuleFor(t => t.AccountId)
            .NotEmpty().WithMessage("Enter a valid GUID. / Введите корректный GUID.");

        RuleFor(t => t.CurrencyType)
            .NotEmpty().WithMessage("The field cannot be empty. Specify the currency type. / Поле не может быть пустым. Укажите тип валюты.")
            .NotNull().WithMessage("The field cannot be null. Specify the currency type. / Поле не может быть null. Укажите тип валюты.");

        RuleFor(t => t.Amount)
            .GreaterThanOrEqualTo(1).WithMessage("The amount should not be less than 1. / Сумма не должна быть меньше 1.")
            .LessThanOrEqualTo(1000000).WithMessage("The maximum limit for the operation has been exceeded. / Превышен максимальный лимит на операцию.");
    }
}