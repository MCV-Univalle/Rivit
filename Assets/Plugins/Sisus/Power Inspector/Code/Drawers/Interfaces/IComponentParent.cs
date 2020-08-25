using UnityEngine;

namespace Sisus
{
	public interface IComponentParent : IParentDrawer
	{
		/// <summary>
		/// Deletes the member drawer and the value
		/// that it represents in the reorderable parent.
		/// </summary>
		/// <param name="delete"> The member to delete. </param>
		void DeleteMember(IDrawer delete);
	}
}