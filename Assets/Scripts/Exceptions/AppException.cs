using System;

namespace ProjectCode1
{
    public enum AppError
    {
        Unknown,
        InvalidUsername,
        NotEnoughCurrency,
    }

    public class AppException : Exception
    {
        public AppError Error { get; set; }

        public AppException(string message, AppError error) : base(message)
        {
            Error = error;
        }
    }
}
