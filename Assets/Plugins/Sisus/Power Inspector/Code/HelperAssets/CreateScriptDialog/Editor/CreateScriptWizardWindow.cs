//#define DISABLE_SCRIPT_WIZARD_MENU_ITEM

#define DEBUG_SELECT

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.CodeDom.Compiler;
using Object = UnityEngine.Object;

namespace Sisus
{
	public class CreateScriptWizardWindow : EditorWindow
	{
		private const int ButtonWidth = 120;
		private const int LabelWidth = 85;
		private const string TemplateDirectoryName = "SmartScriptTemplates";
		private const string ResourcesTemplatePath = "Resources/SmartScriptTemplates";
		private const string MonoBehaviourName = "MonoBehaviour";
		private const string PlainClassName = "Plain Class";
		private const string CustomEditorClassName = "Editor";
		private const string TempEditorClassPrefix = "E:";
		private const string NoTemplateString = "No Template Found";

		// char array can't be const for compiler reasons but this should still be treated as such.
		private static readonly char[] InvalidPathChars = { '<', '>', ':', '"', '|', '?', '*', (char)0 };
		private static readonly char[] PathSeparatorChars = { '/', '\\' };

		private static Styles styles;

		private ScriptPrescription scriptPrescription;
		[CanBeNull]
		private string baseClass;
		private string customEditorTargetClassName = "";
		private bool isEditorClass;
		private bool isCustomEditor;
		private bool focusTextFieldNow = true;
		private GameObject gameObjectToAddTo;
		private string directory = "";
		private Vector2 previewScroll;
		private Vector2 optionsScroll;
		private bool clearKeyboardControl;

		private int templateIndex;
		private string[] templateNames;

		private bool curlyBracesOnNewLine;
		private bool addComments = true;
		private bool addCommentsAsSummary = true;
		private int wordWrapCommentsAfterCharacters = 100;
		private bool addUsedImplicitly;
		private bool spaceAfterMethodName = true;
		private string newLine = "\r\n";
		private string[] namespacesList;

		private bool namespaceIsInvalid;
		private bool classNameIsInvalid = true;
		private bool classAlreadyExists;
		private bool scriptAtPathAlreadyExists;
		private bool customEditorTargetClassDoesNotExist;
		private bool customEditortargetClassIsNotValidType;
		private bool invalidTargetPath;
		private bool invalidTargetPathForEditorScript;
		private bool canCreate;

		private InspectorPreferences preferencesAsset;
		
		private string codePreview;
		private GUIContent codePreviewGUIContent = new GUIContent("");

		private static readonly List<string> reusableStringList = new List<string>(10);

		[NonSerialized]
		private bool setupDone;

		private NewScriptWindowSettings Settings
		{
			get
			{
				return preferencesAsset.createScriptWizard;
			}
		}

		private string GetBuiltinTemplateFullPath()
		{
			return Path.Combine(EditorApplication.applicationContentsPath, ResourcesTemplatePath);
		}

		private string GetCustomTemplateFullPath()
		{
			var templateDirs = Directory.GetDirectories(Application.dataPath, TemplateDirectoryName, SearchOption.AllDirectories);
			if(templateDirs.Length > 0)
			{
				return templateDirs[0];
			}

			Debug.LogWarning("CreateScriptWizardWindow Failed to locate templates directory \""+TemplateDirectoryName+"\" inside \""+ Application.dataPath+"\"");
			return "";
		}

		private void UpdateTemplateNamesAndTemplate()
		{
			// Remember old selected template name
			string oldSelectedTemplateName = null;
			if(templateNames != null && templateNames.Length > 0)
			{
				oldSelectedTemplateName = templateNames[templateIndex];
			}

			// Get new template names
			templateNames = GetTemplateNames();

			// Select template
			if(templateNames.Length == 0)
			{
				scriptPrescription.template = NoTemplateString;
				baseClass = null;
			}
			else
			{
				if(oldSelectedTemplateName != null && templateNames.Contains(oldSelectedTemplateName))
				{
					templateIndex = templateNames.ToList().IndexOf(oldSelectedTemplateName);
				}
				else
				{
					templateIndex = 0;
				}

				scriptPrescription.template = GetTemplate(templateNames[templateIndex]);
			}

			HandleBaseClass();
		}

		private void AutomaticHandlingOnChangeTemplate()
		{
			// Add or remove "Editor" from directory path
			if(isEditorClass)
			{
				if(!FileUtility.IsEditorPath(directory))
				{
					if(string.Equals(preferencesAsset.defaultScriptPath, directory))
					{
						directory = preferencesAsset.defaultEditorScriptPath;
					}
					else
					{
						directory = FileUtility.GetChildDirectory(directory, "Editor");
					}
				}
			}
			else if(FileUtility.IsEditorPath(directory))
			{
				directory = FileUtility.MakeNonEditorPath(directory);
			}

			// Move keyboard focus to relevant field
			if(isCustomEditor)
			{
				focusTextFieldNow = true;
			}
		}

		private string GetBaseClass(string templateContent)
		{
			string firstLine = templateContent.Substring(0, templateContent.IndexOf('\n'));
			if(firstLine.Contains("BASECLASS"))
			{
				string baseClass = firstLine.Substring(10).Trim();
				if(baseClass.Length > 0)
				{
					return baseClass;
				}
			}
			return null;
		}

		private bool GetFunctionIsIncluded(string baseClassName, string functionName, bool includeByDefault)
		{
			string prefName = "PI.FunctionData_" + (baseClassName != null ? baseClassName + "_" : string.Empty) + functionName;
			return EditorPrefs.GetBool(prefName, includeByDefault);
		}

		private void SetFunctionIsIncluded(string baseClassName, string functionName, bool include)
		{
			string prefName = "PI.FunctionData_" + (baseClassName != null ? baseClassName + "_" : string.Empty) + functionName;
			EditorPrefs.SetBool(prefName, include);
			UpdateCodePreview();
		}

		private bool GetNamespaceIsIncluded(string nameSpace, bool includeByDefault)
		{
			string prefName = "PI.Namespace_" + nameSpace;
			return EditorPrefs.GetBool(prefName, includeByDefault);
		}

		private void SetNamespaceIsIncluded(string nameSpace, bool include)
		{
			string prefName = "PI.Namespace_" + nameSpace;
			EditorPrefs.SetBool(prefName, include);
		}

