namespace Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        public abstract string ErrorCode { get; }

        protected DomainException(string message) : base(message) { }
        protected DomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BusinessRuleViolationException : DomainException
    {
        public override string ErrorCode => "BUSINESS_RULE_VIOLATION";

        public BusinessRuleViolationException(string message) : base(message) { }
    }

    public class EntityNotFoundException : DomainException
    {
        public override string ErrorCode => "NOT_FOUND";

        public EntityNotFoundException(string entityName, object key)
            : base($"{entityName} with identifier '{key}' was not found") { }
    }
}
