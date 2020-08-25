using UnityEngine;
#if UNITY_EDITOR
using Sisus.Attributes;
using UnityEditor;
#endif

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ScriptDrawerDemo : MonoBehaviour
	{
		#if UNITY_EDITOR
		[PHeader("The <em>script drawer</em> contains various improvements such as <em>colorful syntax highlighting</em> and <em>line numbering</em>.")]
		[Button]
		private void Demo()
		{
			var script = MonoScript.FromMonoBehaviour(this);
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
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowDrawerInfo("script-drawer");
		}
		#endif
	}
}