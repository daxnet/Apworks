using System;
using Apworks.Exceptions;

namespace Apworks.Tests.Common.ExceptionHandlers
{
    public class SystemExceptionHandler : ExceptionHandler<SystemException>
    {
        protected override bool DoHandle(SystemException ex)
        {
            Helper.WriteTempFile("SystemExceptionHandler");
            return true;
        }
    }
}
