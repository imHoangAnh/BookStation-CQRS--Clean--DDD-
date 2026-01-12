namespace BookStation.Core.Exceptions;
public class DomainExceptions : Exception
{
    public DomainExceptions(string message) : base(message)
    {
    }

    public DomainExceptions(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
public class BusinessRuleException : DomainExceptions
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}