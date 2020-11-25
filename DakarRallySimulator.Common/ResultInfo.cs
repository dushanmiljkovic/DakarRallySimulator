namespace DakarRallySimulator.Common
{
    public sealed class ResultInfo
    {
        public bool IsOk { get; }
        public ErrorDescriptor Error { get; }

        private ResultInfo(bool isOk, ErrorDescriptor error)
        {
            IsOk = isOk;
            Error = error;
        }

        public string ReportError() => $"ErrorType:{Error.ErrorType}, Message:{Error.Message}";
        public static ResultInfo Ok() => new ResultInfo(true, null);
        public static ResultInfo Fail(ErrorType errorType, string message) => new ResultInfo(false, ErrorDescriptor.Create(errorType, message));
    }

    public sealed class ErrorDescriptor
    {
        public ErrorType ErrorType { get; }
        public string Message { get; }
        private ErrorDescriptor(ErrorType errorType, string message)
        {
            ErrorType = errorType;
            Message = message;
        }
        public static ErrorDescriptor Create(ErrorType errorType, string message) => new ErrorDescriptor(errorType, message);
    }

    public enum ErrorType
    {
        ValidationException,
        UnhandledException,
        DomainException,
        ParseException, 
    }
}

