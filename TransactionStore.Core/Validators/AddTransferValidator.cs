﻿using FluentValidation;
using System.Text.RegularExpressions;
using TransactionStore.Core.Models.Transactions.Requests;

namespace Backend.Core.Validators;

public class AddTransferValidator : AbstractValidator<TransferRequest>
{
    public AddTransferValidator()
    {
        RuleFor(t => t.AccountFromId)
            .NotEmpty().NotNull().WithMessage("GUID не должен быть значением по умолчанию (default).")
            .Must(guid => guid.ToString().Length >= 36 && guid.ToString().Length <= 38)
            .WithMessage("Длина GUID должна быть не менее 36 и не более 38 символов.")
            // Проверяем формат GUID с помощью регулярного выражения
            .Must(guid => Regex.IsMatch(guid.ToString(), @"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$"))
            .WithMessage("Некорректный формат GUID. GUID должен быть в формате XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, " +
                         "где X — это шестнадцатеричный символ. Допустимы фигурные скобки в начале и конце строки.");

        RuleFor(t => t.AccountToId)
            .NotEmpty().NotNull().WithMessage("GUID не должен быть значением по умолчанию (default).")
            .Must(guid => guid.ToString().Length >= 36 && guid.ToString().Length <= 38)
            .WithMessage("Длина GUID должна быть не менее 36 и не более 38 символов.")
            // Проверяем формат GUID с помощью регулярного выражения
            .Must(guid => Regex.IsMatch(guid.ToString(), @"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$"))
            .WithMessage("Некорректный формат GUID. GUID должен быть в формате 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX', " +
                         "где X — это шестнадцатеричный символ. Допустимы фигурные скобки в начале и конце строки.");

        //RuleFor(t => t.TransactionType)
        //    .NotEmpty().WithMessage("Поле не может быть пустым. Укажите тип транзакции.")
        //    .NotNull().WithMessage("Поле не может быть null. Укажите тип транзакции.");

        RuleFor(t => t.CurrencyFromType)
            .NotEmpty().WithMessage("Поле не может быть пустым. Укажите тип валюты.")
            .NotNull().WithMessage("Поле не может быть null. Укажите тип валюты.");

        RuleFor(t => t.CurrencyToType)
            .NotEmpty().WithMessage("Поле не может быть пустым. Укажите тип валюты.")
            .NotNull().WithMessage("Поле не может быть null. Укажите тип валюты.");

        RuleFor(t => t.AmountFrom)
            .NotEmpty().WithMessage("Поле не может быть пустым. Введите сумму операции.")
            .NotNull().WithMessage("Поле не может быть null. Введите сумму операции.")
            //.NotEqual(0).WithMessage("Сумма операции не может быть равной нулю.")
            .LessThan(1).WithMessage("Нельзя снять меньше 100.")
            .GreaterThanOrEqualTo(1000000).WithMessage("Превышен максимальный лимит на одну операцию.");

        RuleFor(t => t.AmountTo)
            .NotEmpty().WithMessage("Поле не может быть пустым. Введите сумму операции.")
            .NotNull().WithMessage("Поле не может быть null. Введите сумму операции.")
            //.NotEqual(0).WithMessage("Сумма операции не может быть равной нулю.")
            .LessThan(1).WithMessage("Нельзя внести меньше 1.")
            .GreaterThanOrEqualTo(1000000).WithMessage("Превышен максимальный лимит на одну операцию.");

        RuleFor(t => t.Date)
            .NotEmpty().WithMessage("Поле не может быть пустым. Введите дату и время операции.")
            .NotNull().WithMessage("Поле не может быть null. Введите дату и время операции.")
            .Must(date => date != default(DateTime)).WithMessage("Дата и время операции не должны быть значением по умолчанию.")
            .Must(date => date >= new DateTime(2024, 5, 21)).WithMessage("Дата операции не может быть раньше 2024-05-21.")
            .Must(date => date <= DateTime.Now).WithMessage("Дата операции не может быть позже текущей даты и времени.")
            // Проверяем формат DateTime с помощью регулярного выражения
            .Must(date => Regex.IsMatch(date.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), @"^\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.\d{6}$"))
            .WithMessage("Дата и время операции должны быть в формате 'yyyy-MM-dd HH:mm:ss.ffffff'.");
    }
}