		private void HandleBaseClass()
		{
			if(templateNames.Length == 0)
			{
				baseClass = null;
				return;
			}

			// Get base class
			baseClass = GetBaseClass(scriptPrescription.template);

			// If base class was found, strip first line from template
			if(baseClass != null)
			{
				scriptPrescription.template = scriptPrescription.template.Substring(scriptPrescription.template.IndexOf("\n") + 1);
			}

			isEditorClass = IsEditorClass(baseClass);
			SetIsCustomEditor(baseClass == CustomEditorClassName);
			scriptPrescription.stringReplacements.Clear();

			// Try to find function file first in custom templates folder and then in built-in
			string functionDataFilePath = Path.Combine(GetCustomTemplateFullPath(), baseClass + ".functions.txt");
			if(!File.Exists(functionDataFilePath))
			{
				functionDataFilePath = Path.Combine(GetBuiltinTemplateFullPath(), baseClass + ".functions.txt");
			}

			if(!File.Exists(functionDataFilePath))
			{
				scriptPrescription.functions = null;
			}
			else
			{
				var reader = new StreamReader(functionDataFilePath);
				var functionList = new List<FunctionData>();
				int lineNr = 1;
				while(!reader.EndOfStream)
				{
					string functionLine = reader.ReadLine();
					string functionLineWhole = functionLine;
					try
					{
						if(string.Equals(functionLine.Substring(0, 7), "header ", StringComparison.OrdinalIgnoreCase))
						{
							functionList.Add(new FunctionData(functionLine.Substring(7)));
							continue;
						}

						var function = new FunctionData();

						bool defaultInclude = false;
						if(string.Equals(functionLine.Substring(0, 8), "DEFAULT "))
						{
							defaultInclude = true;
							functionLine = functionLine.Substring(8);
						}

						if(functionLine.StartsWith("[", StringComparison.Ordinal))
						{
							int stop = functionLine.LastIndexOf(']', 0);
							if(stop > 0)
							{
								function.attribute = functionLine.Substring(0, stop+1);
							}
						}

						bool repeat;
						do
						{
							repeat = false;
							if(functionLine.StartsWith("override ", StringComparison.Ordinal))
							{
								function.prefix += "override ";
								functionLine = functionLine.Substring(9);
								repeat = true;
							}

							if(functionLine.StartsWith("abstract ", StringComparison.Ordinal))
							{
								function.prefix += "abstract ";
								functionLine = functionLine.Substring(9);
								repeat = true;
							}

							if(functionLine.StartsWith("virtual ", StringComparison.Ordinal))
							{
								function.prefix += "virtual ";
								functionLine = functionLine.Substring(8);
								repeat = true;
							}

							if(functionLine.StartsWith("private ", StringComparison.Ordinal))
							{
								function.prefix += "private ";
								functionLine = functionLine.Substring(8);
								repeat = true;
							}

							if(functionLine.StartsWith("protected ", StringComparison.Ordinal))
							{
								function.prefix += "protected ";
								functionLine = functionLine.Substring(10);
								repeat = true;
							}

							if(functionLine.StartsWith("public ", StringComparison.Ordinal))
							{
								function.prefix += "public ";
								functionLine = functionLine.Substring(7);
								repeat = true;
							}

							if(functionLine.StartsWith("internal ", StringComparison.Ordinal))
							{
								function.prefix += "internal ";
								functionLine = functionLine.Substring(9);
								repeat = true;
							}
						}
						while(repeat);

						string returnTypeString = GetStringUntilSeperator(ref functionLine, " ");
						function.returnType = (string.Equals(returnTypeString, "void") ? null : returnTypeString);
						function.name = GetStringUntilSeperator(ref functionLine, "(");
						string parameterString = GetStringUntilSeperator(ref functionLine, ")");
						if(function.returnType != null)
						{
							function.returnDefault = GetStringUntilSeperator(ref functionLine, ";");
						}

						function.comment = functionLine;

						var parameterStrings = parameterString.Split(ArrayExtensions.TempCharArray(','), StringSplitOptions.RemoveEmptyEntries);
						var parameterList = new List<UnityEditor.ParameterData>();
						for(int i = 0; i < parameterStrings.Length; i++)
						{
							var paramSplit = parameterStrings[i].Trim().Split(' ');
							parameterList.Add(new UnityEditor.ParameterData(paramSplit[1], paramSplit[0]));
						}
						function.parameters = parameterList.ToArray();

						function.include = GetFunctionIsIncluded(baseClass, function.name, defaultInclude);

						functionList.Add(function);
					}
					catch(Exception e)
					{
						Debug.LogWarning("Malformed function line: \"" + functionLineWhole + "\"\n  at " + functionDataFilePath + ":" + lineNr + "\n" + e);
					}
					lineNr++;
				}
				scriptPrescription.functions = functionList.ToArray();

			}
		}

		private void SetIsCustomEditor(bool setIsCustomEditor)
		{
			if(isCustomEditor != setIsCustomEditor)
			{
				isCustomEditor = setIsCustomEditor;
				customEditorTargetClassDoesNotExist = CustomEditorTargetClassDoesNotExist();
				customEditortargetClassIsNotValidType = CustomEditorTargetClassIsNotValidType();
			}
		}

		private string GetStringUntilSeperator(ref string source, string sep)
		{
			int index = source.IndexOf(sep);
			string result = source.Substring(0, index).Trim();
			source = source.Substring(index + sep.Length).Trim(' ');
			return result;
		}

		private string GetTemplate(string nameWithoutExtension)
		{
			string path = Path.Combine(GetCustomTemplateFullPath(), nameWithoutExtension + ".cs.txt");
			if(File.Exists(path))
			{
				return File.ReadAllText(path);
			}

			path = Path.Combine(GetBuiltinTemplateFullPath(), nameWithoutExtension + ".cs.txt");
			if(File.Exists(path))
			{
				return File.ReadAllText(path);
			}

			return NoTemplateString;
		}

		private string GetTemplateName()
		{
			if(templateNames.Length == 0)
			{
				return NoTemplateString;
			}

			return templateNames[templateIndex];
		}

		// Custom comparer to sort templates alphabetically,
		// but put MonoBehaviour and Plain Class as the first two
		private class TemplateNameComparer : IComparer<string>
		{
			private int GetRank(string s)
			{
				if(string.Equals(s, MonoBehaviourName))
				{
					return 0;
				}

				if(string.Equals(s, PlainClassName))
				{
					return 1;
				}

				if(s.StartsWith(TempEditorClassPrefix))
				{
					return 100;
				}

				return 2;
			}

