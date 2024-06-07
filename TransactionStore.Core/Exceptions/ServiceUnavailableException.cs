namespace TransactionStore.Core.Exceptions;

public class ServiceUnavailableException(string message) : Exception(message)
{
}