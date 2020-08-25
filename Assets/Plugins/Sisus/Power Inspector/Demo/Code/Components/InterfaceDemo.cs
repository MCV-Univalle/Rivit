using UnityEngine;
using Sisus.Attributes;
using Sisus.OdinSerializer;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	[RequireComponents(typeof(InterfaceDemoComponentA), typeof(InterfaceDemoComponentB))]
	public class InterfaceDemo : SerializedMonoBehaviour
	{
		[SerializeField]
		public IComponent IMonoBehaviour;

		[SerializeField]
		public IClass INotUnityObject;

		[SerializeField]
		public IComponentOrClass INothUnityObjectAndNot;
	}
}