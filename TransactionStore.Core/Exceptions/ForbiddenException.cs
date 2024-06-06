namespace TransactionStore.Core.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Access denied. / Доступ запрещен.") { }

    public ForbiddenException(string message) : base(message) { }
}