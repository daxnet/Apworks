using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks
{
    /// <summary>
    /// Represents a collection which contains a set of objects that is from
    /// a specific page of the entire object set.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    public class PagedResult<T> : ICollection<T>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>PagedResult</c> class.
        /// </summary>
        public PagedResult()
        {
            this.data = new List<T>();
        }
        /// <summary>
        /// Initializes a new instance of <c>PagedResult</c> class.
        /// </summary>
        /// <param name="totalRecords">Total number of records contained in the entire object set.</param>
        /// <param name="totalPages">Total number of pages.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="data">The objects contained in the current page.</param>
        public PagedResult(int? totalRecords, int? totalPages, int? pageSize, int? pageNumber, IList<T> data)
        {
            this.totalRecords = totalRecords;
            this.totalPages = totalPages;
            this.pageSize = pageSize;
            this.pageNumber = pageNumber;
            this.data = data;
        }
        #endregion

        #region Public Properties
        private int? totalRecords;
        /// <summary>
        /// Gets or sets the total number of the records.
        /// </summary>
        public int? TotalRecords
        {
            get { return totalRecords; }
            set { totalRecords = value; }
        }

        private int? totalPages;
        /// <summary>
        /// Gets or sets the total pages available.
        /// </summary>
        public int? TotalPages
        {
            get { return totalPages; }
            set { totalPages = value; }
        }

        private int? pageSize;
        /// <summary>
        /// Gets or sets the number of records for each page.
        /// </summary>
        public int? PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        private int? pageNumber;
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int? PageNumber
        {
            get { return pageNumber; }
            set { pageNumber = value; }
        }

        private IList<T> data;
        /// <summary>
        /// Gets a list of objects contained by the current <c>PagedResult{T}</c> object.
        /// </summary>
        public IEnumerable<T> Data
        {
            get { return data; }
        }
        #endregion

        #region ICollection<T> Members
        /// <summary>
        /// Adds an item to the System.Collections.Generic.ICollection{T}.
        /// </summary>
        /// <param name="item">The object to add to the System.Collections.Generic.ICollection{T}.</param>
        public void Add(T item)
        {
            data.Add(item);
        }
        /// <summary>
        /// Removes all items from the System.Collections.Generic.ICollection{T}.
        /// </summary>
        public void Clear()
        {
            data.Clear();
        }
        /// <summary>
        /// Determines whether the System.Collections.Generic.ICollection{T} contains
        /// a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the System.Collections.Generic.ICollection{T}.</param>
        /// <returns>true if item is found in the System.Collections.Generic.ICollection{T}; otherwise,
        /// false.</returns>
        public bool Contains(T item)
        {
            return data.Contains(item);
        }
        /// <summary>
        /// Copies the elements of the System.Collections.Generic.ICollection{T} to an
        /// System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements
        /// copied from System.Collections.Generic.ICollection{T}. The System.Array must
        /// have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Gets the number of elements contained in the System.Collections.Generic.ICollection{T}.
        /// </summary>
        public int Count
        {
            get { return data.Count; }
        }
        /// <summary>
        /// Gets a value indicating whether the System.Collections.Generic.ICollection{T}
        /// is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.Generic.ICollection{T}.
        /// </summary>
        /// <param name="item">The object to remove from the System.Collections.Generic.ICollection{T}.</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return data.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A System.Collections.Generic.IEnumerator{T} that can be used to iterate through
        /// the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through
        /// the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }
}
