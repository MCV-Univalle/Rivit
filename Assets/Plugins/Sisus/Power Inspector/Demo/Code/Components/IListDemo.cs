using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class IListDemo : MonoBehaviour
	{
		[PHeader("Custom classes that implement <em>IList<T></em> are displayed as a list.")]
		[ShowInInspector]
		public MyList<int> IList = new MyList<int>(1, 2, 3);

		[Serializable]
		public class MyList<T> : IList<T>
		{
			[SerializeField]
			private readonly List<T> list;

			public MyList()
			{
				list = new List<T>();
			}

			public MyList(params T[] items)
			{
				list = new List<T>(items);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return list.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return list.GetEnumerator();
			}

			public void Add(T item)
			{
				list.Add(item);
			}

			public void Clear()
			{
				list.Clear();
			}

			public bool Contains(T item)
			{
				return list.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				list.CopyTo(array, arrayIndex);
			}

			public bool Remove(T item)
			{
				return list.Remove(item);
			}

			public int Count
			{
				get
				{
					return list.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return (list as IList<T>).IsReadOnly;
				}
			}

			public int IndexOf(T item)
			{
				return list.IndexOf(item);
			}

			public void Insert(int index, T item)
			{
				list.Insert(index, item);
			}

			public void RemoveAt(int index)
			{
				list.RemoveAt(index);
			}

			public T this[int index]
			{
				get
				{
					return list[index];
				}

				set
				{
					list[index] = value;
				}
			}
		}
	}
}