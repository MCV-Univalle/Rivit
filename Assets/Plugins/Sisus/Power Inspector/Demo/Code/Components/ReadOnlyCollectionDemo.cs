#pragma warning disable CS0414 //we are interested in seeing how the fields look in the inspector, so this warning isn't valid here

using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ReadOnlyCollectionDemo : MonoBehaviour
	{
		[ShowInInspector, PHeader("Collections with the <em>readonly</em> operator can also be displayed.")]
		public readonly string[] readonlyArray = { "1", "2", "3" };

		[ShowInInspector, PHeader("<em>ReadOnlyCollection<T></em> is also supported.")]
		public ReadOnlyCollection<int> readOnlyCollection = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3 });

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}
	}
}