			public int Compare(string x, string y)
			{
				int rankX = GetRank(x);
				int rankY = GetRank(y);
				if(rankX == rankY)
				{
					return x.CompareTo(y);
				}
				return rankX.CompareTo(rankY);
			}
		}

		private string[] GetTemplateNames()
		{
			var templates = new List<string>();

			// Get all file names of custom templates
			if(Directory.Exists(GetCustomTemplateFullPath()))
			{
				templates.AddRange(Directory.GetFiles(GetCustomTemplateFullPath()));
			}

			// Get all file names of built-in templates
			if(Directory.Exists(GetBuiltinTemplateFullPath()))
			{
				templates.AddRange(Directory.GetFiles(GetBuiltinTemplateFullPath()));
			}

			if(templates.Count == 0)
			{
				return new string[0];
			}

			// Filter and clean up list
			templates = templates
				.Distinct()
				.Where(f => (f.EndsWith(".cs.txt")))
				.Select(f => Path.GetFileNameWithoutExtension(f.Substring(0, f.Length - 4)))
				.ToList();

			// Determine which scripts have editor class base class
			for(int i = 0; i < templates.Count; i++)
			{
				string templateContent = GetTemplate(templates[i]);
				if(IsEditorClass(GetBaseClass(templateContent)))
				{
					templates[i] = TempEditorClassPrefix + templates[i];
				}
			}

			// Order list
			templates = templates.OrderBy(f => f, new TemplateNameComparer()).ToList();

			// Insert separator before first editor script template
			bool inserted = false;
			for(int i = 0; i < templates.Count; i++)
			{
				if(templates[i].StartsWith(TempEditorClassPrefix))
				{
					templates[i] = templates[i].Substring(TempEditorClassPrefix.Length);
					if(!inserted)
					{
						templates.Insert(i, string.Empty);
						inserted = true;
					}
				}
			}
			
			return templates.ToArray();
		}
		
		#if !DISABLE_POWER_INSPECTOR_MENU_ITEMS && !DISABLE_SCRIPT_WIZARD_MENU_ITEM
		[MenuItem(PowerInspectorMenuItemPaths.CreateScriptWizardFromCreateMenu, false, PowerInspectorMenuItemPaths.CreateScriptWizardFromCreateMenuPrority), UsedImplicitly]
		private static void OpenFromAssetsMenu()
		{
			Init();
		}
		#endif

		private static void Init()
		{
			var window = GetWindow<CreateScriptWizardWindow>(true, "Create Script Wizard");
			var pos = window.position;
			pos.x = Screen.currentResolution.width * 0.5f - pos.width * 0.5f;
			pos.y = Screen.currentResolution.height * 0.5f - pos.height * 0.5f;
			window.position = pos;
			window.Repaint();
		}

		public CreateScriptWizardWindow()
		{
			// Large initial size
			position = new Rect(50, 50, 770, 500);
			// But allow to scale down to smaller size
			minSize = new Vector2(550, 480);

			scriptPrescription = new ScriptPrescription();
		}

		[UsedImplicitly]

		private void OnFocus()
		{
			PowerInspectorDocumentation.ShowFeatureIfWindowOpen("create-script-wizard");
		}

		[UsedImplicitly]
		private void OnEnable()
		{
			Setup();
		}

		private void Setup()
		{
			setupDone = true;

			UpdateTemplateNamesAndTemplate();
			OnSelectionChange();

			preferencesAsset = InspectorUtility.Preferences;

			if(preferencesAsset != null)
			{
				preferencesAsset.onSettingsChanged += OnSettingsChanged;
			}
			#if DEV_MODE
			else
			{
				Debug.LogWarning("CreateScriptWizardWindow.OnEnable - failed to find Inspector Preferences Asset!");
			}
			#endif

			LoadSettings();

			if(EditorPrefs.HasKey(ScriptBuilder.Name))
			{
				SetClassName(EditorPrefs.GetString(ScriptBuilder.Name));
				EditorPrefs.DeleteKey(ScriptBuilder.Name);
			}

			if(EditorPrefs.HasKey(ScriptBuilder.Namespace))
			{
				SetNamespace(EditorPrefs.GetString(ScriptBuilder.Namespace));
				EditorPrefs.DeleteKey(ScriptBuilder.Namespace);
			}

			var template = EditorPrefs.GetString(ScriptBuilder.Template, "");
			if(template.Length > 0)
			{
				templateIndex = Array.IndexOf(templateNames, template);
				if(templateIndex == -1)
				{
					templateIndex = 0;
				}
			}

			int attachToById = EditorPrefs.GetInt(ScriptBuilder.AttachTo, 0);
			if(attachToById != 0)
			{
				gameObjectToAddTo = EditorUtility.InstanceIDToObject(attachToById) as GameObject;
			}

			directory = EditorPrefs.GetString(ScriptBuilder.SaveIn, "");

			MakeSureAllMandatoryUsingAreIncluded();
			UpdateIncludedNamespaces();
			UpdateCodePreview();
		}

		[UnityEditor.Callbacks.DidReloadScripts, UsedImplicitly]
		private static void OnScriptsReloaded()
		{
			if(EditorPrefs.HasKey(ScriptBuilder.CreatedAtPath))
			{
				var createdScriptPath = EditorPrefs.GetString(ScriptBuilder.CreatedAtPath, "");
				if(createdScriptPath.Length > 0)
				{
					var createdScript = AssetDatabase.LoadAssetAtPath<MonoScript>(createdScriptPath);
					if(createdScript != null)
					{
						#if DEV_MODE && DEBUG_SELECT
						Debug.Log("Select("+StringUtils.ToString(createdScript)+")");
						#endif
						Selection.activeObject = createdScript;
						AssetDatabase.OpenAsset(createdScript, 0);
					}
				}
				EditorPrefs.DeleteKey(ScriptBuilder.CreatedAtPath);
			}
		}

		private void UpdateIncludedNamespaces()
		{
			if(preferencesAsset != null)
			{
				var list = reusableStringList;
				for(int n = 0; n < namespacesList.Length; n++)
				{
					var item = namespacesList[n];
					if(GetNamespaceIsIncluded(item, Settings.usingNamespaceOptions[n].StartsWith("*", StringComparison.Ordinal)))
					{
						list.Add(item);
					}
				}

				if(scriptPrescription.usingNamespaces == null || !scriptPrescription.usingNamespaces.ContentsMatch(list))
				{
					scriptPrescription.usingNamespaces = list.ToArray();
					list.Clear();
					UpdateCodePreview();
				}
				else
				{
					list.Clear();
				}				
			}
		}

