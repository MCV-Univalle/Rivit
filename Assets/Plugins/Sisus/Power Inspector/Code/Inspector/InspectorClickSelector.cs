using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Sisus
{
	[AddComponentMenu("")]
	public class InspectorClickSelector : MonoBehaviour
	{
		private IInspectorDrawer inspectorDrawer;

		// suppress warnings for "Field is never assigned to, and will always have its default value"
		// since field values are actually assigned via the Inspector
		#pragma warning disable 0649

		[SerializeField, UsedImplicitly(ImplicitUseKindFlags.Assign)]
		private int layerMask;
		
		[SerializeField, UsedImplicitly(ImplicitUseKindFlags.Assign)]
		private QueryTriggerInteraction triggerInteraction;
		
		[SerializeField, UsedImplicitly(ImplicitUseKindFlags.Assign)]
		private Type requireComponentForSelectables;

		#pragma warning restore 0649

		private readonly RaycastHit[] raycastHits = new RaycastHit[10];
		private readonly List<Component> components = new List<Component>();

		[UsedImplicitly]
		private void Awake()
		{
			if(inspectorDrawer == null)
			{
				var componentTypes = TypeExtensions.GetImplementingComponentTypes(typeof(IInspectorDrawer), false);
				for(int n = componentTypes.Length - 1; n >= 0; n--)
				{
					inspectorDrawer = GetComponent(componentTypes[n]) as IInspectorDrawer;
					if(inspectorDrawer != null)
					{
						break;
					}
				}
				Debug.Assert(inspectorDrawer != null, "InspectorClickSelector could not find any Component implementing ISelectionManager in scene!");
			}
		}

		[UsedImplicitly]
		private void Update()
		{
			if(Input.GetMouseButtonDown(0))
			{
				int hitCount = Physics.RaycastNonAlloc(Camera.main.ScreenPointToRay(Input.mousePosition), raycastHits, Mathf.Infinity, layerMask, triggerInteraction);
				for(int n = 0; n < hitCount; n++)
				{
					var hit = raycastHits[n];

					if(requireComponentForSelectables != null)
					{
						hit.transform.GetComponents(components);
						bool requiredComponentFound = false;
						for(int c = components.Count - 1; c >= 0; c--)
						{
							var component = components[c];
							if(component != null && requireComponentForSelectables.IsAssignableFrom(component.GetType()))
							{
								requiredComponentFound = true;
								break;
							}
						}
						components.Clear();

						if(!requiredComponentFound)
						{
							continue;
						}
					}

					inspectorDrawer.SelectionManager.Select(hit.transform.gameObject);
				}
			}
		} 
	}
}