using System;
using System.Collections.Generic;

namespace Kernel.Exceptions
{
    public class DomainException : Exception
    {
        public IList<DomainError> DomainErrors { get; private set; } = new List<DomainError>();

        public DomainException(string message) : base(message)
        {
        }

        public void AddDomainError(string errorCode, string errorMessage, string propertyName, object attemptedValue, string className = "",
            DomainErrorType errorType = DomainErrorType.Error)
        {
            DomainErrors.Add(new DomainError(errorCode, errorMessage, propertyName, attemptedValue, className, errorType));
        }
    }

    public class DomainError
    {
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public string PropertyName { get; }
        public object AttemptedValue { get; }
        public DomainErrorType ErrorType { get; }
        public string ClassName { get; }
        
        public DomainError(string errorCode, string errorMessage, string propertyName, object attemptedValue, string className,
            DomainErrorType errorType = DomainErrorType.Error)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
            AttemptedValue = attemptedValue;
            ClassName = className;
            ErrorType = errorType;
        }
    }

    public enum DomainErrorType
    {
        Error = 0,
        Warning = 1,
        Info = 2
    }
}
