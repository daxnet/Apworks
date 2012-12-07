using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apworks.Repositories.MongoDB;
using MongoDB.Driver;

namespace Apworks.Tests.Common.MongoDB
{
    public class MongoDBRepositoryContextSettings : IMongoDBRepositoryContextSettings
    {
        #region IMongoDBRepositoryContextSettings Members

        public MongoServerSettings ServerSettings
        {
            get
            {
                var settings = new MongoServerSettings();
                settings.Server = new MongoServerAddress("localhost");
                settings.SafeMode = SafeMode.True;
                return settings;
            }
        }

        public MongoDatabaseSettings GetDatabaseSettings(MongoServer server)
        {
            return new MongoDatabaseSettings(server, Helper.MongoDB_Database);
        }

        public MapTypeToCollectionNameDelegate MapTypeToCollectionName
        {
            get { return null; }
        }

        #endregion
    }
}
