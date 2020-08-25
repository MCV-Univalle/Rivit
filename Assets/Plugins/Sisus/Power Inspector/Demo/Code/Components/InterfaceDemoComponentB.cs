using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class InterfaceDemoComponentB : MonoBehaviour, IComponent, IComponentOrClass
	{
		[ShowInInspector]
		public string Name
		{
			get { return "Component B"; }
		}
	}
}