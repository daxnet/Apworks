// ==================================================================================================================                                                                                          
//        ,::i                                                           BBB                
//       BBBBBi                                                         EBBB                
//      MBBNBBU                                                         BBB,                
//     BBB. BBB     BBB,BBBBM   BBB   UBBB   MBB,  LBBBBBO,   :BBG,BBB :BBB  .BBBU  kBBBBBF 
//    BBB,  BBB    7BBBBS2BBBO  BBB  iBBBB  YBBJ :BBBMYNBBB:  FBBBBBB: OBB: 5BBB,  BBBi ,M, 
//   MBBY   BBB.   8BBB   :BBB  BBB .BBUBB  BB1  BBBi   kBBB  BBBM     BBBjBBBr    BBB1     
//  BBBBBBBBBBBu   BBB    FBBP  MBM BB. BB BBM  7BBB    MBBY .BBB     7BBGkBB1      JBBBBi  
// PBBBFE0GkBBBB  7BBX   uBBB   MBBMBu .BBOBB   rBBB   kBBB  ZBBq     BBB: BBBJ   .   iBBB  
//BBBB      iBBB  BBBBBBBBBE    EBBBB  ,BBBB     MBBBBBBBM   BBB,    iBBB  .BBB2 :BBBBBBB7  
//vr7        777  BBBu8O5:      .77r    Lr7       .7EZk;     L77     .Y7r   irLY  JNMMF:    
//               LBBj
//
// Apworks Application Development Framework
// Copyright (C) 2010-2011 apworks.codeplex.com.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ==================================================================================================================

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Apworks.Application;
using Apworks.Config;
using Apworks.Events.Serialization;

