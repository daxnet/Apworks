using Castle.DynamicProxy;

namespace Apworks.Tests.Common.Interceptors
{
    public class ExceptionHandlingInterceptor : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }

        #endregion
    }
}
