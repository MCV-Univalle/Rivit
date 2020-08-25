using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo.Button
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ButtonDemo : MonoBehaviour
	{
		[PHeader("The <em>Button</em> attribute can be used to display any method as a button in the inspector.", "<em>Static</em> methods, <em>parameters</em> and <em>return values</em> are all supported.")]

		[HideInInspector]
		private string message = "";

		[ShowInInspector]
		public string Console
		{
			get
			{
				return message;
			}
		}

		[Button]
		public void Button()
		{
			message = "Hello from Button!";
		}

		[Button]
		public void AnotherButton(string message)
		{
			this.message = "Message from Another Button: \"" + message + "\"";
		}
	}
}