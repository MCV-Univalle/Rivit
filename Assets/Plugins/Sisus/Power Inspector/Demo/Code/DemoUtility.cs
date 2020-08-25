using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus.Demo
{
	public static class DemoUtility
	{
		public static void ShowCode(MonoBehaviour monoBehaviour)
		{
			#if UNITY_EDITOR
			var script = MonoScript.FromMonoBehaviour(monoBehaviour);
			var inspector = InspectorUtility.ActiveInspector;

			bool isMiddleClick = Event.current.button == 2;

			inspector.OnNextLayout(()=>
			{
				var splittable = inspector.InspectorDrawer as ISplittableInspectorDrawer;
				if(splittable != null)
				{
					if(isMiddleClick)
					{
						splittable.ShowInSplitView(script);
						return;
					}
				
					if(splittable.ViewIsSplit)
					{
						splittable.CloseSplitView();
					}
				}
				inspector.Select(script);
			});
			#endif
		}
	}
}