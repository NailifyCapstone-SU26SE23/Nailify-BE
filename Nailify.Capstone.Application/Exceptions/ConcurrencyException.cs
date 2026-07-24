using System;

namespace Nailify.Capstone.Application.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException() : base("Dữ liệu đã bị thay đổi bởi một tác vụ khác. Vui lòng tải lại trang.")
        {
        }

        public ConcurrencyException(string message) : base(message)
        {
        }

        public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
