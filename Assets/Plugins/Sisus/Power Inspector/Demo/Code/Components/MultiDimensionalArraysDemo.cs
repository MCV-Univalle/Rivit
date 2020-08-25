#pragma warning disable CS0414 //we are interested in seeing how the fields look in the inspector, so this warning isn't valid here

using Sisus.OdinSerializer;
using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class MultiDimensionalArraysDemo : SerializedMonoBehaviour
	{
		[PHeader("<em>Two-dimensional arrays</em> can be displayed by Power Inspector.", "They are rendered in one flattened list, similarly to one-dimensional arrays.")]
		[ShowInInspector]
		public string[,] array2D =
		{
			{"0,0","0,1"},
			{"1,0","1,1"}
		};

		[PHeader("<em>Three-dimensional arrays</em> are also supported.")]
		[ShowInInspector]
		public string[,,] array3D =
		{
			{{"0,0,0","0,0,1","0,0,2"}, {"0,1,0","0,1,1","0,1,2"}, {"0,2,0","0,2,1","0,2,2"}},
			{{"1,0,0","1,0,1","1,0,2"}, {"1,1,0","1,1,1","1,1,2"}, {"1,2,0","1,2,1","1,2,2"}},
			{{"2,0,0","2,0,1","2,0,2"}, {"2,1,0","2,1,1","2,1,2"}, {"2,2,0","2,2,1","2,2,2"}}
		};
		
		[PHeader("<em>Jagged arrays</em> are supported.")]
		[ShowInInspector]
		public string[][] arrayJagged =
		{
			new[]{"0,0","0,1"},
			new[]{"1,0","1,1","1,2"},
			new[]{"2,0","2,1","2,2","2,3"},
			new[]{"3,0","3,1","3,2","3,3","3,4"}
		};

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}
	}
}