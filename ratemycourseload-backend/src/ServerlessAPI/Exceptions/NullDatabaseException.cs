namespace ServerlessAPI.Exceptions;

public class NullDatabaseException : Exception
{
    public NullDatabaseException()
    {
    }

    public NullDatabaseException(string message) : base(message)
    {
    }

    public NullDatabaseException(string message, Exception inner) : base(message, inner)
    {
    }
}