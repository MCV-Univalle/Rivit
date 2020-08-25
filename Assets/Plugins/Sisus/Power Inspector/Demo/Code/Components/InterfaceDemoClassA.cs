using Sisus.Attributes;

namespace Sisus.Demo
{
	public class InterfaceDemoClassA : IClass, IComponentOrClass
	{
		[ShowInInspector]
		public string Name
		{
			get { return "Class A"; }
		}
	}
}