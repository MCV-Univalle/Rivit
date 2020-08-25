using Sisus.Attributes;

namespace Sisus.Demo
{
	public class InterfaceDemoClassB : IClass, IComponentOrClass
	{
		[ShowInInspector]
		public string Name
		{
			get { return "Class B"; }
		}
	}
}