using System;
using Castle.DynamicProxy;

namespace Apworks.Tests.Common.Interceptors
{
    public class MockInterceptorA : IInterceptor
    {
        public static event EventHandler InterceptOccur;

        protected static void OnIntercept()
        {
            var tmp = InterceptOccur;
            if (tmp != null)
                tmp(null, EventArgs.Empty);
        }

        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            OnIntercept();
            invocation.Proceed();
        }

        #endregion
    }
}
