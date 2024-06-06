namespace TransactionStore.Core.Exceptions;

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException() : base("There is no connection to the database. / Нет соединения с базой данных.") { }

    public ServiceUnavailableException(string message) : base(message) { }
}