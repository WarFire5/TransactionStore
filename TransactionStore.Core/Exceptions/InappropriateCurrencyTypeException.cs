namespace TransactionStore.Core.Exceptions;

public class InappropriateCurrencyTypeException : Exception
{
    public InappropriateCurrencyTypeException(string message) : base(message) { }
}
