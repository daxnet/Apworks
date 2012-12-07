using System;
using Apworks.Exceptions;

namespace Apworks.Tests.Common.ExceptionHandlers
{
    public class InvalidOperationExceptionHandler : ExceptionHandler<InvalidOperationException>
    {
        protected override bool DoHandle(InvalidOperationException ex)
        {
            Helper.WriteTempFile("InvalidOperationExceptionHandler");
            return true;
        }
    }
}
