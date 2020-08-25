//#define DEBUG_ENABLED

using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sisus
{
	[Serializable]
	public class InspectorSerializedState : IDisposable
	{
		[SerializeField]
		private int[] inspected;

		[SerializeField]
		private bool viewLocked;

		[SerializeField]
		private Vector2 scrollPos;

		[SerializeField]
		private string filter;
		
		public InspectorSerializedState(InspectorState inspectorState)
		{
			var targets = inspectorState.inspected;

			int count = targets.Length;

			#if DEV_MODE && DEBUG_ENABLED
			Debug.Log("Creating InspectorSerializedState from "+targets.Length+" targets: "+ StringUtils.ToString(targets));
			#endif

			var references = new int[count];
			for(int n = count - 1; n >= 0; n--)
			{
				var target = targets[n];
				if(target == null)
				{
					#if DEV_MODE
					Debug.LogWarning("InspectorSerializedState inspected["+n+"] reference was null! Will skip serializing.");
					#endif

					references = references.RemoveAt(n);
				}
				else
				{
					references[n] = ObjectIds.Get(target);

					#if DEV_MODE
					Debug.Assert(ObjectIds.GetTarget(references[n]) == target);
					#endif
				}
			}
			inspected = references;

			viewLocked = inspectorState.ViewIsLocked;

			scrollPos = inspectorState.ScrollPos;

			filter = inspectorState.filter.RawInput;
		}

		public void Dispose()
		{
			inspected = null;
			GC.SuppressFinalize(this);
		}

		public Object[] Deserialize()
		{
			#if DEV_MODE && DEBUG_ENABLED
			Debug.Log("Deserializing InspectorSerializedState from "+ inspected.Length+" inspected...");
			#endif

			int count = inspected.Length;
			var references = new Object[count];
			for(int n = count - 1; n >= 0; n--)
			{
				var target = ObjectIds.GetTarget(inspected[n]);
				if(target != null)
				{
					references[n] = target;
				}
				else
				{
					#if DEV_MODE
					Debug.LogWarning("InspectorSerializedState inspected["+n+"] reference could not be re-established. This is expected behaviour if the target has been destroyed.");
					#endif
					references = references.RemoveAt(n);
				}
			}
			return references;
		}

		/// <summary> Deserialize inspector state. </summary>
		/// <param name="drawer"> The drawer of the inspector. </param>
		/// <param name="inspector">
		/// The inspector over which the state should be deserialized. If this is null, a new split view
		/// will be opened in the drawer, and state will be deserialized over that.
		/// </param>
		public void Deserialize(IInspectorDrawer drawer, [CanBeNull]IInspector inspector)
		{
			#if DEV_MODE && DEBUG_ENABLED
			Debug.Log("Restoring state of "+ inspector + " from "+ inspected.Length+" int...");
			#endif

			if(inspector == null)
			{
				var splittable = drawer as ISplittableInspectorDrawer;
				if(viewLocked)
				{
					splittable.ShowInSplitView(Deserialize());
				}
				else
				{
					splittable.SetSplitView(true);
				}
				inspector = splittable.SplitView;
			}
			else
			{
				inspector.State.ViewIsLocked = viewLocked;
				if(viewLocked)
				{
					inspector.RebuildDrawers(Deserialize(), false);
				}
				else
				{
					inspector.RebuildDrawersIfTargetsChanged();
				}
			}

			if(!string.IsNullOrEmpty(filter))
			{
				inspector.SetFilter(filter);
			}

			inspector.OnNextLayout(()=>inspector.State.ScrollPos = scrollPos);
		}
	}
}