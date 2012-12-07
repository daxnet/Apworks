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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Apworks.Repositories.MongoDB.Conventions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Apworks.Repositories.MongoDB
{
    /// <summary>
    /// Represents the MongoDB repository context.
    /// </summary>
    public class MongoDBRepositoryContext : RepositoryContext, IMongoDBRepositoryContext
    {
        #region Private Fields
        private readonly Guid id = Guid.NewGuid();
        private readonly MongoServer server;
        private readonly MongoDatabase database;
        private readonly IMongoDBRepositoryContextSettings settings;
        private readonly object syncObj = new object();
        private readonly Dictionary<Type, MongoCollection> mongoCollections = new Dictionary<Type, MongoCollection>();
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>MongoDBRepositoryContext</c> class.
        /// </summary>
        /// <param name="settings">The <see cref="IMongoDBRepositoryContextSettings"/> instance which contains
        /// the setting information for initializing the repository context.</param>
        public MongoDBRepositoryContext(IMongoDBRepositoryContextSettings settings)
        {
            this.settings = settings;
            server = new MongoServer(settings.ServerSettings);
            database = server.GetDatabase(settings.GetDatabaseSettings(server));
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                server.Disconnect();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Registers the MongoDB Bson serialization conventions.
        /// </summary>
        /// <param name="autoGenerateID">A <see cref="Boolean"/> value which indicates whether
        /// the ID value should be automatically generated when a new document is inserting.</param>
        /// <param name="localDateTime">A <see cref="Boolean"/> value which indicates whether
        /// the local date/time should be used when serializing/deserializing <see cref="DateTime"/> values.</param>
        public static void RegisterConventions(bool autoGenerateID = true, bool localDateTime = true)
        {
            RegisterConventions(autoGenerateID, localDateTime, null);
        }
        /// <summary>
        /// Registers the MongoDB Bson serialization conventions.
        /// </summary>
        /// <param name="autoGenerateID">A <see cref="Boolean"/> value which indicates whether
        /// the ID value should be automatically generated when a new document is inserting.</param>
        /// <param name="localDateTime">A <see cref="Boolean"/> value which indicates whether
        /// the local date/time should be used when serializing/deserializing <see cref="DateTime"/> values.</param>
        /// <param name="additionConventions">Additional conventions that needs to be registered.</param>
        public static void RegisterConventions(bool autoGenerateID, bool localDateTime, ConventionProfile additionConventions)
        {
            var convention = new ConventionProfile();
            convention.SetIdMemberConvention(new NamedIdMemberConvention("id", "Id", "ID", "iD"));

            if (autoGenerateID)
                convention.SetIdGeneratorConvention(new GuidIDGeneratorConvention());

            if (localDateTime)
                convention.SetSerializationOptionsConvention(new UseLocalDateTimeConvention());

            if (additionConventions != null)
                convention.Merge(additionConventions);

            BsonClassMap.RegisterConventions(convention, type => true);
        }

        #endregion

        #region IMongoDBRepositoryContext Members
        /// <summary>
        /// Gets a <see cref="IMongoDBRepositoryContextSettings"/> instance which contains the settings
        /// information used by current context.
        /// </summary>
        public IMongoDBRepositoryContextSettings Settings
        {
            get { return settings; }
        }
        /// <summary>
        /// Gets the <see cref="MongoCollection"/> instance by the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> object.</param>
        /// <returns>The <see cref="MongoCollection"/> instance.</returns>
        public MongoCollection GetCollectionForType(Type type)
        {
            lock (syncObj)
            {
                if (this.mongoCollections.ContainsKey(type))
                    return this.mongoCollections[type];
                else
                {
                    MongoCollection mongoCollection = null;
                    if (settings.MapTypeToCollectionName != null)
                        mongoCollection = this.database.GetCollection(settings.MapTypeToCollectionName(type));
                    else
                        mongoCollection = this.database.GetCollection(type.Name);
                    this.mongoCollections.Add(type, mongoCollection);
                    return mongoCollection;
                }
            }
        }
        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void Commit()
        {
            lock (syncObj)
            {
                foreach (var newObj in this.NewCollection)
                {
                    MongoCollection collection = this.GetCollectionForType(newObj.GetType());
                    collection.Insert(newObj);
                }
                foreach (var modifiedObj in this.ModifiedCollection)
                {
                    MongoCollection collection = this.GetCollectionForType(modifiedObj.GetType());
                    collection.Save(modifiedObj);
                }
                foreach (var delObj in this.DeletedCollection)
                {
                    Type objType = delObj.GetType();
                    PropertyInfo propertyInfo = objType.GetProperty("ID", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (propertyInfo == null)
                        throw new InvalidOperationException("Cannot delete an object which doesn't contain an ID property.");
                    Guid id = (Guid)propertyInfo.GetValue(delObj, null);
                    MongoCollection collection = this.GetCollectionForType(objType);
                    IMongoQuery query = Query.EQ("_id", id);
                    collection.Remove(query);
                }
                this.ClearRegistrations();
                this.Committed = true;
            }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            this.Committed = false;
        }

        #endregion
    }
}
