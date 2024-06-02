using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TransactionStore.Business.Services;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.Business.Tests
{
    public class TransactionsServiceTests
    {
        private readonly Mock<ITransactionsRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<DepositWithdrawRequest>> _depositWithdrawValidatorMock;
        private readonly Mock<IValidator<TransferRequest>> _transferValidatorMock;
        private readonly TransactionsService _service;

        public TransactionsServiceTests()
        {
            _repositoryMock = new Mock<ITransactionsRepository>();
            _mapperMock = new Mock<IMapper>();
            _depositWithdrawValidatorMock = new Mock<IValidator<DepositWithdrawRequest>>();
            _transferValidatorMock = new Mock<IValidator<TransferRequest>>();
            _service = new TransactionsService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _depositWithdrawValidatorMock.Object,
                _transferValidatorMock.Object
            );
        }

        [Fact]
        public void AddDepositWithdrawTransaction_ValidRequest_ReturnsTransactionId()
        {
            // Arrange
            var request = TransactionsServiceTestData.GetDepositWithdrawRequest();
            var transactionDto = new TransactionDto
            {
                AccountId = request.AccountId,
                CurrencyType = request.CurrencyType,
                Amount = request.Amount,
                TransactionType = TransactionType.Deposit
            };

            _depositWithdrawValidatorMock.Setup(v => v.Validate(It.IsAny<DepositWithdrawRequest>()))
                                         .Returns(new ValidationResult());
            _mapperMock.Setup(m => m.Map<TransactionDto>(request)).Returns(transactionDto);
            var transactionId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.AddDepositWithdrawTransaction(It.IsAny<TransactionDto>())).Returns(transactionId);

            // Act
            var result = _service.AddDepositWithdrawTransaction(TransactionType.Deposit, request);

            // Assert
            result.Should().Be(transactionId);
            _repositoryMock.Verify(r => r.AddDepositWithdrawTransaction(It.Is<TransactionDto>(t =>
                t.AccountId == request.AccountId &&
                t.CurrencyType == request.CurrencyType &&
                t.Amount == request.Amount &&
                t.TransactionType == TransactionType.Deposit
            )), Times.Once);
        }

        [Fact]
        public void AddTransferTransaction_ValidRequest_TransactionsAdded()
        {
            // Arrange
            var request = TransactionsServiceTestData.GetTransferRequest();
            _transferValidatorMock.Setup(v => v.Validate(It.IsAny<TransferRequest>()))
                                  .Returns(new ValidationResult());

            // Act
            _service.AddTransferTransaction(request);

            // Assert
            _repositoryMock.Verify(r => r.AddTransferTransaction(It.IsAny<TransactionDto>(), It.IsAny<TransactionDto>()), Times.Once);
        }

        [Fact]
        public void AddDepositWithdrawTransaction_InvalidRequest_ThrowsValidationException()
        {
            // Arrange
            var request = TransactionsServiceTestData.GetDepositWithdrawRequest();
            var validationFailures = new[] { new ValidationFailure("Amount", "Amount must be greater than zero.") };
            _depositWithdrawValidatorMock.Setup(v => v.Validate(It.IsAny<DepositWithdrawRequest>()))
                                         .Returns(new ValidationResult(validationFailures));

            // Act
            Action act = () => _service.AddDepositWithdrawTransaction(TransactionType.Deposit, request);

            // Assert
            act.Should().Throw<ValidationException>().WithMessage("Amount must be greater than zero.");
        }

        [Fact]
        public void AddTransferTransaction_InvalidRequest_ThrowsValidationException()
        {
            // Arrange
            var request = TransactionsServiceTestData.GetTransferRequest();
            var validationFailures = new[] { new ValidationFailure("Amount", "Amount must be greater than zero.") };
            _transferValidatorMock.Setup(v => v.Validate(It.IsAny<TransferRequest>()))
                                  .Returns(new ValidationResult(validationFailures));

            // Act
            Action act = () => _service.AddTransferTransaction(request);

            // Assert
            act.Should().Throw<ValidationException>().WithMessage("Amount must be greater than zero.");
        }
    }
}