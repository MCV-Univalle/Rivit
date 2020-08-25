#pragma warning disable 0414

using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ShowInInspectorDemo : MonoBehaviour
	{
		[PHeader("The <em>ShowInInspector</em> attribute can be used to expose any <em>field, property or method</em> in Power Inspector (including <em>static</em> ones).")]
		[ShowInInspector]
		private bool nonSerializedFieldSupport = true;
		
		[field : SerializeField]
		#if CSHARP_7_3_OR_NEWER
		public bool SerializedPropertySupport { get; set; } = true;
		#else
		public bool SerializedPropertySupport { get; set; }
		#endif

		[ShowInInspector]
		public bool NonSerializedPropertySupport
		{
			get
			{
				return true;
			}

			set
			{
				Debug.Log((value ? "Correct." : "Incorrect.") + " Properties are supported.");
			}
		}

		[ShowInInspector]
		#if CSHARP_7_3_OR_NEWER
		public bool GetOnlyPropertySupport { get; } = true;
		#else
		public bool GetOnlyPropertySupport { get; }
		#endif

		[ShowInInspector]
		public bool SetOnlyPropertySupport
		{
			set
			{
				Debug.Log((value ? "Correct." : "Incorrect.") + " Set Only Properties are supported.");
			}
		}

		[ShowInInspector]
		private bool MethodSupport()
		{
			return true;
		}

		[ShowInInspector]
		private static bool StaticMethodSupport()
		{
			return true;
		}

		[ShowInInspector]
		private static bool MethodWithParametersSupport(bool isSupported = true)
		{
			Debug.Log((isSupported ? "Correct." : "Incorrect.") + " Methods with parameters are supported.");
			return true;
		}

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("showininspector");
		}
	}
}