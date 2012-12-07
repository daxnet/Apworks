using System;
using Apworks.Exceptions;

namespace Apworks.Tests.Common.ExceptionHandlers
{
    public class ExceptionExceptionHandler : ExceptionHandler<Exception>
    {
        protected override bool DoHandle(Exception ex)
        {
            Helper.WriteTempFile("ExceptionExceptionHandler");
            return true;
        }
    }
}
