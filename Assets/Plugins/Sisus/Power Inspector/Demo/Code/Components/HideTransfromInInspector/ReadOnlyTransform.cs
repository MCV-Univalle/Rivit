using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo.HideTransformInInspector
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	[HideTransformInInspector]
	public class ReadOnlyTransform : MonoBehaviour
	{
		[ShowInInspector]
		public Vector3 Position
		{
			get
			{
				return transform.localPosition;
			}
		}

		[ShowInInspector]
		public Vector3 Rotation
		{
			get
			{
				return transform.localEulerAngles;
			}
		}

		[ShowInInspector]
		private Vector3 Scale
		{
			get
			{
				return transform.localScale;
			}
		}
	}
}