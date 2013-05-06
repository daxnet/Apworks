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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apworks.Storage
{
    /// <summary>
    /// Represents the property bag that contains a list of the mapping between the properties and their values
    /// for a specific object.
    /// </summary>
    public class PropertyBag : IEnumerable<KeyValuePair<string, object>>
    {
        #region Private Fields
        private readonly Dictionary<string, object> propertyValues = new Dictionary<string, object>();
        #endregion

        #region Public Static Fields
        /// <summary>
        /// The binding flags for getting properties on a given object.
        /// </summary>
        public static readonly BindingFlags PropertyBagBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>PropertyBag</c> class.
        /// </summary>
        public PropertyBag()
        {
        }
        /// <summary>
        /// Initializes a new instance of <c>PropertyBag</c> class and populates the content by using the given object.
        /// </summary>
        /// <param name="target">The target object used for initializing the property bag.</param>
        public PropertyBag(object target)
        {
            target
                .GetType()
                .GetProperties(PropertyBagBindingFlags)
                .ToList()
                .ForEach(pi => propertyValues.Add(pi.Name, pi.GetValue(target, null)));
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the property value by using the index.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <returns>The property value.</returns>
        public object this[string idx]
        {
            get
            {
                return propertyValues[idx];
            }
            set
            {
                propertyValues[idx] = value;
            }
        }
        /// <summary>
        /// Gets the number of elements in the property bag.
        /// </summary>
        public int Count
        {
            get { return propertyValues.Count; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Clears the property bag.
        /// </summary>
        public void Clear()
        {
            propertyValues.Clear();
        }
        /// <summary>
        /// Adds a property and its value to the property bag.
        /// </summary>
        /// <param name="propertyName">The name of the property to be added.</param>
        /// <param name="propertyValue">The value of the property.</param>
        /// <returns>The instance with the added property.</returns>
        public PropertyBag Add(string propertyName, object propertyValue)
        {
            propertyValues.Add(propertyName, propertyValue);
            return this;
        }
        /// <summary>
        /// Adds a property to property bag, to be used as the sort field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="propertyName">The name of the property to be added.</param>
        /// <returns>The instance with the added sort field.</returns>
        public PropertyBag AddSort<T>(string propertyName)
        {
            propertyValues.Add(propertyName, default(T));
            return this;
        }
        /// <summary>
        /// Gets the <see cref="System.String"/> value which represents the current property bag.
        /// </summary>
        /// <returns>The <see cref="System.String"/> value which represents the current property bag.</returns>
        public override string ToString()
        {
            return string.Format("[{0}]", string.Join(", ", propertyValues.Keys.Select(p => p)));
        }
        #endregion

        #region IEnumerator<KeyValuePair<string, object>> Members
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return propertyValues.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return propertyValues.GetEnumerator();
        }
        #endregion
    }
}
