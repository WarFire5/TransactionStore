using FluentValidation;
using TransactionStore.Core.Models.Transactions.Requests;

namespace Backend.Core.Validators;

public class AddTransferValidator : AbstractValidator<TransferRequest>
{
    public AddTransferValidator()
    {
        RuleFor(t => t.AccountFromId)
            .NotEmpty().WithMessage("Введите корректный GUID.");

        RuleFor(t => t.AccountToId)
            .NotEmpty().WithMessage("Введите корректный GUID.");

        RuleFor(t => t.CurrencyFromType)
            .NotEmpty().WithMessage("Поле не может быть пустым. Укажите тип валюты.")
            .NotNull().WithMessage("Поле не может быть null. Укажите тип валюты.");

        RuleFor(t => t.CurrencyToType)
            .NotEmpty().WithMessage("Поле не может быть пустым. Укажите тип валюты.")
            .NotNull().WithMessage("Поле не может быть null. Укажите тип валюты.");

        RuleFor(t => t.Amount)
            .GreaterThanOrEqualTo(1).WithMessage("Сумма не должна быть меньше 1.")
            .LessThanOrEqualTo(1000000).WithMessage("Превышен максимальный лимит на операцию.");
    }
}