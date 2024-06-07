namespace TransactionStore.Core.Exceptions;

public class ValidationException(string message) : Exception(message)
{
}