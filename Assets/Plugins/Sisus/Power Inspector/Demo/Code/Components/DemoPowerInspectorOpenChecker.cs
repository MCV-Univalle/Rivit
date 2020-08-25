#if UNITY_EDITOR
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

namespace Sisus.Demo
{
	#if UNITY_2018_3_OR_NEWER
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class DemoPowerInspectorOpenChecker : MonoBehaviour
	{
		#if UNITY_2017_3_OR_NEWER // assembly definition files are necessary for referencing PowerInspectorWindow
		[UsedImplicitly]
		private void OnEnable()
		{
			if(!ApplicationUtility.IsReady() || Application.isPlaying)
			{
				return;
			}

			EditorApplication.delayCall += CheckPowerInspectorIsOpen;
		}

		private void CheckPowerInspectorIsOpen()
		{
			if(!ApplicationUtility.IsReady())
			{
				return;
			}

			if(InspectorUtility.ActiveManager == null || InspectorUtility.ActiveManager.FirstInspector == null)
			{
				if(Resources.FindObjectsOfTypeAll<PowerInspectorWindow>().Length == 0)
				{
					if(EditorUtility.DisplayDialog("Open Power Inspector?", "This demo scene has been designed to showcase the different features of Power Inspector.\n\nWould you like to open the Power Inspector window now?", "Open Power Inspector", "No thanks"))
					{
						PowerInspectorWindowUtility.OpenWindowIfNotAlreadyOpen();
					}
				}
			}
		}
		#endif
	}
}
#endif