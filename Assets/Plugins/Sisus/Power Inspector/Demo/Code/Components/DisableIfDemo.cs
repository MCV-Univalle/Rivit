using System;
using UnityEngine;
using Sisus.Attributes;
using JetBrains.Annotations;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class DisableIfDemo : MonoBehaviour
	{
		[DisableIf("hasValidResourceTarget", true)]
		public GameObject prefab;

		[DisableIf("prefab", Is.Not, null)]
		public string prefabResourceName = "";

		[HideInInspector, SerializeField]
		private bool hasValidResourceTarget;

		[ShowInInspector][NotNull]
		public GameObject Instantiate()
		{
			if(prefab != null)
			{
				return Instantiate(prefab);
			}

			if(hasValidResourceTarget)
			{
				return Instantiate(Resources.Load<GameObject>(prefabResourceName));
			}

			throw new NullReferenceException(name + " - Both prefab and resource name missing!");
		}

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("disableif");
		}

		[UsedImplicitly]
		private void OnValidate()
		{
			if(prefab != null)
			{
				var name = prefab.name;
				var tryLoad = Resources.Load<GameObject>(name);
				prefabResourceName = tryLoad != null ? name : "";
				hasValidResourceTarget = false;
			}
			else
			{
				hasValidResourceTarget = prefabResourceName.Length > 0 && Resources.Load<GameObject>(prefabResourceName) != null;
			}
		}
	}
}