		private void LoadSettings()
		{
			if(preferencesAsset != null)
			{
				var settings = Settings;
				curlyBracesOnNewLine = settings.curlyBracesOnNewLine;
				addComments = settings.addComments;
				addCommentsAsSummary = settings.addCommentsAsSummary;
				wordWrapCommentsAfterCharacters = settings.wordWrapCommentsAfterCharacters;
				addUsedImplicitly = settings.addUsedImplicitly;
				spaceAfterMethodName = settings.spaceAfterMethodName;
				newLine = settings.newLine;
				namespacesList = settings.usingNamespaceOptions;
				for(int n = namespacesList.Length - 1; n >= 0; n--)
				{
					if(namespacesList[n].StartsWith("*", StringComparison.Ordinal))
					{
						namespacesList[n] = namespacesList[n].Substring(1);
					}
				}
			}
		}

		[UsedImplicitly]
		private void OnGUI()
		{
			if(!setupDone)
			{
				Setup();
			}

			if(styles == null)
			{
				styles = new Styles();
			}
			
			float labelWidthWas = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 85f;
			
			if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && canCreate)
			{
				Create();
			}

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(10);

				PreviewGUI();
				GUILayout.Space(10);

				EditorGUILayout.BeginVertical();
				{
					OptionsGUI();

					GUILayout.Space(10);

					CreateAndCancelButtonsGUI();
				}
				EditorGUILayout.EndVertical();

				GUILayout.Space(10);
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);

			EditorGUIUtility.labelWidth = labelWidthWas;

			// Clear keyboard focus if clicking a random place inside the dialog, or if ClearKeyboardControl flag is set.
			if(clearKeyboardControl || (Event.current.type == EventType.MouseDown && Event.current.button == 0))
			{
				KeyboardControlUtility.KeyboardControl = 0;
				clearKeyboardControl = false;
				Repaint();
			}

