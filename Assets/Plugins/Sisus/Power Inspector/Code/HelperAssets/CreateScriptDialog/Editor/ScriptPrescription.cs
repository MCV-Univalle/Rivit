using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace UnityEditor
{
	[Serializable]
	internal class ScriptPrescription
	{
		public string nameSpace = "";
		public string className = "";
		public string template = "";
		[NotNull]
		public string[] usingNamespaces = new string[0];
		[CanBeNull]
		public FunctionData[] functions;
		[NotNull]
		public readonly Dictionary<string, string> stringReplacements = new Dictionary<string, string> ();
	}
	
	internal struct FunctionData
	{
		public string attribute;
		public string prefix;
		public string name;
		public string returnType;
		public string returnDefault;
		[CanBeNull]
		public ParameterData[] parameters;
		public string comment;
		public bool include;
		
		public FunctionData(string headerName)
		{
			attribute = "";
			prefix = "";
			comment = headerName;
			name = null;
			returnType = null;
			returnDefault = null;
			parameters = null;
			include = false;
		}
	}
	
	internal struct ParameterData
	{
		public string name;
		public string type;
		
		public ParameterData(string name, string type)
		{
			this.name = name;
			this.type = type;
		}
	}
}