using System;
using JetBrains.Annotations;
using Object = UnityEngine.Object;
#if !UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
#endif

namespace Sisus
{
	public static class InstanceIdUtility
	{
		[CanBeNull]
		public static Object IdToObject(int instanceId, [NotNull]Type type)
		{
			#if UNITY_EDITOR
			return UnityEditor.EditorUtility.InstanceIDToObject(instanceId);
			#else
			var options = Resources.FindObjectsOfTypeAll(type);
			for(int n = options.Length - 1; n >= 0; n--)
			{
				var test = options[n];
				if(test.GetInstanceID() == instanceId)
				{
					return test;
				}
			}
			return null;
			#endif
		}
	}
}