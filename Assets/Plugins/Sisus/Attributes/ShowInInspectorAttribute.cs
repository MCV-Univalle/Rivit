using System;
using JetBrains.Annotations;

namespace Sisus.Attributes
{
	/// <summary>
	/// Attribute that specifies that its target should be shown in Power Inspector, even if it is not public,
	/// not serialized or otherwise wouldn't qualify to be shown by default.
	/// 
	/// Works on any fields, properties, methods and indexers.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method), MeansImplicitUse(ImplicitUseKindFlags.Access | ImplicitUseKindFlags.Assign)]
	public class ShowInInspectorAttribute : Attribute
	{

	}
}