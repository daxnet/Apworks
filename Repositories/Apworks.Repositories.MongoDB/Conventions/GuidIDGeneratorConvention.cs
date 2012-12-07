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
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Apworks.Repositories.MongoDB.Conventions
{
    /// <summary>
    /// Represents the ID generator convention which generates a <see cref="System.Guid"/> value
    /// for ID.
    /// </summary>
    public class GuidIDGeneratorConvention : IIdGeneratorConvention
    {
        #region IIdGeneratorConvention Members
        /// <summary>
        /// Gets the Id generator for an Id member.
        /// </summary>
        /// <param name="memberInfo">The member.</param>
        /// <returns>An Id generator.</returns>
        public virtual IIdGenerator GetIdGenerator(MemberInfo memberInfo)
        {
            if (memberInfo.DeclaringType.GetInterfaces().Any(intf => intf == typeof(IEntity)) && 
                (memberInfo.Name == "ID" || memberInfo.Name == "Id" || memberInfo.Name == "iD" ||
                memberInfo.Name == "id" || memberInfo.Name == "_id"))
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
                        if (propertyInfo.PropertyType == typeof(Guid) ||
                            propertyInfo.PropertyType == typeof(Guid?))
                            return new GuidGenerator();
                        break;
                    case MemberTypes.Field:
                        FieldInfo fieldInfo = (FieldInfo)memberInfo;
                        if (fieldInfo.FieldType == typeof(Guid) ||
                            fieldInfo.FieldType == typeof(Guid?))
                            return new GuidGenerator();
                        break;
                    default:
                        break;
                }
            }
            return null;
        }

        #endregion
    }
}
