using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Apworks.Services.ApplicationServices
{
    /// <summary>
    /// Represents the data transfer object which maps the domain object to serializable data
    /// object and vice versa.
    /// </summary>
    /// <typeparam name="TDomainObject">The type of the domain object.</typeparam>
    [Serializable]
    [DataContract]
    [XmlRoot]
    public abstract class DataTransferObject<TDomainObject>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>DataTransferObject</c> class.
        /// </summary>
        public DataTransferObject() { }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Maps the domain object to the current data transfer object.
        /// </summary>
        /// <param name="domainObject">The domain object to be mapped.</param>
        protected abstract void MapFrom(TDomainObject domainObject);
        /// <summary>
        /// Maps the current data transfer object to the domain object.
        /// </summary>
        /// <returns>The domain object.</returns>
        protected abstract TDomainObject MapTo();
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Maps the domain object to the data transfer object.
        /// </summary>
        /// <typeparam name="TDO">The type of the domain object.</typeparam>
        /// <typeparam name="TDTO">The type of the data transfer object.</typeparam>
        /// <param name="domainObject">The instance of the domain object to be mapped.</param>
        /// <returns>The instance of the data transfer object.</returns>
        public static TDTO MapFrom<TDO, TDTO>(TDO domainObject)
            where TDTO : DataTransferObject<TDO>, new()
        {
            TDTO dto = new TDTO();
            dto.MapFrom(domainObject);
            return dto;
        }
        /// <summary>
        /// Maps the data transfer object to the domain object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the data transfer object.</typeparam>
        /// <typeparam name="TDO">The type of the domain object.</typeparam>
        /// <param name="dto">The instance of the data transfer object.</param>
        /// <returns>The instance of the domain object.</returns>
        public static TDO MapTo<TDTO, TDO>(TDTO dto)
            where TDTO : DataTransferObject<TDO>, new()
        {
            TDO domainObject = dto.MapTo();
            return domainObject;
        }
        #endregion
    }
}
