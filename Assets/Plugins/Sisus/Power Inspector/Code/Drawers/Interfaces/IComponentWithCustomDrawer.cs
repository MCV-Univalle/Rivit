using UnityEngine;

namespace Sisus
{
	public interface IComponentWithCustomDrawer
	{
		IComponentDrawer GetComponentDrawer(Component[] targets, IParentDrawer parent, IInspector inspector);
	}
}