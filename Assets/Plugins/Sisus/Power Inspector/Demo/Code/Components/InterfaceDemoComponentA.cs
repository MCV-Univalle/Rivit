using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class InterfaceDemoComponentA : MonoBehaviour, IComponent, IComponentOrClass
	{
		[ShowInInspector]
		public string Name
		{
			get { return "Component A"; }
		}
	}
}