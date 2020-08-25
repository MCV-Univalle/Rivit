#if UNITY_EDITOR
using System;
using System.Reflection;
using Object = UnityEngine.Object;

#if !NET_STANDARD_2_0
using Sisus.Vexe.FastReflection;
#endif

namespace Sisus
{
	/// <summary> Can be used for drawing the asset bundle GUI for target objects in the editor. </summary>
	public class AssetBundleGUIDrawer
	{
		private object[] onAssetBundleNameGUIParams = new object[1];
		private object assetBundleNameGUI;
		#if !NET_STANDARD_2_0
		private MethodCaller<object, object> onAssetBundleNameGUI;
		#else
		private MethodInfo onAssetBundleNameGUI;
		#endif
		
		public bool HasTarget
		{
			get
			{
				return onAssetBundleNameGUIParams[0] != null;
			}
		}

		public AssetBundleGUIDrawer()
		{
			#if UNITY_EDITOR
			var type = Types.GetInternalEditorType("UnityEditor.AssetBundleNameGUI");
			assetBundleNameGUI = Activator.CreateInstance(type);
			#if (UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0
			onAssetBundleNameGUI = type.GetMethod("OnAssetBundleNameGUI", BindingFlags.Public | BindingFlags.Instance).DelegateForCall();
			#else
			onAssetBundleNameGUI = type.GetMethod("OnAssetBundleNameGUI", BindingFlags.Public | BindingFlags.Instance);
			#endif
			#endif
		}
		
		public void ResetState()
		{
			onAssetBundleNameGUIParams[0] = null;
		}

		public void SetAssets(Object[] targets)
		{
			onAssetBundleNameGUIParams[0] = targets;
		}

		public void Draw()
		{
			#if UNITY_EDITOR && !NET_STANDARD_2_0
			onAssetBundleNameGUI(assetBundleNameGUI, onAssetBundleNameGUIParams);
			#else
			onAssetBundleNameGUI.Invoke(assetBundleNameGUI, onAssetBundleNameGUIParams);
			#endif
		}
	}
}
#endif