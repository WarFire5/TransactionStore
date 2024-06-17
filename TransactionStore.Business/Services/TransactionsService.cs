using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;
using TransactionStore.DataLayer.Repositories;
using ValidationException = FluentValidation.ValidationException;

namespace TransactionStore.Business.Services;

public class TransactionsService(ITransactionsRepository transactionsRepository, IMapper mapper,
    IValidator<DepositWithdrawRequest> addDepositWithdrawValidator,
    IValidator<TransferRequest> addTransferValidator) : ITransactionsService
{
    private readonly ILogger _logger = Log.ForContext<TransactionsService>();

    public async Task<Guid> AddDepositWithdrawTransactionAsync(TransactionType transactionType, DepositWithdrawRequest request)
    {
        _logger.Information("Validating the request asynchronously. / Асинхронная валидация запроса.");
        var validationResult = await addDepositWithdrawValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            _logger.Information("Mapping the request object to TransactionDto. / Преобразование объекта запроса в TransactionDto.");
            TransactionDto transaction = mapper.Map<TransactionDto>(request);

            _logger.Information("A commission provider instance is created to obtain the commission percentage. / Создается экземпляр провайдера комиссий для получения процентной ставки комиссии.");
            var commissionsProvider = new CommissionsProvider();
            var commissionPercent = commissionsProvider.GetPercentForTransaction(transactionType);

            _logger.Information("Calculating the commission amount based on the transaction amount and commission percent. / Расчет суммы комиссии на основе суммы транзакции и процентной ставки комиссии.");
            var commissionAmount = transaction.Amount * commissionPercent / 100;

            switch (transactionType)
            {
                case TransactionType.Deposit:
                    _logger.Information("Subtracting commission amount from deposit transaction amount. / Вычитание суммы комиссии из суммы депозитной транзакции.");
                    transaction.Amount -= commissionAmount;
                    break;

                case TransactionType.Withdraw:
                    _logger.Information("Adding commission amount to withdrawal transaction amount and making it negative. / Добавление суммы комиссии к сумме транзакции снятия и преобразование суммы в отрицательное значение.");
                    transaction.Amount = -(transaction.Amount - commissionAmount);
                    break;

                default:
                    _logger.Information("Throwing an error if the transaction type is not suitable. / Выдача ошибки, если тип транзакции не подходит.");
                    throw new Core.Exceptions.ValidationException("The transaction type must be deposit or withdrawal. / Тип транзакции должен быть deposit или withdraw.");
            }

            _logger.Information("Setting the transaction type. / Установка типа транзакции.");
            transaction.TransactionType = transactionType;

            _logger.Information("Adding the transaction to the repository and returning the transaction ID. / Добавление транзакции в репозиторий и возврат идентификатора транзакции.");
            return await transactionsRepository.AddDepositWithdrawTransactionAsync(transaction);
        }

        _logger.Information("Handling validation errors. / Обработка ошибок валидации.");
        string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
        throw new ValidationException(exceptions);
    }

    public async Task<TransferGuidsResponse> AddTransferTransactionAsync(TransferRequest request)
    {
        _logger.Information("Validating the transfer request asynchronously. / Асинхронная валидация запроса на перевод.");
        var validationResult = await addTransferValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            _logger.Information("Calculating the commission. / Считаем комиссию.");
            var commissionsProvider = new CommissionsProvider();
            var commissionPercent = commissionsProvider.GetPercentForTransaction(TransactionType.Transfer);
            var commissionAmount = request.Amount * commissionPercent / 100;

            _logger.Information("Creating the withdraw transaction with commission. / Создание транзакции снятия с учетом комиссии.");
            var (transferWithdraw, withdrawAmount) = CreateWithdrawTransaction(request, commissionAmount);

            _logger.Information("Creating the deposit transaction. / Создание транзакции пополнения.");
            var transferDeposit = CreateDepositTransaction(request, withdrawAmount);

            _logger.Information("Adding the transfer transaction to the repository and getting the response. / Добавление транзакции перевода в репозиторий и получение ответа.");
            var response = await transactionsRepository.AddTransferTransactionAsync(transferWithdraw, transferDeposit);
            return response;
        }
        else
        {
            _logger.Information("Throwing an error if validation fails. / Выдача ошибки, если валидация не пройдена.");
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(exceptions);
        }
    }

    public (TransactionDto, decimal) CreateWithdrawTransaction(TransferRequest request, decimal commissionAmount)
    {
        _logger.Information("Creating withdraw transaction DTO. / Создание DTO для транзакции снятия.");
        var withdrawAmount = request.Amount - commissionAmount;
        return (new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            Amount = -withdrawAmount
        }, withdrawAmount);
    }

    public TransactionDto CreateDepositTransaction(TransferRequest request, decimal withdrawAmount)
    {
        _logger.Information("Getting currency conversion rates and calculating deposit amount. / Получение курсов валют и расчет суммы депозита.");
        var currencyRatesProvider = new CurrencyRatesProvider();
        var rateToUSD = currencyRatesProvider.ConvertFirstCurrencyToUsd(request.CurrencyFromType);
        var amountUsd = withdrawAmount * rateToUSD;
        var rateFromUsd = currencyRatesProvider.ConvertUsdToSecondCurrency(request.CurrencyToType);

        _logger.Information("Creating deposit transaction DTO. / Создание DTO для транзакции пополнения.");
        return new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            Amount = amountUsd * rateFromUsd
        };
    }

    public async Task<List<TransactionWithAccountIdResponse>> GetTransactionByIdAsync(Guid id)
    {
        _logger.Information($"Getting transaction by ID {id}. / Получение транзакции по ID {id}.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionByIdAsync(id);
        return mapper.Map<List<TransactionWithAccountIdResponse>>(transactions);
    }

    public async Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id)
    {
        _logger.Information($"Getting transactions for account with ID {id}. / Получение транзакций для аккаунта с ID {id}.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionsByAccountIdAsync(id);
        return mapper.Map<List<TransactionResponse>>(transactions);
    }

    public async Task<AccountBalanceResponse> GetBalanceByAccountIdAsync(Guid id)
    {
        _logger.Information($"Getting a list of transactions for account with ID {id}. / Получение списка транзакций для аккаунта с ID {id}.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionsByAccountIdAsync(id);

        _logger.Information($"Getting balance for account with ID {id}. / Получение баланса для аккаунта с ID {id}.");
        var balance = transactions.Sum(t => t.Amount);

        _logger.Information($"For an account with ID {id} the balance was calculated - {balance}. / Для аккаунта с ID {id} рассчитан баланс - {balance}.");
        AccountBalanceResponse accountBalance = new()
        {
            AccountId = transactions[0].AccountId,
            Balance = balance
        };

        return accountBalance;
    }
}