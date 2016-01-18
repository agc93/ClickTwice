using System;

namespace ClickTwice.Publisher.Core.Exceptions
{
    public class OperationInProgressException : Exception
    {
        public OperationInProgressException(OperationType operationType) : base(BuildMessage(operationType))
        {
            this.Operation = operationType;
        }

        public OperationInProgressException(OperationType operationType, InvalidOperationException innerException) : base(BuildMessage(operationType), innerException)
        {
            
        }

        private static string BuildMessage(OperationType operationType)
        {
            return $"{operationType} operation already in progress";
        }

        public OperationType Operation { get; set; }
    }
}