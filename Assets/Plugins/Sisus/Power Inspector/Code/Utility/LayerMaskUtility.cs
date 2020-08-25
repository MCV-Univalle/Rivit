using System.Collections.Generic;
using UnityEngine;

namespace Sisus
{
	public static class LayerMaskUtility
	{
		public const int FirstUserLayerIndex = 8;
		public const int LastLayerIndex = 31;

		public static readonly string[] LayerMaskNames = new LayerMaskNamesBuilder();
		public static readonly int[] LayerMaskIndexes = new LayerMaskNamesBuilder();
		
		private class LayerMaskNamesBuilder
		{
			private string[] names;
			private int[] indexes;

			public LayerMaskNamesBuilder()
			{
				var nameList = new List<string>(LastLayerIndex);
				var indexList = new List<int>(LastLayerIndex);
				
				for(int i = 0; i <= LastLayerIndex; i++)
				{
					var layerName = LayerMask.LayerToName(i);
					if(layerName.Length > 0)
					{
						nameList.Add(layerName);
						indexList.Add(i);
					}
				}
				names = nameList.ToArray();
				indexes = indexList.ToArray();
			}

			public static implicit operator string[](LayerMaskNamesBuilder layerMaskNames)
			{
				return layerMaskNames.names;
			}

			public static implicit operator int[] (LayerMaskNamesBuilder layerMaskNames)
			{
				return layerMaskNames.indexes;
			}
		}
	}
}