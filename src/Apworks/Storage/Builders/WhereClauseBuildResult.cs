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
// Copyright (C) 2010-2013 apworks.org.
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

namespace Apworks.Storage.Builders
{
    /// <summary>
    /// Represents the where clause build result.
    /// </summary>
    public sealed class WhereClauseBuildResult
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets a <c>System.String</c> value which represents the generated
        /// WHERE clause.
        /// </summary>
        public string WhereClause { get; set; }
        /// <summary>
        /// Gets or sets a <c>Dictionary&lt;string, object&gt;</c> instance which contains
        /// the mapping of the parameters and their values.
        /// </summary>
        public Dictionary<string, object> ParameterValues { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>WhereClauseBuildResult</c> class.
        /// </summary>
        public WhereClauseBuildResult() { }
        /// <summary>
        /// Initializes a new instance of <c>WhereClauseBuildResult</c> class.
        /// </summary>
        /// <param name="whereClause">The <c>System.String</c> value which represents the generated
        /// WHERE clause.</param>
        /// <param name="parameterValues">The <c>Dictionary&lt;string, object&gt;</c> instance which contains
        /// the mapping of the parameters and their values.</param>
        public WhereClauseBuildResult(string whereClause, Dictionary<string, object> parameterValues)
        {
            WhereClause = whereClause;
            ParameterValues = parameterValues;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <c>System.String</c> object which represents the content of the Where Clause
        /// Build Result.
        /// </summary>
        /// <returns>A <c>System.String</c> object which represents the content of the Where Clause
        /// Build Result.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(WhereClause);
            sb.Append(Environment.NewLine);
            ParameterValues.ToList().ForEach(kvp =>
                {
                    sb.Append(string.Format("{0} = [{1}] (Type: {2})", kvp.Key, kvp.Value.ToString(), kvp.Value.GetType().FullName));
                    sb.Append(Environment.NewLine);
                });
            return sb.ToString();
        }
        #endregion
    }
}
