using System;
using UnityEngine.Serialization;

[Serializable]
public class NewScriptWindowSettings
{
	public bool curlyBracesOnNewLine;
	[FormerlySerializedAs("addMethodComments")]
	public bool addComments = true;
	public bool addCommentsAsSummary = true;
	public int wordWrapCommentsAfterCharacters = 100;
	public bool addUsedImplicitly;
	public bool spaceAfterMethodName = true;
	public string newLine = "\r\n";
	public string[] usingNamespaceOptions =
	{
		"System",
		"System.Collections",
		"System.Collections.Generic",
		"System.Linq",
		"UnityEngine",
		"UnityEditor",
		"Object = UnityEngine.Object",
		"JetBrains.Annotations"
		//"Debug = UnityEngine.Debug"
	};
}