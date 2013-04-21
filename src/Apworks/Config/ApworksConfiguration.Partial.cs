using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apworks.Config
{
    partial class ApworksConfigSection
    {
        #region Private Methods
        
        private InterceptContractElement FindInterceptContractElement(Type contractType)
        {
            if (this.Interception.Contracts == null)
                return null;

            foreach (InterceptContractElement interceptContractElement in this.Interception.Contracts)
            {
                if (interceptContractElement.Type.Equals(contractType.AssemblyQualifiedName))
                    return interceptContractElement;
            }
            return null;
        }

        private InterceptMethodElement FindInterceptMethodElement(InterceptContractElement interceptContractElement, MethodInfo method)
        {
            if (interceptContractElement == null)
                return null;
            if (interceptContractElement.Methods == null)
                return null;

            foreach (InterceptMethodElement interceptMethodElement in interceptContractElement.Methods)
            {
                string methodSignature = null;
                if (method.IsGenericMethod)
                    methodSignature = method.GetGenericMethodDefinition().GetSignature();
                else
                    methodSignature = method.GetSignature();
                if (interceptMethodElement.Signature.Equals(methodSignature))
                    return interceptMethodElement;
            }
            return null;
        }

        private IEnumerable<string> FindInterceptorRefNames(InterceptMethodElement interceptMethodElement)
        {
            if (interceptMethodElement == null)
                return null;
            if (interceptMethodElement.InterceptorRefs == null)
                return null;
            List<string> ret = new List<string>();
            foreach (InterceptorRefElement interceptorRefElement in interceptMethodElement.InterceptorRefs)
            {
                ret.Add(interceptorRefElement.Name);
            }
            return ret;
        }

        private string FindInterceptorTypeNameByRefName(string refName)
        {
            if (this.Interception == null || this.Interception.Interceptors == null)
                return null;
            foreach (InterceptorElement interceptorElement in this.Interception.Interceptors)
            {
                if (interceptorElement.Name.Equals(refName))
                    return interceptorElement.Type;
            }
            return null;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the serialized XML string for the current Apworks configuration section.
        /// </summary>
        /// <returns>The serialized XML string.</returns>
        public string GetSerializedXmlString()
        {
            return this.SerializeSection(null, AppConfigSource.DefaultConfigSection, System.Configuration.ConfigurationSaveMode.Full);
        }

        /// <summary>
        /// Returns a list of <see cref="System.String"/> values which represents the types
        /// of interceptors references by the given method.
        /// </summary>
        /// <param name="contractType">The type for the method.</param>
        /// <param name="method">The method.</param>
        /// <returns>A list of <see cref="System.String"/> values which contains the interceptor types.</returns>
        public IEnumerable<string> GetInterceptorTypes(Type contractType, MethodInfo method)
        {
            if (this.Interception == null ||
                this.Interception.Interceptors == null ||
                this.Interception.Contracts == null)
                return null;
            
            InterceptContractElement interceptContractElement = this.FindInterceptContractElement(contractType);
            if (interceptContractElement == null)
                return null;

            InterceptMethodElement interceptMethodElement = this.FindInterceptMethodElement(interceptContractElement, method);
            if (interceptMethodElement == null)
                return null;

            var interceptorRefNames = this.FindInterceptorRefNames(interceptMethodElement);
            if (interceptorRefNames == null || interceptorRefNames.Count() == 0)
                return null;

            List<string> ret = new List<string>();
            foreach (var interceptorRefName in interceptorRefNames)
            {
                var interceptorTypeName = this.FindInterceptorTypeNameByRefName(interceptorRefName);
                if (!string.IsNullOrEmpty(interceptorTypeName))
                    ret.Add(interceptorTypeName);
            }
            return ret;
        }
        #endregion
    }
}