			if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				var rect = position;
				rect.x = 0f;
				rect.y = 0f;
				if(rect.Contains(Event.current.mousePosition))
				{
					DrawGUI.Use(Event.current);
					var menu = Menu.Create();
					menu.Add("Edit Preferences", ShowPreferences);
					ContextMenuUtility.Open(menu, null);
				}
			}
		}

		private bool CanCreate()
		{
			return scriptPrescription.className.Length > 0 &&
				!scriptAtPathAlreadyExists &&
				!classAlreadyExists &&
				!classNameIsInvalid &&
				!customEditorTargetClassDoesNotExist &&
				!customEditortargetClassIsNotValidType &&
				!invalidTargetPath &&
				!invalidTargetPathForEditorScript &&
				!namespaceIsInvalid;
		}

		private void Create()
		{
			EditorPrefs.SetString(ScriptBuilder.CreatedAtPath, TargetPath());

			CreateScript();

			if(CanAddComponent())
			{
				var addScriptMethod = typeof(InternalEditorUtility).GetMethod("AddScriptComponentUncheckedUndoable", BindingFlags.Static | BindingFlags.NonPublic);
				addScriptMethod.Invoke(null, new Object[] { gameObjectToAddTo, AssetDatabase.LoadAssetAtPath(TargetPath(), typeof(MonoScript)) as MonoScript });
			}

			Close();
			GUIUtility.ExitGUI();
		}
		
		private string GetFullClassName()
		{
			if(scriptPrescription.nameSpace.Length > 0)
			{
				return string.Concat(scriptPrescription.nameSpace, ".", scriptPrescription.className);
			}
			return scriptPrescription.className;
		}

		private void CreateAndCancelButtonsGUI()
		{
			// Create string to tell the user what the problem is
			string blockReason = string.Empty;
			if(!canCreate && scriptPrescription.className.Length > 0)
			{
				if(scriptAtPathAlreadyExists)
				{
					blockReason = "A script called \"" + GetFullClassName() + "\" already exists at that path.";
				}
				else if(classAlreadyExists)
				{
					blockReason = "A class called \"" + GetFullClassName() + "\" already exists.";
				}
				else if(classNameIsInvalid)
				{
					blockReason = "The script name may only consist of a-z, A-Z, 0-9, _.";
				}
				else if(customEditorTargetClassDoesNotExist)
				{
					if(customEditorTargetClassName.Length == 0)
					{
						blockReason = "Fill in the script component to make an editor for.";
					}
					else
					{
						blockReason = "A class called \"" + customEditorTargetClassName + "\" could not be found.";
					}
				}
				else if(customEditortargetClassIsNotValidType)
				{
					blockReason = "The class \"" + customEditorTargetClassName + "\" is not of type UnityEngine.Object.";
				}
				else if(invalidTargetPath)
				{
					blockReason = "The folder path contains invalid characters.";
				}
				else if(invalidTargetPathForEditorScript)
				{
					blockReason = "Editor scripts should be stored in a folder called Editor.";
				}
			}

			// Warning about why the script can't be created
			if(blockReason.Length > 0)
			{
				styles.warningContent.text = blockReason;
				GUILayout.BeginHorizontal();
				{
					GUI.color = Color.red;
					GUILayout.Label(styles.warningContent, EditorStyles.wordWrappedMiniLabel);
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();
			}

			// Cancel and create buttons
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if(GUILayout.Button("Cancel", GUILayout.Width(ButtonWidth)))
				{
					Close();
					GUIUtility.ExitGUI();
				}

				bool guiEnabledTemp = GUI.enabled;
				GUI.enabled = canCreate;
				if(GUILayout.Button(GetCreateButtonText(), GUILayout.Width(ButtonWidth)))
				{
					Create();
				}
				GUI.enabled = guiEnabledTemp;
			}
			GUILayout.EndHorizontal();
		}

		private bool CanAddComponent()
		{
			return (gameObjectToAddTo != null && baseClass == MonoBehaviourName);
		}

		private void OptionsGUI()
		{
			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			{
				NamespaceGUI();
				
				GUILayout.Space(10);
				
				NameGUI();

				GUILayout.Space(10);

				TargetPathGUI();

				GUILayout.Space(20);

				TemplateSelectionGUI();

				if(string.Equals(GetTemplateName(), MonoBehaviourName))
				{
					GUILayout.Space(10);
					AttachToGUI();
				}

				if(isCustomEditor)
				{
					GUILayout.Space(10);
					CustomEditorTargetClassNameGUI();
				}

				GUILayout.Space(10);

				UsingGUI();
				FunctionsGUI();
			}
			EditorGUILayout.EndVertical();
		}

		private bool FunctionHeader(string header, bool expandedByDefault)
		{
			GUILayout.Space(5);
			bool expanded = GetFunctionIsIncluded(baseClass, header, expandedByDefault);
			bool expandedNew = GUILayout.Toggle(expanded, header, EditorStyles.foldout);
			if(expandedNew != expanded)
			{
				SetFunctionIsIncluded(baseClass, header, expandedNew);
			}

			return expandedNew;
		}

		private void UsingGUI()
		{
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Using", GUILayout.Width(LabelWidth - 4));

				if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
				{
					var lastRect = GUILayoutUtility.GetLastRect();
					if(lastRect.Contains(Event.current.mousePosition))
					{
						DrawGUI.Use(Event.current);
						var menu = Menu.Create();
						menu.Add("Edit Using List", ShowUsingListInPreferences);
						ContextMenuUtility.Open(menu, null);
					}
				}

				EditorGUILayout.BeginVertical(styles.loweredBox);
				{
					for(int i = 0, count = namespacesList.Length; i < count; i++)
					{
						var item = namespacesList[i];
						int foundIndex = Array.IndexOf(scriptPrescription.usingNamespaces, item);
						var include = foundIndex != -1;
						var toggleRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.toggle);

						bool set;
						if(CanRemoveUsing(item))
						{
							set = GUI.Toggle(toggleRect, include, item);
						}
						else
						{
							GUI.enabled = false;
							GUI.Toggle(toggleRect, include, item);
							set = true;
							GUI.enabled = true;
						}

						if(set != include)
						{
							EditorGUIUtility.editingTextField = false;
							SetNamespaceIsIncluded(item, set);
							UpdateIncludedNamespaces();
							UpdateCodePreview();
						}
					}
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}

		private bool CanRemoveUsing(string usingNamespace)
		{
			switch(usingNamespace)
			{
				case "JetBrains.Annotations":
					return !AnnotationsNamespaceRequired();
				case "UnityEngine":
					switch(baseClass)
					{
						case "MonoBehaviour":
						case "ScriptableObject":
							return false;
						case "AssetPostprocessor":
							return !GetFunctionIsIncluded("AssetPostprocessor", "OnPostProcessTexture", false);
						default:
							return true;
					}
				case "UnityEditor":
					switch(baseClass)
					{
						case "AssetPostprocessor":
						case "Editor":
						case "EditorWindow":
						case "ScriptableWizard":
							return false;
					}
					switch(scriptPrescription.template)
					{
						case "Menu Item":
							return false;
						default:
							return true;
					}
				default:
					return true;
			}
		}

		private bool AnnotationsNamespaceRequired()
		{
			if(!addUsedImplicitly)
			{
				return false;
			}

			switch(templateNames[templateIndex])
			{
				case "ScriptableObject":
				case "MonoBehaviour":
				case "Asset Postprocessor":
				case "Custom Editor":
				case "Editor Window":
					for(int n = scriptPrescription.functions.Length - 1; n >= 0; n--)
					{
						if(scriptPrescription.functions[n].include)
						{
							return true;
						}
					}
					return false;
				case "Plain Class":
					return false;
				case "Menu Item":
					return true;
				default:
					return false;
			}
		}

		private void FunctionsGUI()
		{
			if(scriptPrescription.functions == null)
			{
				GUILayout.FlexibleSpace();
				return;
			}

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Functions", GUILayout.Width(LabelWidth - 4));

				if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
				{
					var lastRect = GUILayoutUtility.GetLastRect();
					if(lastRect.Contains(Event.current.mousePosition))
					{
						DrawGUI.Use(Event.current);
						var menu = Menu.Create();
						menu.Add("Edit Functions", ShowCurrentTemplateFunctionsInExplorer);
						ContextMenuUtility.Open(menu, null);
					}
				}

				EditorGUILayout.BeginVertical(styles.loweredBox);
				{
					optionsScroll = EditorGUILayout.BeginScrollView(optionsScroll);
					{
						bool expanded = FunctionHeader("General", true);

						for(int i = 0, count = scriptPrescription.functions.Length; i < count; i++)
						{
							var func = scriptPrescription.functions[i];

							if(func.name == null)
							{
								expanded = FunctionHeader(func.comment, false);
							}
							else if(expanded)
							{
								var toggleRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.toggle);
								toggleRect.x += 15;
								toggleRect.width -= 15;
								bool include = GUI.Toggle(toggleRect, func.include, new GUIContent(func.name, func.comment));
								if(include != func.include)
								{
									EditorGUIUtility.editingTextField = false;
									scriptPrescription.functions[i].include = include;
									SetFunctionIsIncluded(baseClass, func.name, include);
									UpdateCodePreview();
								}
							}
						}
					}
					EditorGUILayout.EndScrollView();
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void MakeSureAllMandatoryUsingAreIncluded()
		{
			bool changed = false;

			if(addUsedImplicitly)
			{
				if(Array.IndexOf(scriptPrescription.usingNamespaces, "JetBrains.Annotations") == -1)
				{
					SetNamespaceIsIncluded("JetBrains.Annotations", true);
					changed = true;
				}
			}

			bool unityEngineRequired = false;
			bool unityEditorRequired = false;

			switch(baseClass)
			{
				case "MonoBehaviour":
				case "ScriptableObject":
					unityEngineRequired = true;
					break;
				case "AssetPostprocessor":
				case "CustomEditor":
				case "EditorWindow":
					unityEditorRequired = true;
					break;
			}

			switch(scriptPrescription.template)
			{
				case "Menu Item":
					unityEditorRequired = true;
					break;
			}

			if(unityEngineRequired && Array.IndexOf(scriptPrescription.usingNamespaces, "UnityEngine") == -1)
			{
				SetNamespaceIsIncluded("UnityEngine", true);
				changed = true;
			}

			if(unityEditorRequired && Array.IndexOf(scriptPrescription.usingNamespaces, "UnityEditor") == -1)
			{
				SetNamespaceIsIncluded("UnityEditor", true);
				changed = true;
			}

			if(changed)
			{
				UpdateIncludedNamespaces();
			}
		}

		private void AttachToGUI()
		{
			GUILayout.BeginHorizontal();
			{
				gameObjectToAddTo = EditorGUILayout.ObjectField("Add To", gameObjectToAddTo, typeof(GameObject), true) as GameObject;

				if(ClearButton())
				{
					gameObjectToAddTo = null;
				}
			}
			GUILayout.EndHorizontal();

			HelpField("Click a GameObject or Prefab to select.");
		}

		private void SetClassNameBasedOnTargetClassName()
		{
			if(customEditorTargetClassName.Length == 0)
			{
				SetClassName(string.Empty);
			}
			else
			{
				SetClassName(customEditorTargetClassName + "Editor");
			}
		}
		
		private void CustomEditorTargetClassNameGUI()
		{
			GUI.SetNextControlName("CustomEditorTargetClassNameField");

			if(customEditorTargetClassDoesNotExist)
			{
				GUI.color = Color.red;
			}
			string newName = EditorGUILayout.TextField("Editor for", customEditorTargetClassName);
			GUI.color = Color.white;
			scriptPrescription.stringReplacements["$TargetClassName"] = newName;
			SetCustomEditorTargetClassName(newName);

			if(focusTextFieldNow && Event.current.type == EventType.Repaint)
			{
				DrawGUI.FocusControl("CustomEditorTargetClassNameField");
				focusTextFieldNow = false;
				Repaint();
			}

			HelpField("Script component to make an editor for.");
		}

		private void TargetPathGUI()
		{
			SetDirectory(EditorGUILayout.TextField("Save Path", directory, GUILayout.ExpandWidth(true)));

			if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				var lastRect = GUILayoutUtility.GetLastRect();
				if(lastRect.Contains(Event.current.mousePosition))
				{
					DrawGUI.Use(Event.current);
					var menu = Menu.Create();
					menu.Add("Edit Default Script Path", ShowDefaultScriptPathInPreferences);
					ContextMenuUtility.Open(menu, null);
				}
			}

			HelpField("Click a folder in the Project view to select.");
		}
		
		private bool ClearButton()
		{
			return GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(45));
		}

		private void TemplateSelectionGUI()
		{
			templateIndex = Mathf.Clamp(templateIndex, 0, templateNames.Length - 1);
			int templateIndexNew = EditorGUILayout.Popup("Template", templateIndex, templateNames);
			if(templateIndexNew != templateIndex)
			{
				templateIndex = templateIndexNew;
				UpdateTemplateNamesAndTemplate();
				AutomaticHandlingOnChangeTemplate();

				UpdateCodePreview();
				EditorGUIUtility.editingTextField = false;
			}

			if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				var lastRect = GUILayoutUtility.GetLastRect();
				if(lastRect.Contains(Event.current.mousePosition))
				{
					DrawGUI.Use(Event.current);
					var menu = Menu.Create();
					menu.Add("Edit Template", ShowCurrentTemplateInExplorer);
					ContextMenuUtility.Open(menu, null);
				}
			}
		}

		private void ShowCurrentTemplateInExplorer()
		{
			if(string.IsNullOrEmpty(baseClass))
			{
				return;
			}
			
			// Try to find function file first in custom templates folder and then in built-in
			string functionDataFilePath = Path.Combine(GetCustomTemplateFullPath(), baseClass + ".cs.txt");
			if(File.Exists(functionDataFilePath))
			{
				EditorUtility.RevealInFinder(functionDataFilePath);
				return;
			}

			functionDataFilePath = Path.Combine(GetBuiltinTemplateFullPath(), baseClass + ".cs.txt");
			if(File.Exists(functionDataFilePath))
			{
				EditorUtility.RevealInFinder(functionDataFilePath);
				return;
			}
		}

		private void ShowCurrentTemplateFunctionsInExplorer()
		{
			if(string.IsNullOrEmpty(baseClass))
			{
				return;
			}
			
			// Try to find function file first in custom templates folder and then in built-in
			string functionDataFilePath = Path.Combine(GetCustomTemplateFullPath(), baseClass + ".functions.txt");
			if(File.Exists(functionDataFilePath))
			{
				EditorUtility.RevealInFinder(functionDataFilePath);
				return;
			}

			functionDataFilePath = Path.Combine(GetBuiltinTemplateFullPath(), baseClass + ".functions.txt");
			if(File.Exists(functionDataFilePath))
			{
				EditorUtility.RevealInFinder(functionDataFilePath);
				return;
			}
		}

		private void ShowPreferences()
		{
			if(Event.current == null)
			{
				DrawGUI.OnNextBeginOnGUI(ShowPreferences, true);
				return;
			}

			var preferencesDrawer = PowerInspectorPreferences.RequestGetExistingOrCreateNewWindow();
			preferencesDrawer.SetActiveView("Create Script Wizard");
		}

		private void ShowUsingListInPreferences()
		{
			if(Event.current == null)
			{
				DrawGUI.OnNextBeginOnGUI(ShowUsingListInPreferences, true);
				return;
			}

			var preferencesDrawer = PowerInspectorPreferences.RequestGetExistingOrCreateNewWindow();
			preferencesDrawer.SetActiveView("Create Script Wizard");
			var createScriptWizard = preferencesDrawer.FindVisibleMember("Create Script Wizard") as IParentDrawer;
			if(createScriptWizard != null)
			{
				createScriptWizard.SetUnfolded(true);
				var usingOptions = preferencesDrawer.FindVisibleMember("Using Namespace Options") as IParentDrawer;
				if(usingOptions != null)
				{
					usingOptions.SetUnfolded(true);
					usingOptions.Select(ReasonSelectionChanged.Command);
				}
			}
		}

		private void ShowDefaultNamespaceInPreferences()
		{
			if(Event.current == null)
			{
				DrawGUI.OnNextBeginOnGUI(ShowUsingListInPreferences, true);
				return;
			}

			var preferencesDrawer = PowerInspectorPreferences.RequestGetExistingOrCreateNewWindow();
			preferencesDrawer.SetActiveView("Create Script Wizard");
			var defaultNamespace = preferencesDrawer.FindVisibleMember("Default Namespace");
			if(defaultNamespace != null)
			{
				defaultNamespace.Select(ReasonSelectionChanged.Command);
			}
		}

		private void ShowDefaultScriptPathInPreferences()
		{
			if(Event.current == null)
			{
				DrawGUI.OnNextBeginOnGUI(ShowUsingListInPreferences, true);
				return;
			}

			var preferencesDrawer = PowerInspectorPreferences.RequestGetExistingOrCreateNewWindow();
			preferencesDrawer.SetActiveView("Create Script Wizard");
			var defaultScriptPath = preferencesDrawer.FindVisibleMember("Default Script Path");
			if(defaultScriptPath != null)
			{
				defaultScriptPath.Select(ReasonSelectionChanged.Command);
			}
		}

		private void NamespaceGUI()
		{
			if(namespaceIsInvalid)
			{
				GUI.color = Color.red;
			}

			SetNamespace(EditorGUILayout.TextField("Namespace", scriptPrescription.nameSpace));

			if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				var lastRect = GUILayoutUtility.GetLastRect();
				if(lastRect.Contains(Event.current.mousePosition))
				{
					DrawGUI.Use(Event.current);
					var menu = Menu.Create();
					menu.Add("Edit Default Namespace", ShowDefaultNamespaceInPreferences);
					ContextMenuUtility.Open(menu, null);
				}
			}

			GUI.color = Color.white;
		}

		private void NameGUI()
		{
			if(classNameIsInvalid || classAlreadyExists)
			{
				GUI.color = Color.red;
			}
			GUI.SetNextControlName("ScriptNameField");
			SetClassName(EditorGUILayout.TextField("Name", scriptPrescription.className));
			GUI.color = Color.white;

			if(focusTextFieldNow && !isCustomEditor && Event.current.type == EventType.Repaint)
			{
				DrawGUI.FocusControl("ScriptNameField");
				focusTextFieldNow = false;
			}
		}
		
		private void PreviewGUI()
		{
			var viewWidth = Mathf.Max(position.width * 0.4f, position.width - 380f);

			EditorGUILayout.BeginVertical(GUILayout.Width(viewWidth));
			{
				// Reserve room for preview title
				var previewHeaderRect = GUILayoutUtility.GetRect(new GUIContent("Preview"), styles.previewTitle);

				bool openRightClickMenu;

				// Preview scroll view
				previewScroll = EditorGUILayout.BeginScrollView(previewScroll, styles.previewBox);
				{
					EditorGUILayout.BeginHorizontal();
					{
						// Tiny space since style has no padding in right side
						GUILayout.Space(5);

						var rect = GUILayoutUtility.GetRect
						(
							codePreviewGUIContent,
							styles.previewArea,
							GUILayout.ExpandWidth(true),
							GUILayout.ExpandHeight(true)
						);

						codePreview = EditorGUI.TextArea(rect, codePreview, styles.previewArea);

						openRightClickMenu = Event.current.type == EventType.MouseDown && Event.current.button == 1 && rect.Contains(Event.current.mousePosition);
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndScrollView();

				// Draw preview title after box itself because otherwise the top row
				// of pixels of the slider will overlap with the title
				GUI.Label(previewHeaderRect, new GUIContent("Preview"), styles.previewTitle);

				if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
				{
					if(previewHeaderRect.Contains(Event.current.mousePosition))
					{
						openRightClickMenu = true;
					}
				}

				if(openRightClickMenu)
				{
					DrawGUI.Use(Event.current);
					var menu = Menu.Create();
					menu.Add("Edit Template", ShowCurrentTemplateInExplorer);
					ContextMenuUtility.Open(menu, null);
				}

				GUILayout.Space(4);
			}
			EditorGUILayout.EndVertical();
		}

		private bool InvalidTargetPath()
		{
			if(directory.IndexOfAny(InvalidPathChars) >= 0)
			{
				return true;
			}

			if(TargetDir().Split(PathSeparatorChars, StringSplitOptions.None).Contains(string.Empty))
			{
				return true;
			}

			return false;
		}

		private bool InvalidTargetPathForEditorScript()
		{
			return isEditorClass && !FileUtility.IsEditorPath(directory);
		}

		private bool IsFolder(Object obj)
		{
			return Directory.Exists(AssetDatabase.GetAssetPath(obj));
		}

		private void HelpField(string helpText)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Empty, GUILayout.Width(LabelWidth - 4));
			GUILayout.Label(helpText, styles.helpBox);
			GUILayout.EndHorizontal();
		}

		private string TargetPath()
		{
			return Path.Combine(TargetDir(), scriptPrescription.className + ".cs");
		}

		private string TargetDir()
		{
			return Path.Combine("Assets", directory.Trim(PathSeparatorChars));
		}

		private void SetNamespace(string newNamespace)
		{
			if(!string.Equals(scriptPrescription.nameSpace, newNamespace))
			{
				scriptPrescription.nameSpace = newNamespace;

				namespaceIsInvalid = NamespaceIsInvalid();
				classAlreadyExists = ClassAlreadyExists();
				customEditortargetClassIsNotValidType = CustomEditorTargetClassIsNotValidType();
				canCreate = CanCreate();
				UpdateCodePreview();
			}
		}

		private void SetClassName(string newClassName)
		{
			if(!string.Equals(scriptPrescription.className, newClassName))
			{
				scriptPrescription.className = newClassName;

				classNameIsInvalid = ClassNameIsInvalid();
				classAlreadyExists = ClassAlreadyExists();

				scriptAtPathAlreadyExists = File.Exists(TargetPath());
				invalidTargetPath = InvalidTargetPath();
				invalidTargetPathForEditorScript = InvalidTargetPathForEditorScript();

				customEditorTargetClassDoesNotExist = CustomEditorTargetClassDoesNotExist();
				customEditortargetClassIsNotValidType = CustomEditorTargetClassIsNotValidType();
				
				canCreate = CanCreate();

				UpdateCodePreview();
			}
		}

		private void SetDirectory(string setDirectory)
		{
			if(!string.Equals(directory, setDirectory))
			{
				directory = setDirectory;

				scriptAtPathAlreadyExists = File.Exists(TargetPath());
				invalidTargetPath = InvalidTargetPath();
				invalidTargetPathForEditorScript = InvalidTargetPathForEditorScript();
			}
		}

		private void SetCustomEditorTargetClassName(string newClassName)
		{
			if(!string.Equals(customEditorTargetClassName, newClassName))
			{
				customEditorTargetClassName = newClassName;
				SetClassNameBasedOnTargetClassName();
				customEditorTargetClassDoesNotExist = CustomEditorTargetClassDoesNotExist();
				customEditortargetClassIsNotValidType = CustomEditorTargetClassIsNotValidType();

				UpdateCodePreview();
			}
		}
		
		
		private bool ClassNameIsInvalid()
		{
			return scriptPrescription.className.Length == 0 || !CodeGenerator.IsValidLanguageIndependentIdentifier(scriptPrescription.className);
		}

		private bool NamespaceIsInvalid()
		{
			var n = scriptPrescription.nameSpace;
			int count = n.Length;
			if(count == 0)
			{
				return false;
			}

			//namespaces can contain dots, but not in the beginning, not in the end, and not two in a row
			return n[0] == '.' || n[count - 1] == '.' || !CodeGenerator.IsValidLanguageIndependentIdentifier(n.Replace("..", "!").Replace(".", ""));
		}

		private bool CustomEditorTargetClassExists()
		{
			var types = TypeExtensions.AllVisibleTypes;
			for(int n = types.Length - 1; n >= 0; n--)
			{
				var type = types[n];
				if(string.Equals(type.Name, customEditorTargetClassName))
				{
					string nameSpace = type.Namespace;
					if(string.Equals(nameSpace, scriptPrescription.nameSpace))
					{
						return true;
					}

					for(int u = scriptPrescription.usingNamespaces.Length - 1; u >= 0; u--)
					{
						if(string.Equals(nameSpace, scriptPrescription.usingNamespaces[u]))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool ClassExists(string nameSpace, string className)
		{
			var types = TypeExtensions.AllVisibleTypes;
			for(int n = types.Length - 1; n >= 0; n--)
			{
				var type = types[n];
				if(string.Equals(type.Name, className))
				{
					if(string.Equals(type.Namespace, nameSpace))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool ClassAlreadyExists()
		{
			if(scriptPrescription.className.Length == 0)
			{
				return false;
			}

			return ClassExists(scriptPrescription.nameSpace, scriptPrescription.className);
		}

		private bool CustomEditorTargetClassDoesNotExist()
		{
			if(!isCustomEditor)
			{
				return false;
			}

			if(customEditorTargetClassName.Length == 0)
			{
				return true;
			}

			return !CustomEditorTargetClassExists();
		}

		private bool CustomEditorTargetClassIsNotValidType()
		{
			if(!isCustomEditor)
			{
				return false;
			}

			if(customEditorTargetClassName.Length == 0)
			{
				return true;
			}

			return AppDomain.CurrentDomain.GetAssemblies()
				.All(a => !typeof(UnityEngine.Object).IsAssignableFrom(a.GetType(customEditorTargetClassName, false)));
		}

		private string GetCreateButtonText()
		{
			return CanAddComponent() ? "Create and Add" : "Create";
		}

		private void CreateScript()
		{
			if(!Directory.Exists(TargetDir()))
			{
				Directory.CreateDirectory(TargetDir());
			}

			using(var writer = new StreamWriter(TargetPath()))
			{
				writer.Write(codePreview);
			}
			AssetDatabase.Refresh();
		}

		private void OnSelectionChange()
		{
			clearKeyboardControl = true;

			if(Selection.activeObject == null)
			{
				return;
			}

			if(IsFolder(Selection.activeObject))
			{
				directory = AssetPathWithoutAssetPrefix(Selection.activeObject);
				if(isEditorClass && InvalidTargetPathForEditorScript())
				{
					directory = Path.Combine(directory, "Editor");
				}
			}
			else if(Selection.activeGameObject != null)
			{
				gameObjectToAddTo = Selection.activeGameObject;
			}
			else if(isCustomEditor && Selection.activeObject is MonoScript)
			{
				SetCustomEditorTargetClassName(Selection.activeObject.name);
				SetClassNameBasedOnTargetClassName();
			}

			Repaint();
		}

		private string AssetPathWithoutAssetPrefix(Object obj)
		{
			return AssetDatabase.GetAssetPath(obj).Substring(7);
		}

		private bool IsEditorClass(string className)
		{
			if(className == null)
			{
				return false;
			}

			return GetAllClasses("UnityEditor").Contains(className);
		}

		/// Method to populate a list with all the class in the namespace provided by the user
		static List<string> GetAllClasses(string nameSpace)
		{
			// Get the UnityEditor assembly
			var assembly = Types.EditorAssembly;

			// Create a list for the namespaces
			List<string> namespaceList = new List<string>();

			// Create a list that will hold all the classes the suplied namespace is executing
			List<string> returnList = new List<string>();
			
			foreach(var type in assembly.GetTypes())
			{
				if(string.Equals(type.Namespace, nameSpace))
				{
					namespaceList.Add(type.Name);
				}
			}

			// Now loop through all the classes returned above and add them to our classesName list
			foreach(String className in namespaceList)
			{
				returnList.Add(className);
			}

			return returnList;
		}
		
		[UsedImplicitly]
		private void OnDestroy()
		{
			EditorPrefs.SetString(ScriptBuilder.SaveIn, directory);
			if(templateIndex >= 0 && templateIndex < templateNames.Length)
			{
				EditorPrefs.SetString(ScriptBuilder.Template, templateNames[templateIndex]);
			}
			EditorPrefs.DeleteKey(ScriptBuilder.AttachTo);

			if(preferencesAsset != null)
			{
				preferencesAsset.onSettingsChanged -= OnSettingsChanged;
			}
		}

		private void OnSettingsChanged(InspectorPreferences preferences)
		{
			preferencesAsset = preferences;
			LoadSettings();
			UpdateCodePreview();
		}

		private void UpdateCodePreview()
		{
			#if DEV_MODE
			Debug.Log("UpdateCodePreview");
			#endif

			using(var scriptGenerator = new ScriptBuilder(scriptPrescription, curlyBracesOnNewLine, addComments, addCommentsAsSummary, wordWrapCommentsAfterCharacters, addUsedImplicitly, spaceAfterMethodName, newLine))
			{
				codePreview = scriptGenerator.ToString();
			}
			codePreviewGUIContent = new GUIContent(codePreview);

			GUI.changed = true;
			Repaint();
		}

		private class Styles
		{
			public GUIContent warningContent = new GUIContent("");
			public GUIStyle previewBox = new GUIStyle("OL Box");
			public GUIStyle previewTitle = new GUIStyle("OL Title");
			public GUIStyle loweredBox = new GUIStyle("TextField");
			public GUIStyle helpBox = new GUIStyle("helpbox");
			public GUIStyle previewArea;
			public Styles()
			{
				loweredBox.padding = new RectOffset(1, 1, 1, 1);

				previewArea = new GUIStyle(EditorStyles.miniLabel);
				previewArea.alignment = TextAnchor.UpperLeft;
			}
		}
	}
}