//#define DEBUG_ENABLED

using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// In Unity EditorWindows lose their state during assembly reloads (e.g. when a script is changed).
	/// To avoid inspector drawers reverting to their default states when this happens, we can serialize
	/// the parts of their state that we want to persist past the assembly reload using this class.
	/// </summary>
	[Serializable]
	public class SplittableInspectorDrawerSerializedState : IDisposable
	{
		[SerializeField]
		private InspectorSerializedState mainViewState;
		[SerializeField]
		private InspectorSerializedState splitViewState;

		#if DONT_USE_ODIN_SERIALIZER
		// Temp ad-hoc fix for Unity 2019.3 alpha when using JsonUtility for serialization.
		// Because JsonUtility replaces null splitViewState with a default instance, we have to
		// manually handle case where view is not split.
		[SerializeField]
		private bool viewIsSplit;
		#endif

		public SplittableInspectorDrawerSerializedState([NotNull]ISplittableInspectorDrawer window)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(window != null, "SplittableInspectorDrawerSerializedState constructor called with null window.");
			Debug.Assert(window.MainView != null);
			Debug.Assert(window.MainView.State != null);
			#endif

			mainViewState = new InspectorSerializedState(window.MainView.State);
			if(window.ViewIsSplit)
			{
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(window != null);
				Debug.Assert(window.SplitView != null);
				Debug.Assert(window.SplitView.State != null);
				#endif

				splitViewState = new InspectorSerializedState(window.SplitView.State);
			}
			#if DEV_MODE && PI_ASSERTATIONS
			else { Debug.Assert(splitViewState == null); }
			#endif

			#if DONT_USE_ODIN_SERIALIZER
			viewIsSplit = window.ViewIsSplit;
			#endif

			#if DEV_MODE && DEBUG_ENABLED
			Debug.Log("Saving state of "+window.GetType().Name+"...\nmainView: "+StringUtils.ToString(mainViewState.Deserialize())+"\nsplitView: "+(splitViewState == null ? "null" : StringUtils.ToString(splitViewState.Deserialize())));
			#endif
		}
		
		public void Dispose()
		{
			mainViewState.Dispose();

			if(splitViewState != null)
			{
				splitViewState.Dispose();
			}

			GC.SuppressFinalize(this);
		}

		public void Serialize(string fullFilePath)
		{
			var bytes = ToBytes();
			
			#if DEV_MODE && PI_ASSERTATIONS
			if(splitViewState == null) { Debug.Assert(Deserialize(bytes).splitViewState == null, "splitViewState should be null but was not after being turned to bytes and back!"); }
			else { Debug.Assert(Deserialize(bytes).splitViewState != null); }
			#endif

			File.WriteAllBytes(fullFilePath, bytes);
		}

		private byte[] ToBytes()
		{
			return PrettySerializer.ToBytes(this);
		}

		[NotNull]
		public static SplittableInspectorDrawerSerializedState Deserialize(string fullFilePath)
		{
			return Deserialize(File.ReadAllBytes(fullFilePath));
		}

		[NotNull]
		public static SplittableInspectorDrawerSerializedState Deserialize(byte[] bytes)
		{
			var result = PrettySerializer.FromBytes<SplittableInspectorDrawerSerializedState>(bytes);
			#if DONT_USE_ODIN_SERIALIZER
			if(!result.viewIsSplit) { result.splitViewState = null; }
			#endif
			return result;
		}

		public void Deserialize([NotNull]ISplittableInspectorDrawer drawer)
		{
			#if DEV_MODE && DEBUG_ENABLED
			Debug.Log("Restoring state of "+drawer+"...\nmainView: "+StringUtils.ToString(mainViewState.Deserialize())+"\nsplitView: "+(splitViewState == null ? "null" : StringUtils.ToString(splitViewState.Deserialize())));
			#endif

			mainViewState.Deserialize(drawer, drawer.MainView);

			#if DONT_USE_ODIN_SERIALIZER
			if(!viewIsSplit)
			{
				drawer.SetSplitView(false);
				return;
			}
			#endif

			if(splitViewState != null)
			{
				mainViewState.Deserialize(drawer, drawer.ViewIsSplit ? drawer.SplitView : null);
			}
			else
			{
				drawer.SetSplitView(false);
			}
		}
	}
}