using System.Collections.Generic;
using System.Linq;
using Tmc.SystemFrameworks.Common;

namespace JsonTranslationManager
{

    /// <summary>
    /// Unique Collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniqueList<T> : ObservableList<T>
    {
        public delegate bool DelegateEquals(T a, T b);

        private readonly DelegateEquals _equalsFunction;

        public UniqueList()
        {
            _equalsFunction = Equals;
        }

        public UniqueList(DelegateEquals function)
        {
            _equalsFunction = function;
        }

        public UniqueList(UniqueList<T> source)
            : base(source)
        {
            _equalsFunction = source._equalsFunction;
        } 

        /// <summary>
        /// Method that adds a new item if item is unique based on specified condition
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns true if item is added to the collection</returns>
        public new void Add(T item)
        {
            if (!this.Any(i => _equalsFunction(i, item)))
            {
                base.Add(item);
            }
        }

        public new void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

	    public new void Insert(int index, T item)
	    {
            if (!this.Any(i => _equalsFunction(i, item)))
		    {
			    base.Insert(index, item);
		    }
	    }

        protected virtual bool Equals(T a, T b)
        {
            return a.Equals(b);
        }
    }
}
