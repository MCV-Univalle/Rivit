using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Sisus;

namespace UnityEditor
{
	internal class ScriptBuilder : IDisposable
	{
		public const string Name = "PI.CreateScriptWizard/Name";
		public const string Namespace = "PI.CreateScriptWizard/Namespace";
		public const string SaveIn = "PI.CreateScriptWizard/SaveIn";
		public const string Template = "PI.CreateScriptWizard/Template";
		public const string AttachTo = "PI.CreateScriptWizard/AttachTo";
		public const string CreatedAtPath = "PI.CreateScriptWizard/CreatedAtPath";

		private TextWriter writer;
		private string text;
		private ScriptPrescription scriptPrescription;
		private string indentation;
		private int indentLevel;

		private bool curlyBracesOnNewLine;
		private bool addComments;
		private bool addCommentsAsSummary;
		private int wordWrapCommentsAfterCharacters = 35;
		private bool addUsedImplictly;
		private bool spaceAfterMethodName;
		private string newLine;

		private int IndentLevel
		{
			get
			{
				return indentLevel;
			}
			set
			{
				indentLevel = value;
				indentation = string.Empty;
				for(int i = 0; i < indentLevel; i++)
				{
					indentation += "	";
				}
			}
		}

		private string ClassName
		{
			get
			{
				if(scriptPrescription.className.Length > 0)
				{
					return scriptPrescription.className;
				}

				return "Example";
			}
		}
		
		public ScriptBuilder(ScriptPrescription scriptPrescription, bool setCurlyBracesOnNewLine, bool setAddComments, bool setAddCommentsAsSummary, int setWordWrapCommentsAfterCharacters, bool setAddUsedImplictly, bool setSpaceAfterMethodName, string setNewLine)
		{
			this.scriptPrescription = scriptPrescription;

			curlyBracesOnNewLine = setCurlyBracesOnNewLine;
			addComments = setAddComments;
			addCommentsAsSummary = setAddCommentsAsSummary;
			wordWrapCommentsAfterCharacters = setWordWrapCommentsAfterCharacters;
			addUsedImplictly = setAddUsedImplictly;
			spaceAfterMethodName = setSpaceAfterMethodName;
			newLine = string.Equals(setNewLine, "\r\n") || string.Equals(setNewLine, "\n") ? setNewLine : Environment.NewLine;
		}
		
		public override string ToString()
		{
			text = scriptPrescription.template;
			writer = new StringWriter();
			writer.NewLine = "\n";

			// Make sure all line endings are Unix (Mac OS X) format
			text = Regex.Replace(text, @"\r\n?", "\n");

			// Class Name
			text = text.Replace("$ClassName", ClassName);
			//text = text.Replace("$Namespace", Namespace);
			text = text.Replace("$NicifiedClassName", ObjectNames.NicifyVariableName(ClassName));

			// Other replacements
			foreach(var keyAndValue in scriptPrescription.stringReplacements)
			{
				text = text.Replace(keyAndValue.Key, keyAndValue.Value);
			}

			if(!addUsedImplictly)
			{
				text = text.Replace(", UsedImplicitly", "");
			}

			if(!curlyBracesOnNewLine)
			{
				text = text.Replace("\n{", " {");
				text = text.Replace("\n\t{", " {");
				text = text.Replace("\n\t\t{", " {");
			}

			if(!addComments)
			{
				for(int commentStart = text.IndexOf("//"); commentStart != -1; commentStart = text.IndexOf("//", commentStart))
				{
					int lineStart = text.LastIndexOf('\n', commentStart - 1);

					bool onlyWhitespaceBeforeComment = true;
					if(lineStart != -1)
					{
						for(int n = lineStart + 1; n < commentStart; n++)
						{
							if(!text[n].IsWhiteSpace())
							{
								onlyWhitespaceBeforeComment = false;
								break;
							}
						}
					}
					
					int commentEnd = text.IndexOf('\n', commentStart + 2);
					if(commentEnd == -1)
					{
						commentEnd = text.Length;
					}

					if(onlyWhitespaceBeforeComment)
					{
						commentStart = lineStart == -1 ? 0 : lineStart; //here!
					}

					#if DEV_MODE
					UnityEngine.Debug.Log("commentStart="+commentStart+ ", commentEnd="+ commentEnd+ ", onlyWhitespaceBeforeComment="+ onlyWhitespaceBeforeComment+ ", text:\n"+ text);
					#endif

					text = text.Substring(0, commentStart) + text.Substring(commentEnd);
				}
			}
			else if(!addCommentsAsSummary)
			{
				text = text.Replace("\t/// <summary>\n", "");
				text = text.Replace("\t/// </summary>\n", "");
				text = text.Replace("\t/// ", "\t// ");
			}

			// Functions
			// Find $Functions keyword including leading tabs
			var match = Regex.Match(text, @"(\t*)\$Functions");
			if(match.Success)
			{
				// Set indent level to number of tabs before $Functions keyword
				IndentLevel = match.Groups[1].Value.Length;
				bool hasFunctions = false;
				if(scriptPrescription.functions != null)
				{
					var includedFunctions = scriptPrescription.functions.Where(f => f.include).ToArray();
					for(int n = 0, lastIndex = includedFunctions.Length - 1; n <= lastIndex; n++)
					{
						var function = includedFunctions[n];
						WriteFunction(function);

						if(n != lastIndex)
						{
							WriteBlankLine();
						}
						hasFunctions = true;
					}

					// Replace $Functions keyword plus newline with generated functions text
					if(hasFunctions)
					{
						text = text.Replace(match.Value + "\n", writer.ToString());
					}
				}

				if(!hasFunctions)
				{
					text = text.Replace(match.Value + "\n", string.Empty);
				}
			}

			if(scriptPrescription.nameSpace.Length > 0)
			{
				var lines = text.Split(ArrayExtensions.TempCharArray('\n'), StringSplitOptions.None);
				for(int n = lines.Length - 1; n >= 0; n--)
				{
					lines[n] = string.Concat("\t", lines[n]);
				}
				
				text = string.Join(newLine, lines);
				text = string.Concat("namespace ", scriptPrescription.nameSpace, curlyBracesOnNewLine ? newLine : " ", "{", newLine, text, newLine, "}");
			}

			//add used namespaces to the beginning of the code
			int namespacesCount = scriptPrescription.usingNamespaces.Length;
			if(namespacesCount > 0)
			{
				text = string.Concat(newLine, text);

				for(int n = namespacesCount - 1; n >= 0; n--)
				{
					text = string.Concat("using ", scriptPrescription.usingNamespaces[n], ";", newLine, text);
				}
			}

			return text;
		}

