//---------------------------------------------------------------------------------------------
// <copyright file="ShuffleBag.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace BubbleBreak
{
	public class ShuffleBag<T> : ICollection<T>, IList<T>
	{
		readonly List<T> data = new List<T> ();
		int cursor;
		//private T last;
		Random newRandom = new Random();

		/// <summary>
		/// Get the next value from the ShuffleBag
		/// </summary>
		public T Next ()
		{
			if (cursor < 1) {
				cursor = data.Count - 1;
				return data.Count < 1 ? default(T) : data [0];
			}
			int grab = (int)Math.Floor (newRandom.NextDouble() * (cursor + 1));
			T temp = data[grab];
			data[grab] = data [cursor];
			data[cursor] = temp;
			cursor--;
			return temp;
		}

		//---------------------------------------------------------------------------------------------------------
		// Constructor
		//---------------------------------------------------------------------------------------------------------
		//This Constructor will let you do this: ShuffleBag<int> intBag = new ShuffleBag<int>(new int[] {1, 2, 3, 4, 5});

		public ShuffleBag(T[] initalValues) {
			for (int i = 0; i < initalValues.Length; i++) {
				Add (initalValues[i]);
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// Constructor
		//---------------------------------------------------------------------------------------------------------
		//Constructor with no values

		public ShuffleBag() {} 

		#region IList[T] implementation
		public int IndexOf (T item)
		{
			return data.IndexOf (item);
		}

		public void Insert (int index, T item)
		{
			cursor = data.Count;
			data.Insert (index, item);
		}

		public void RemoveAt (int index)
		{
			cursor = data.Count - 2;
			data.RemoveAt (index);
		}

		public T this[int index] {
			get {
				return data [index];
			}
			set {
				data [index] = value;
			}
		}
		#endregion



		#region IEnumerable[T] implementation
		IEnumerator<T> IEnumerable<T>.GetEnumerator ()
		{
			return data.GetEnumerator ();
		}
		#endregion

		#region ICollection[T] implementation
		public void Add (T item)
		{
			//Debug.Log (item);
			data.Add (item);
			cursor = data.Count - 1;
		}

		public int Count {
			get {
				return data.Count;
			}
		}

		public void Clear ()
		{
			data.Clear ();
		}

		public bool Contains (T item)
		{
			return data.Contains (item);
		}

		public void CopyTo (T[] array, int arrayIndex)
		{
			foreach (T item in data) {
				array.SetValue (item, arrayIndex);
				arrayIndex = arrayIndex + 1;
			}
		}

		public bool Remove (T item)
		{
			cursor = data.Count - 2;
			return data.Remove (item);
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return data.GetEnumerator ();
		}
		#endregion

	}
}

