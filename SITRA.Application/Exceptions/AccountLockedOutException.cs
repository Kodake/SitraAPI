using System;

namespace SITRA.Core.Exceptions
{
    public class AccountLockedOutException : Exception
    {
        public AccountLockedOutException(string message) : base(message)
        {
        }
    }
}
