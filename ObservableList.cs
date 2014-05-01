using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Tmc.SystemFrameworks.Common
{
	[Serializable]
	public class ObservableList<T> : List<T>//, INotifyCollectionChanged
	{
		public ObservableList() { }
		public ObservableList(IEnumerable<T> enumerable) : base(enumerable) { }
		public ObservableList(int capacity) : base(capacity) { }

		public new void Add(T obj)
		{
			base.Add(obj);
			OnCollectionChanged();
		}

		public new void AddRange(IEnumerable<T> obj)
		{
			base.AddRange(obj);
			OnCollectionChanged();
		}

		public new void Clear()
		{
			base.Clear();
			OnCollectionChanged();
		}

		public new void Insert(int index, T obj)
		{
			base.Insert(index, obj);
			OnCollectionChanged();
		}

		public new void InsertRange(int index, IEnumerable<T> obj)
		{
			base.InsertRange(index, obj);
			OnCollectionChanged();
		}

		public new void Remove(T obj)
		{
			base.Add(obj);
			OnCollectionChanged();
		}


		public new int RemoveAll(Predicate<T> predicate)
		{
			int retVal = base.RemoveAll(predicate);
			if (retVal > 0)
			{
				OnCollectionChanged();
			}
			return retVal;
		}

		public new void RemoveAt(int index)
		{
			base.RemoveAt(index);
			OnCollectionChanged();
		}

		public new void RemoveRange(int index, int count)
		{
			base.RemoveRange(index, count);
			OnCollectionChanged();
		}

		public new void Reverse()
		{
			base.Reverse();
			OnCollectionChanged();
		}

		public new void Reverse(int index, int count)
		{
			base.Reverse(index, count);
			OnCollectionChanged();
		}

		public new void Sort()
		{
			base.Sort();
			OnCollectionChanged();
		}

		public new void Sort(Comparison<T> comparison)
		{
			base.Sort(comparison);
			OnCollectionChanged();
		}

		public new void Sort(IComparer<T> comparer)
		{
			base.Sort(comparer);
			OnCollectionChanged();
		}

		public new void Sort(int index, int count, IComparer<T> comparer)
		{
			base.Sort(index, count, comparer);
			OnCollectionChanged();
		}

		public new int Capacity
		{
			get { return base.Capacity; }
			set
			{
				base.Capacity = value;
				if (value < Count)
				{
					OnCollectionChanged();
				}
			}
		}

		protected void OnCollectionChanged()
		{
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public new T this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = value;
				OnCollectionChanged();
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