		private void WriteBlankLine()
		{
			writer.WriteLine(indentation);
		}

		private void WriteComment(string comment, bool asSummary)
		{
			if(comment.Length < 3)
			{
				return;
			}
						
			string prefix;
			if(asSummary)
			{
				writer.WriteLine(indentation + "/// <summary>");
				prefix = indentation + "/// ";
			}
			else
			{
				prefix = indentation + "// ";
			}

			int index = 0;
			do
			{
				if(comment.Length <= index + wordWrapCommentsAfterCharacters)
				{
					writer.WriteLine(prefix + comment.Substring(index));
					break;
				}

				int wrapIndex = comment.IndexOf(' ', index + wordWrapCommentsAfterCharacters);
				if(wrapIndex < 0)
				{
					writer.WriteLine(prefix + comment.Substring(index));
					break;
				}
					
				writer.WriteLine(prefix + comment.Substring(index, wrapIndex - index));
				index = wrapIndex + 1;
			}
			while(true);

			if(asSummary)
			{
				writer.WriteLine(indentation + "/// </summary>");
			}
		}

		private void WriteAddUsedImplicitly()
		{
			writer.WriteLine(indentation + "[UsedImplicitly]");
		}
		
		private void WriteFunction(FunctionData function)
		{
			if(addComments)
			{
				WriteComment(function.comment, addCommentsAsSummary);
			}

			if(addUsedImplictly)
			{
				WriteAddUsedImplicitly();
			}

			// Function header
			string paramString = "";
			for(int i = 0; i < function.parameters.Length; i++)
			{
				paramString += function.parameters[i].type + " " + function.parameters[i].name;
				if(i < function.parameters.Length - 1)
				{
					paramString += ", ";
				}
			}
			
			string returnTypeString = (function.returnType == null ? "void " : function.returnType + " ");

			if(!string.IsNullOrEmpty(function.attribute))
			{
				writer.WriteLine(string.Concat(indentation, function.attribute));
			}

			string lineContent = indentation + function.prefix + returnTypeString + function.name;
			
			if(spaceAfterMethodName)
			{
				lineContent += " ";
			}
			lineContent += "(" + paramString + ")";
			if(curlyBracesOnNewLine)
			{
				lineContent += writer.NewLine + indentation + "{";
			}
			else
			{
				lineContent += " {";
			}
			writer.WriteLine(lineContent);

			// Function content
			IndentLevel++;
			string functionContentString = (function.returnType == null ? string.Empty : function.returnDefault + ";");
			writer.WriteLine(indentation + functionContentString);
			IndentLevel--;
			writer.WriteLine(indentation + "}");
		}

		public void Dispose()
		{
			if(writer != null)
			{
				writer.Dispose();
			}

			scriptPrescription = null;
		}
	}
}