using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ShowIfDemo : MonoBehaviour
	{
		[PHeader("The <em>ShowIf</em> attribute can be used to show a class member only when a predicate statement is true.")]
		public bool addName = false;

		[ShowIf("addName", true)]
		public string firstName;

		[ShowIf("addName", true)]
		public bool addLastName = false;

		[ShowIf("addLastName && addLastName")]
		public string lastName;

		[ShowIf("addName")]
		public string FullName
		{
			get
			{
				if(addLastName && lastName.Length > 0)
				{
					return firstName + " " + lastName;
				}
				return firstName;
			}
		}

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("showif");
		}
	}
}