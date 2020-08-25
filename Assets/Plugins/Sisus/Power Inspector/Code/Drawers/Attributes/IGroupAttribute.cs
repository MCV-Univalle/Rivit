using UnityEngine;
using JetBrains.Annotations;

namespace Sisus.Attributes
{
	/// <summary>
	/// Interface for attributes that are used for grouping members.
	/// </summary>
	public interface IGroupAttribute
	{
		/// <summary> Prefix label for group foldout. </summary>
		GUIContent Label
		{
			get;
		}

		/// <summary> Create new instance of drawer that implements ICustomGroupDrawer and should be used for drawing the group. </summary>
		/// <param name="parent"> Parent for drawer. </param>
		/// <param name="readOnly"> Determines whether or not members should be drawn greyed-out and non-editable. </param>
		/// <returns></returns>
		[NotNull]
		ICustomGroupDrawer CreateDrawer([CanBeNull]IParentDrawer parent, bool readOnly);
	}
}