namespace Apworks.Events.Storage
{
    /// <summary>
    /// Represents the domain event data object which holds the data of a specific domain event.
    /// </summary>
    /// <remarks>The <c>DomainEventDataObject</c> class implemented the Data Transfer Object pattern
    /// that was described in Martin Fowler's book, Patterns of Enterprise Application Architecture.
    /// For more information about Data Transfer Object pattern, please refer to http://martinfowler.com/eaaCatalog/dataTransferObject.html.
    /// </remarks>
    [Serializable]
    [XmlRoot]
    [DataContract]
    public class DomainEventDataObject : DomainEvent
    {
        #region Private Fields
        private readonly IDomainEventSerializer serializer;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the domain event data object.
        /// </summary>
        public DomainEventDataObject()
        {
            this.serializer = GetDomainEventSerializer();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets an array of <see cref="System.Byte"/> value which holds the data
        /// of current domain event object.
        /// </summary>
        [XmlElement]
        [DataMember]
        public byte[] Data { get; set; }
        /// <summary>
        /// Gets or sets the assembly qualified name of the type of the domain event.
        /// </summary>
        [XmlElement]
        [DataMember]
        public override string AssemblyQualifiedEventType
        {
            get
            {
                return base.AssemblyQualifiedEventType;
            }
            set
            {
                base.AssemblyQualifiedEventType = value;
            }
        }
        /// <summary>
        /// Gets or sets the branch on which domain event data object exists.
        /// </summary>
        [XmlElement]
        [DataMember]
        public override long Branch
        {
            get
            {
                return base.Branch;
            }
            set
            {
                base.Branch = value;
            }
        }
        /// <summary>
        /// Gets or sets the identifier of the domain event.
        /// </summary>
        /// <remarks>Note that since the <c>DomainEventDataObject</c> is the data
        /// presentation of the corresponding domain event object, this identifier value
        /// can also be considered to be the identifier for the <c>DomainEventDataObject</c> instance.</remarks>
        [XmlElement]
        [DataMember]
        public override Guid ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                base.ID = value;
            }
        }
        /// <summary>
        /// Gets or sets the identifier of the aggregate root which holds the instance
        /// of the current domain event.
        /// </summary>
        [XmlElement]
        [DataMember]
        public override Guid SourceID
        {
            get
            {
                return base.SourceID;
            }
            set
            {
                base.SourceID = value;
            }
        }
        /// <summary>
        /// Gets or sets the assembly qualified name of the type of the aggregate root.
        /// </summary>
        [XmlElement]
        [DataMember]
        public override string AssemblyQualifiedSourceType
        {
            get
            {
                return base.AssemblyQualifiedSourceType;
            }
            set
            {
                base.AssemblyQualifiedSourceType = value;
            }
        }
        /// <summary>
        /// Gets or sets the date and time on which the event was produced.
        /// </summary>
        /// <remarks>The format of this date/time value could be various between different
        /// systems. Apworks recommend system designer or architect uses the standard
        /// UTC date/time format.</remarks>
        [XmlElement]
        [DataMember]
        public override DateTime Timestamp
        {
            get
            {
                return base.Timestamp;
            }
            set
            {
                base.Timestamp = value;
            }
        }
        /// <summary>
        /// Gets or sets the version of the domain event data object.
        /// </summary>
        [XmlElement]
        [DataMember]
        public override long Version
        {
            get
            {
                return base.Version;
            }
            set
            {
                base.Version = value;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the serializer for domain events.
        /// </summary>
        /// <returns>The domain event serializer instance.</returns>
        private static IDomainEventSerializer GetDomainEventSerializer()
        {
            IDomainEventSerializer serializer = null;
            ApworksConfigSection config = AppRuntime.Instance.CurrentApplication.ConfigSource.Config;
            if (config.Serializers == null ||
                config.Serializers.EventSerializer == null ||
                string.IsNullOrEmpty(config.Serializers.EventSerializer.Provider) ||
                string.IsNullOrWhiteSpace(config.Serializers.EventSerializer.Provider))
            {
                serializer = new DomainEventXmlSerializer();
            }
            else
            {
                string typeName = config.Serializers.EventSerializer.Provider;
                Type serializerType = Type.GetType(typeName);
                if (serializerType == null)
                    throw new InfrastructureException("The serializer defined by type '{0}' doesn't exist.", typeName);
                serializer = (IDomainEventSerializer)Activator.CreateInstance(serializerType);
            }
            return serializer;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates and initializes the domain event data object from the given domain event.
        /// </summary>
        /// <param name="entity">The domain event instance from which the domain event data object
        /// is created and initialized.</param>
        /// <returns>The initialized data object instance.</returns>
        public static DomainEventDataObject FromDomainEvent(IDomainEvent entity)
        {
            IDomainEventSerializer serializer = GetDomainEventSerializer();
            DomainEventDataObject obj = new DomainEventDataObject();
            obj.Branch = entity.Branch;
            obj.Data = serializer.Serialize(entity);
            obj.ID = entity.ID;
            if (string.IsNullOrEmpty(entity.AssemblyQualifiedEventType))
                obj.AssemblyQualifiedEventType = entity.GetType().AssemblyQualifiedName;
            else
                obj.AssemblyQualifiedEventType = entity.AssemblyQualifiedEventType;
            obj.Timestamp = entity.Timestamp;
            obj.Version = entity.Version;
            obj.SourceID = entity.SourceID;
            obj.AssemblyQualifiedSourceType = entity.AssemblyQualifiedSourceType;
            return obj;
        }
        /// <summary>
        /// Converts the domain event data object to its corresponding domain event entity instance.
        /// </summary>
        /// <returns>The domain event entity instance that is converted from the current domain event data object.</returns>
        public IDomainEvent ToDomainEvent()
        {
            if (string.IsNullOrEmpty(this.AssemblyQualifiedEventType))
                throw new ArgumentNullException("AssemblyQualifiedTypeName");
            if (this.Data == null || this.Data.Length == 0)
                throw new ArgumentNullException("Data");

            Type type = Type.GetType(this.AssemblyQualifiedEventType);
            IDomainEvent ret = (IDomainEvent)serializer.Deserialize(type, this.Data);
            ret.ID = this.ID;
            return ret;
        }
        #endregion
    }
}
