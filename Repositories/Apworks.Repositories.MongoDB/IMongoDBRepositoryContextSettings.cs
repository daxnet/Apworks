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
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Apworks.Repositories.MongoDB
{
    /// <summary>
    /// Represents the methods that maps a given <see cref="Type"/> object to
    /// the name of the <see cref="MongoCollection"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> object.</param>
    /// <returns>The name of the <see cref="MongoCollection"/>.</returns>
    public delegate string MapTypeToCollectionNameDelegate(Type type);

    /// <summary>
    /// Represents that the implemented classes are MongoDB repository context settings.
    /// </summary>
    public interface IMongoDBRepositoryContextSettings
    {
        /// <summary>
        /// Gets the database name.
        /// </summary>
        string DatabaseName { get; }
        /// <summary>
        /// Gets the instance of <see cref="MongoServerSettings"/> class which represents the
        /// settings for MongoDB server.
        /// </summary>
        MongoServerSettings ServerSettings { get; }
        /// <summary>
        /// Gets the instance of <see cref="MongoDatabaseSettings"/> class which represents the
        /// settings for MongoDB database.
        /// </summary>
        /// <param name="server">The MongoDB server instance.</param>
        /// <returns>The instance of <see cref="MongoDatabaseSettings"/> class.</returns>
        MongoDatabaseSettings GetDatabaseSettings(MongoServer server);
        /// <summary>
        /// Gets the method that maps a given <see cref="Type"/> object to
        /// the name of the <see cref="MongoCollection"/>.
        /// </summary>
        MapTypeToCollectionNameDelegate MapTypeToCollectionName { get; }
    }
}
