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
// Copyright (C) 2010-2015 by daxnet.
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
using Apworks.Snapshots.Serialization;
using System.Reflection;

namespace Apworks.Snapshots
{
    /// <summary>
    /// Represents the snapshot data object.
    /// </summary>
    [Serializable]
    [XmlRoot]
    [DataContract]
    public class SnapshotDataObject : IEntity
    {
        #region Private Fields
        private readonly ISnapshotSerializer serializer;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>SnapshotDataObject</c> class.
        /// </summary>
        public SnapshotDataObject()
        {
            ApworksConfigSection config = AppRuntime.Instance.CurrentApplication.ConfigSource.Config;
            if (config.Serializers == null ||
                config.Serializers.SnapshotSerializer == null ||
                string.IsNullOrEmpty(config.Serializers.SnapshotSerializer.Provider) ||
                string.IsNullOrWhiteSpace(config.Serializers.SnapshotSerializer.Provider))
            {
                serializer = new SnapshotXmlSerializer();
            }
            else
            {
                string typeName = config.Serializers.SnapshotSerializer.Provider;
                Type serializerType = Type.GetType(typeName);
                if (serializerType == null)
                    throw new InfrastructureException("The serializer defined by type '{0}' doesn't exist.", typeName);
                serializer = (ISnapshotSerializer)Activator.CreateInstance(serializerType);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets an array of <see cref="System.Byte"/> values that represents
        /// the binary content of the snapshot data.
        /// </summary>
        [XmlElement]
        [DataMember]
        public byte[] SnapshotData { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the aggregate root.
        /// </summary>
        [XmlElement]
        [DataMember]
        public Guid AggregateRootID { get; set; }
        /// <summary>
        /// Gets or sets the assembly qualified name of the type of the aggregate root.
        /// </summary>
        [XmlElement]
        [DataMember]
        public string AggregateRootType { get; set; }
        /// <summary>
        /// Gets or sets the assembly qualified name of the type of the snapshot.
        /// </summary>
        [XmlElement]
        [DataMember]
        public string SnapshotType { get; set; }
        /// <summary>
        /// Gets or sets the version of the snapshot.
        /// </summary>
        /// <remarks>This version is also equal to the version of the event
        /// on which the snapshot was taken.</remarks>
        [XmlElement]
        [DataMember]
        public long Version { get; set; }
        /// <summary>
        /// Gets or sets the branch of the snapshot.
        /// </summary>
        [XmlElement]
        [DataMember]
        public long Branch { get; set; }
        /// <summary>
        /// Gets or sets the timestamp on which the snapshot was taken.
        /// </summary>
        [XmlElement]
        [DataMember]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the snapshot data object.
        /// </summary>
        [XmlElement]
        [DataMember]
        public Guid ID { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Extracts the snapshot from the current snapshot data object.
        /// </summary>
        /// <returns>The snapshot instance.</returns>
        public ISnapshot ExtractSnapshot()
        {
            try
            {
                Type snapshotType = Type.GetType(SnapshotType);
                if (snapshotType == null)
                    return null;
                return (ISnapshot)serializer.Deserialize(snapshotType, this.SnapshotData);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Creates the snapshot data object from the aggregate root.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root for which the snapshot is being created.</param>
        /// <returns>The snapshot data object.</returns>
        public static SnapshotDataObject CreateFromAggregateRoot(ISourcedAggregateRoot aggregateRoot)
        {
            ISnapshotSerializer serializer = null;

            ApworksConfigSection config = AppRuntime.Instance.CurrentApplication.ConfigSource.Config;
            if (config.Serializers == null ||
                config.Serializers.SnapshotSerializer == null ||
                string.IsNullOrEmpty(config.Serializers.SnapshotSerializer.Provider) ||
                string.IsNullOrWhiteSpace(config.Serializers.SnapshotSerializer.Provider))
            {
                serializer = new SnapshotXmlSerializer();
            }
            else
            {
                string typeName = config.Serializers.SnapshotSerializer.Provider;
                Type serializerType = Type.GetType(typeName);
                if (serializerType == null)
                    throw new InfrastructureException("The serializer defined by type '{0}' doesn't exist.", typeName);
                serializer = (ISnapshotSerializer)Activator.CreateInstance(serializerType);
            }

            ISnapshot snapshot = aggregateRoot.CreateSnapshot();

            return new SnapshotDataObject
            {
                AggregateRootID = aggregateRoot.ID,
                AggregateRootType = aggregateRoot.GetType().AssemblyQualifiedName,
                Version = aggregateRoot.Version,
                Branch = Constants.ApplicationRuntime.DefaultBranch,
                SnapshotType = snapshot.GetType().AssemblyQualifiedName,
                Timestamp = snapshot.Timestamp,
                SnapshotData = serializer.Serialize(snapshot)
            };
        }
        /// <summary>
        /// Returns the hash code for current snapshot data object.
        /// </summary>
        /// <returns>The calculated hash code for the current snapshot data object.</returns>
        public override int GetHashCode()
        {
            return Utils.GetHashCode(this.AggregateRootID.GetHashCode(),
                this.AggregateRootType.GetHashCode(),
                this.Branch.GetHashCode(),
                this.ID.GetHashCode(),
                this.SnapshotData.GetHashCode(),
                this.SnapshotType.GetHashCode(),
                this.Timestamp.GetHashCode(),
                this.Version.GetHashCode());
        }
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value indicating whether this instance is equal to a specified
        /// object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if obj is an instance of the <see cref="Apworks.Snapshots.SnapshotDataObject"/> type and equals the value of this
        /// instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            SnapshotDataObject other = obj as SnapshotDataObject;
            if ((object)other == (object)null)
                return false;
            return this.ID == other.ID;
        }
        #endregion

    }
}
