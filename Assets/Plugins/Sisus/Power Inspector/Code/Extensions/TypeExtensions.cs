//#define DEBUG_CACHE_NEW_TYPE_NAME
//#define DEBUG_DEFAULT_VALUE
#define DEBUG_DEFAULT_VALUE_EXCEPTIONS
//#define DEBUG_GENERATE_FILE_LIST_OF_ALL_TYPES
#define DEBUG_GET_TYPE
//#define DEBUG_CONTAINS_OBJECT_REFERENCES
//#define DEBUG_SKIP_TYPE
//#define DEBUG_SKIP_ASSEMBLY

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#if UNITY_2017_3_OR_NEWER
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;
#endif
#endif

#if PI_ASSERTATIONS
using System.Linq;
#endif

#if DEV_MODE
using Debug = UnityEngine.Debug;
#endif

namespace Sisus
{
	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public static class TypeExtensions
	{
		internal static int createInstanceRecursiveCallCount = 0;
		private const int MaxRecursiveCallCount = 25;

		private static bool setupDone;

		private static Assembly[] assemblies;

		private static Assembly[] unityAssemblies;

		/// <summary>
		/// Alphabetically sorted list of all visible exported types (including abstract ones)
		/// </summary>
		private static Type[] allVisibleTypes;

		//labels for all types in the the types array, in the same order
		private static string[] allVisibleTypeLabels;

		/// <summary>
		/// List of all exported but invisible types (non-public type or a non-public nested type such that any of the enclosing types are non-public). Includes abstract types also.
		/// </summary>
		private static Type[] allInvisibleTypes;

		/// <summary>
		/// List of all visible types that are not UnityEngine.Object types
		/// </summary>
		private static Type[] allNonUnityObjectTypes;

		/// <summary>
		/// List of all visible abstract types
		/// </summary>
		private static Type[] abstractTypes;

		/// <summary>
		/// List of all invisible abstract types
		/// </summary>
		private static Type[] invisibleAbstractTypes;

		/// <summary>
		/// List of all visible and non-abstract Component types
		/// </summary>
		private static Type[] componentTypes;

		private static HashSet<Type> invalidComponentTypes;

		/// <summary>
		/// List of all visible and non-abstract ScriptableObject types, excluding Editor types
		/// </summary>
		private static Type[] scriptableObjectTypes;

		#if UNITY_EDITOR
		/// <summary>
		/// List of all visible and non-abstract Editor types
		/// </summary>
		private static Type[] editorTypes;

		/// <summary>
		/// List of all invisible and non-abstract Editor types
		/// </summary>
		private static Type[] invisibleEditorTypes;
		#endif

		/// <summary>
		/// List of all visible and non-abstract UnityObject.Engine types excluding Component types
		/// </summary>
		private static Type[] otherUnityObjectTypes;

		/// <summary>
		/// List of all visible and non-abstract class types excluding UnityEngine.Object types
		/// </summary>
		private static Type[] nonUnityObjectClassTypes;

		private static Type[] invisibleNonUnityObjectClassTypes;

		/// <summary>
		/// List of all visible value types excluding enums
		/// </summary>
		private static Type[] nonEnumValueTypes;

		/// <summary>
		/// List of all invisible value types that are not enum.
		/// </summary>
		private static Type[] invisibleNonEnumValueTypes;

		/// <summary>
		/// List of all visible enum types
		/// </summary>
		private static Type[] visibleEnumTypes;

		/// <summary>
		/// List of all invisible enum types.
		/// </summary>
		private static Type[] enumTypesIncludingInvisible;

		private static readonly Dictionary<string, KeyValuePair<string, Type>[]> enumTypesByName = new Dictionary<string, KeyValuePair<string, Type>[]>(50);
		private static readonly Dictionary<string, KeyValuePair<string, Type>[]> TypesByName = new Dictionary<string, KeyValuePair<string, Type>[]>(1000);
		private static readonly Dictionary<Type, string> TypePopupMenuPaths = new Dictionary<Type, string>();
		private static readonly Dictionary<Type, string> TypeFullNames = new Dictionary<Type, string>();
		private static readonly Dictionary<Type, string> TypeNames = new Dictionary<Type, string>();
		private static readonly List<Type> ReusableTypeList = new List<Type>(100);
		private static readonly Dictionary<Assembly, Type[]> HiddenTypesByAssembly = new Dictionary<Assembly, Type[]>(100);
		private static readonly BiDictionary<Type, int> TypeIds = new BiDictionary<Type, int>();

		#if UNITY_EDITOR
		/// <summary>
		/// this is initialized on load due to the usage of the InitializeOnLoad attribute
		/// </summary>
		static TypeExtensions()
		{
			if(!setupDone)
			{
				EditorApplication.delayCall += SetupWhenReady;
			}
		}

		private static void SetupWhenReady()
		{
			if(setupDone)
			{
				return;
			}

			if(!ApplicationUtility.IsReady())
			{
				EditorApplication.delayCall += SetupWhenReady;
				return;
			}

			Setup();
		}
		#endif

		public static Assembly[] Assemblies
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				return assemblies;
			}
		}

		public static Assembly[] UnityAssemblies
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				return unityAssemblies;
			}
		}

		/// <summary>
		/// Gets a list of all non-abstract and visible Types
		/// </summary>
		/// <value>
		/// list of all non-abstract and visible Types
		/// </value>
		public static Type[] AllVisibleTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}

				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(allVisibleTypes[0] != null);
				#endif
				return allVisibleTypes;
			}
		}

		/// <summary>
		/// Gets a list of all non-abstract and visible Types
		/// </summary>
		/// <value>
		/// list of all non-abstract and visible Types
		/// </value>
		public static Type[] VisibleEnumTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(visibleEnumTypes[0] != null);
				#endif
				return visibleEnumTypes;
			}
		}

		/// <summary>
		/// Gets a list of all non-abstract and visible Types
		/// </summary>
		/// <value>
		/// list of all non-abstract and visible Types
		/// </value>
		public static Type[] EnumTypesIncludingInvisible
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(enumTypesIncludingInvisible.Length == 0 || enumTypesIncludingInvisible[0] != null);
				#endif
				return enumTypesIncludingInvisible;
			}
		}

		/// <summary>
		/// Gets a list of all non-abstract and visible non-UnityEngine.Object types
		/// </summary>
		/// <value>
		/// list of all non-abstract and visible non-UnityEngine.Object types
		/// </value>
		public static Type[] AllNonUnityObjectTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(allNonUnityObjectTypes[0] != null);
				#endif
				return allNonUnityObjectTypes;
			}
		}

		public static string[] MenuLabelsForAllVisibleTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(allVisibleTypeLabels[0] != null);
				#endif
				return allVisibleTypeLabels;
			}
		}

		public static Type[] AbstractTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				#if DEV_MODE
				Debug.Assert(abstractTypes[0] != null);
				#endif
				return abstractTypes;
			}
		}
		
		public static Type[] ComponentTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(componentTypes[0] != null);
				#endif
				return componentTypes;
			}
		}

		
		public static HashSet<Type> InvalidComponentTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				return invalidComponentTypes;
			}
		}

		public static Type[] OtherUnityObjectTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(otherUnityObjectTypes[0] != null);
				#endif
				return otherUnityObjectTypes;
			}
		}
		
		public static Type[] NonUnityObjectClassTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}
				return nonUnityObjectClassTypes;
			}
		}
		
		public static Type[] NonEnumValueTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}				
				return nonEnumValueTypes;
			}
		}

		public static Type[] AllInvisibleTypes
		{
			get
			{
				if(!setupDone)
				{
					Setup();
				}

				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(allInvisibleTypes[0] != null);
				#endif
				return allInvisibleTypes;
			}
		}

		#if DEV_MODE && UNITY_EDITOR
		//[MenuItem("Test/Setup")]
		#endif
		private static void Setup()
		{
			setupDone = true;
			
			var all = new List<Type>(9500);
			var allNotUnity = new List<Type>(7000);
			var invisibles = new List<Type>(8000);
			var abstracts = new List<Type>(1700);
			var invisibleAbstracts = new List<Type>(1000); //what is optimal value?
			var components = new List<Type>(500);
			var invalidComponents = new List<Type>(0);
			var unityObjects = new List<Type>(150);
			var scriptableObjects = new List<Type>(150);
			#if UNITY_EDITOR
			var editors = new List<Type>(100);
			var invisibleEditors = new List<Type>(220);
			#endif
			var classes = new List<Type>(5000);
			var invisibleClasses = new List<Type>(6000);
			var values = new List<Type>(1000);
			var invisibleValues = new List<Type>(1000);
			var enums = new List<Type>(1600);
			var invisibleEnums = new List<Type>(1000);
			TypeIds.Clear();

			#if DEV_MODE && DEBUG_GENERATE_FILE_LIST_OF_ALL_TYPES
			var s = new System.Text.StringBuilder(10000);
			#endif

			int nth = 0;

			var unitysAssemblies = new List<Assembly>(100);

			HashSet<string> ignoredAssemblies;
			var ignoredAssembliesFilePath = new StackTrace(true).GetFrame(0).GetFileName();
			ignoredAssembliesFilePath = ignoredAssembliesFilePath.Substring(0, ignoredAssembliesFilePath.Length - ".cs".Length);
			ignoredAssembliesFilePath += ".IgnoredAssemblies.txt";
			if(File.Exists(ignoredAssembliesFilePath))
			{
				ignoredAssemblies = new HashSet<string>(File.ReadAllLines(ignoredAssembliesFilePath));
			}
			else
			{
				ignoredAssemblies = new HashSet<string>();
			}

			#if UNITY_EDITOR && UNITY_2017_3_OR_NEWER
			var editorOnlyAssemblies = new HashSet<string>();
			var assemblyCompilationInfos = CompilationPipeline.GetAssemblies();
			for(int n = assemblyCompilationInfos.Length - 1; n >= 0; n--)
			{
				var assemblyInfo = assemblyCompilationInfos[n];
				if((assemblyInfo.flags & AssemblyFlags.EditorAssembly) == AssemblyFlags.EditorAssembly)
				{
					editorOnlyAssemblies.Add(assemblyInfo.name);
				}
			}
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(editorOnlyAssemblies.Count > 0);
			#endif
			#endif

			assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for(int a = assemblies.Length - 1; a >= 0; a--)
			{
				var assembly = assemblies[a];
				
				var assemblyName = assembly.GetName().Name;

				if(ignoredAssemblies.Contains(assemblyName))
				{
					#if DEV_MODE && DEBUG_SKIP_ASSEMBLY
					Debug.Log("Skipping assembly " + assemblyName + " because it was found in TypeExtensions.IgnoredAssemblies.txt...");
					#endif
					continue;
				}

				// Add assemblies in namespaces starting with "Unity.", "UnityEngine." and "UnityEditor." to unitysAssemblies.
				if(assemblyName.StartsWith("Unity", StringComparison.Ordinal))
				{
					string sub = assemblyName.Substring(5);
					if(sub.StartsWith(".", StringComparison.Ordinal) || sub.StartsWith("Engine.", StringComparison.Ordinal) || sub.StartsWith("Editor.", StringComparison.Ordinal) || sub.Equals("Editor"))
					{
						unitysAssemblies.Add(assembly);
					}
				}
				else if(string.Equals(assemblyName, "Anonymously Hosted DynamicMethods Assembly"))
				{
					continue;
				}

				#if UNITY_EDITOR && UNITY_2017_3_OR_NEWER
				bool isEditorAssembly = editorOnlyAssemblies.Contains(assemblyName);
				#endif

				#if DEV_MODE && DEBUG_GENERATE_FILE_LIST_OF_ALL_TYPES
				s.Append("\n\n\n>>>");
				s.Append(assembly.GetName().Name);
				s.Append(":<<<\n");
				#endif
				
				//NOTE: important to also get invisible types, so that internal Editors such as RectTransformEditor are also returned
				var types = assembly.GetLoadableTypes(false);
				for(int n = types.Length - 1; n >= 0; n--)
				{
					var t = types[n];

					// Skip classes that have the StructLayoutAttribute to avoid crashes occurring when data marshaling occurs
					// between managed and unmanaged code when e.g. GetCustomAttributes is called for classes members of such a class.
					if(t.GetCustomAttributes(typeof(System.Runtime.InteropServices.StructLayoutAttribute), true).Length > 0)
					{
						#if DEV_MODE && DEBUG_SKIP_TYPE
						Debug.Log("Skipping type " + t.FullName + " because it contains the StructLayoutAttribute...");
						#endif
						continue;
					}

					TypeIds.Add(t, nth);
					nth++;

					var name = t.Name;
					var fullName = t.FullName;

					var fullNameAndType = new KeyValuePair<string, Type>(fullName, t);
					KeyValuePair<string, Type>[] fullNamesAndTypes;
					if(TypesByName.TryGetValue(name, out fullNamesAndTypes))
					{
						int index = fullNamesAndTypes.Length;
						Array.Resize(ref fullNamesAndTypes, index + 1);
						fullNamesAndTypes[index] = fullNameAndType;
					}
					else
					{
						fullNamesAndTypes = ArrayPool<KeyValuePair<string, Type>>.CreateWithContent(fullNameAndType);
					}
					TypesByName[name] = fullNamesAndTypes;

					if(!t.IsVisible)
					{
						#if DEV_MODE && DEBUG_GENERATE_FILE_LIST_OF_ALL_TYPES
						s.Append("(");
						s.Append(t.FullName);
						s.Append(" )\n");
						#endif

						invisibles.Add(t);

						if(t.IsAbstract)
						{
							invisibleAbstracts.Add(t);
							continue;
						}

						if(t.IsClass)
						{
							#if UNITY_EDITOR
							if(Types.Editor.IsAssignableFrom(t))
							{
								invisibleEditors.Add(t);
								continue;
							}
							#endif

							invisibleClasses.Add(t);
							continue;
						}

						if(t.IsEnum)
						{
							invisibleEnums.Add(t);
							continue;
						}

						invisibleValues.Add(t);
						continue;
					}

					all.Add(t);

					#if DEV_MODE && DEBUG_GENERATE_FILE_LIST_OF_ALL_TYPES
					s.Append(fullName);
					s.Append('\n');
					#endif

					if(t.IsAbstract)
					{
						abstracts.Add(t);
						continue;
					}

					if(t.IsClass)
					{
						if(t.IsUnityObject())
						{
							if(t.IsComponent())
							{
								components.Add(t);
								#if UNITY_EDITOR && UNITY_2017_3_OR_NEWER
								if(isEditorAssembly)
								{
									invalidComponents.Add(t);
								}
								#endif
								continue;
							}
							#if UNITY_EDITOR
							if(Types.Editor.IsAssignableFrom(t))
							{
								editors.Add(t);
								continue;
							}
							#endif
							if(t.IsScriptableObject())
							{
								scriptableObjects.Add(t);
								continue;
							}
							unityObjects.Add(t);
							continue;
						}
						allNotUnity.Add(t);
						classes.Add(t);
						continue;
					}
					
					allNotUnity.Add(t);

					if(t.IsEnum)
					{
						enums.Add(t);
						if(enumTypesByName.TryGetValue(name, out fullNamesAndTypes))
						{
							int index = fullNamesAndTypes.Length;
							Array.Resize(ref fullNamesAndTypes, index + 1);
							fullNamesAndTypes[index] = fullNameAndType;
						}
						else
						{
							fullNamesAndTypes = ArrayPool<KeyValuePair<string, Type>>.CreateWithContent(fullNameAndType);
						}
						enumTypesByName[name] = fullNamesAndTypes;
						continue;
					}

					values.Add(t);
				}
			}

			//TO DO: Assert that no arrays have any duplicates of types?
			
			//as an optimization, make sure that some of the most common types are found near the ends of the arrays
			//(so when looped in reverse order using a for-loop, they'll be checked first)
			components.Remove(Types.Animator);
			components.Add(Types.Animator);
			components.Remove(Types.BoxCollider);
			components.Add(Types.BoxCollider);
			components.Remove(Types.RectTransform);
			components.Add(Types.RectTransform);
			components.Remove(Types.Transform);
			components.Add(Types.Transform);

			unityObjects.Remove(Types.GameObject);
			unityObjects.Add(Types.GameObject);

			values.Remove(Types.Double);
			values.Add(Types.Double);
			values.Remove(Types.Vector3);
			values.Add(Types.Vector3);
			values.Remove(Types.Int);
			values.Add(Types.Int);
			values.Remove(Types.Bool);
			values.Add(Types.Bool);
			values.Remove(Types.Float);
			values.Add(Types.Float);

			classes.Remove(Types.String);
			classes.Add(Types.String);

			allVisibleTypes = ArrayPool<Type>.Create(all);

			abstractTypes = ArrayPool<Type>.Create(abstracts);
			invisibleAbstractTypes = ArrayPool<Type>.Create(invisibleAbstracts);
			componentTypes = ArrayPool<Type>.Create(components);
			invalidComponentTypes = new HashSet<Type>((invalidComponents));
			scriptableObjectTypes = ArrayPool<Type>.Create(scriptableObjects);
			#if UNITY_EDITOR
			editorTypes = ArrayPool<Type>.Create(editors);
			#endif
			otherUnityObjectTypes = ArrayPool<Type>.Create(unityObjects);
			nonUnityObjectClassTypes = ArrayPool<Type>.Create(classes);
			visibleEnumTypes = ArrayPool<Type>.Create(enums);
			nonEnumValueTypes = ArrayPool<Type>.Create(values);
			allNonUnityObjectTypes = ArrayPool<Type>.Create(allNotUnity);

			allInvisibleTypes = ArrayPool<Type>.Create(invisibles);
			#if UNITY_EDITOR
			invisibleEditorTypes = ArrayPool<Type>.Create(invisibleEditors);
			#endif
			invisibleNonEnumValueTypes = ArrayPool<Type>.Create(invisibleValues);
			invisibleNonUnityObjectClassTypes = ArrayPool<Type>.Create(invisibleClasses);
			
			invisibleEnums.AddRange(visibleEnumTypes);
			enumTypesIncludingInvisible = ArrayPool<Type>.Create(invisibleEnums);

			int count = allVisibleTypes.Length;
			allVisibleTypeLabels = new string[count];
			for(int n = count - 1; n >= 0; n--)
			{
				var type = allVisibleTypes[n];
				
				string menuPath = StringUtils.ToString(type, '/');
				TypePopupMenuPaths[type] =  menuPath;
				allVisibleTypeLabels[n] = menuPath;
				TypeFullNames[type] = menuPath.Replace('/', '.');
				TypeNames[type] = StringUtils.ToString(type, '\0');
			}

			Array.Sort(allVisibleTypeLabels, allVisibleTypes);
			
			unityAssemblies = unitysAssemblies.ToArray();

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(unityAssemblies.Length > 0, "TypeExtensions.UnityAssemblies length zero");
			#endif

			#if DEV_MODE
			//Debug.Log("allVisibleTypes.Length = " + allVisibleTypes.Length);
			//Debug.Log("allNonUnityObjectTypes.Length = " + allNonUnityObjectTypes.Length);
			//Debug.Log("abstractTypes.Length = " + abstractTypes.Length);
			//Debug.Log("componentTypes.Length = " + componentTypes.Length);
			//Debug.Log("otherUnityObjectTypes.Length = " + otherUnityObjectTypes.Length);
			//Debug.Log("scriptableObjectTypes.Length = " + scriptableObjectTypes.Length);
			//Debug.Log("editorTypes.Length = " + editorTypes.Length);
			//Debug.Log("enumTypes.Length = " + visibleEnumTypes.Length);
			//Debug.Log("nonEnumValueTypes.Length = " + nonEnumValueTypes.Length);
			//Debug.Log("nonUnityObjectClassTypes.Length = " + nonUnityObjectClassTypes.Length);
			//Debug.Log("invisibleTypes.Length = " + allInvisibleTypes.Length);
			//Debug.Log("invisibleEditorTypes.Length = " + invisibleEditorTypes.Length);
			//Debug.Log("invisibleNonEnumValueTypes.Length = " + invisibleNonEnumValueTypes.Length);
			//Debug.Log("invisibleNonUnityObjectClassTypes.Length = " + invisibleNonUnityObjectClassTypes.Length);

			#if DEV_MODE && DEBUG_GENERATE_FILE_LIST_OF_ALL_TYPES
			System.IO.File.WriteAllText(Application.dataPath + "/Testing/TypesList.txt", s.ToString());
			#endif
			
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(allVisibleTypeLabels.Length == allVisibleTypes.Length);
			Debug.Assert(allVisibleTypes.Distinct().Count() == allVisibleTypes.Length);
			#endif
		}
		
		public static Type GetType(int typeIndex)
		{
			return AllVisibleTypes[typeIndex];
		}
		
		public static bool IsNumeric(this Type type)
		{
			return type == Types.Int || type == Types.Float || type == Types.Long || type == Types.Short || type == Types.UInt || type == Types.UShort || type == Types.Double || type == Types.Decimal || type == Types.ULong;
		}

		public static bool IsUnityObject(this Type type)
		{
			return Types.UnityObject.IsAssignableFrom(type);
		}

		public static bool IsComponent(this Type type)
		{
			return Types.Component.IsAssignableFrom(type);
		}

		public static bool IsScriptableObject(this Type type)
		{
			return Types.ScriptableObject.IsAssignableFrom(type);
		}

		public static bool IsGameObject(this Type type)
		{
			return type == Types.GameObject;
		}

		//finds the nth member with the given type inside the array and returns its index
		//if target did not have as many targets with said type, returns -1
		public static int IndexOfNthInstanceOfType<T>(this T[] array, Type type, int nth)
		{
			int count = array.Length;
			for(int n = 0; n < count; n++)
			{
				var test = array[n];
				if(test != null && type == test.GetType())
				{
					if(nth == 1)
					{
						return n;
					}
					nth--;
				}
			}
			return -1;
		}

		/// <summary>
		/// Finds the nth member with the given type inside the array and returns its index
		/// if target did not have as many targets with said type, returns -1
		/// </summary>
		/// <param name="array">Array whose contents will be iterated</param>
		/// <param name="nth">A number equal to or larger than one.</param>
		/// <returns>Int of nth null in array or -1 if not enough nulls were found in the array.</returns>
		public static int IndexOfNthNull<T>(this T[] array, int nth) where T : class
		{
			int count = array.Length;
			for(int n = 0; n < count; n++)
			{
				if(array[n] == null)
				{
					if(nth == 1)
					{
						return n;
					}
					nth--;
				}
			}
			return -1;
		}

		/// <summary>
		/// TO DO: Give this a better name
		/// Return -1 if target was not found inside array. Otherwise returns value of 0 or above, based on how many objects with the same type as target precede it
		/// </summary>
		public static int CountPrecedingInstancesWithSameType<T>(this T[] array, T target) // where T : class
		{
			int found = -1;
			int count = array.Length;
			var type = target.GetType();
			for(int n = 0; n < count; n++)
			{
				var check = array[n];

				if(target.Equals(check))
				{
					found++;
					return found;
				}
				
				if(check != null && type.Equals(check.GetType()))
				{
					found++;
				}
			}

			return -1;
		}
		
		public static bool IsJaggedArray([NotNull]this Type type)
		{
			if(!type.IsArray)
			{
				return false;
			}
			return type.Name.EndsWith("[][]", StringComparison.Ordinal);
		}
		
		public static object DefaultValue([NotNull]this Type type)
		{
			createInstanceRecursiveCallCount = 0;
			return type.DefaultValueInternal();
		}

		internal static object DefaultValueInternal([NotNull]this Type type)
		{
			if(type == Types.String)
			{
				return "";
			}

			if(type.IsArray)
			{
				string typeName = type.Name;

				var elementType = type.GetElementType();

				#if DEV_MODE && DEBUG_DEFAULT_VALUE
				Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") IsArray with ElementType="+ StringUtils.ToString(elementType));
				#endif

				#if DEV_MODE && DEBUG_DEFAULT_VALUE
				if(elementType.IsAbstract)
				{
					#if DEV_MODE && DEBUG_DEFAULT_VALUE
					Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") elementType " + StringUtils.ToString(elementType) + " IsAbstract: returning null");
					#endif
				}

				if(elementType.IsGenericType)
				{
					var genericTypeDefinition = elementType.GetGenericTypeDefinition();
					Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") elementType " + StringUtils.ToString(elementType) + " IsGenericType with IsGenericTypeDefinition=" + elementType.IsGenericTypeDefinition + ", genericTypeDefinition=" + StringUtils.ToString(genericTypeDefinition));
				}
				#endif

				// If element type is a generic type, we can't generate a default value instance, since we don't know which type to use.
				// For that we'd need a method like DefaultValue(Type type, params Type[] genericTypes).
				if(elementType.IsGenericTypeDefinition)
				{
					#if DEV_MODE && DEBUG_DEFAULT_VALUE
					var genericTypeDefinition = elementType.GetGenericTypeDefinition();
					Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") elementType " + StringUtils.ToString(elementType) + " IsGenericTypeDefinition with IsGenericType="+ elementType.IsGenericType +", genericTypeDefinition=" + StringUtils.ToString(genericTypeDefinition)+": returning null");
					#endif
					return null;
				}

				// If element type is a generic parameter type, we can't generate a default value instance, since we don't know which type to use.
				// For that we'd need a method like DefaultValue(Type type, params Type[] genericTypes).
				if(elementType.IsGenericParameter)
				{
					#if DEV_MODE && DEBUG_DEFAULT_VALUE
					Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") elementType " + StringUtils.ToString(elementType) + " IsGenericParameter: returning null");
					#endif
					return null;
				}
				
				switch(type.GetArrayRank())
				{
					case 1:
						if(type.IsJaggedArray())
						{
							Array.CreateInstance(elementType.MakeArrayType(), 0);
						}
						return Array.CreateInstance(elementType, 0);
					case 2:
						return Array.CreateInstance(elementType, 0, 0);
					case 3:
						return Array.CreateInstance(elementType, 0, 0, 0);
					default:
						int arrayDefinitionIndex = typeName.LastIndexOf('[');
						int jaggedIndex = typeName.IndexOf(',', arrayDefinitionIndex + 1);
						int rank;
						for(rank = 2; (jaggedIndex = typeName.IndexOf(',', jaggedIndex + 1)) != -1; rank++) { }
						var lengths = ArrayPool<int>.Create(rank);
						return Array.CreateInstance(elementType, lengths);
				}
			}
			
			if(type.IsGenericTypeDefinition)
			{
				#if DEV_MODE && DEBUG_DEFAULT_VALUE
				Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") IsGenericTypeDefinition: returning null");
				#endif
				return null;
			}

			if(type.IsGenericParameter)
			{
				#if DEV_MODE && DEBUG_DEFAULT_VALUE
				Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") IsGenericParameter: returning null");
				#endif
				return null;
			}

			if(type.IsAbstract)
			{
				#if DEV_MODE && DEBUG_DEFAULT_VALUE
				Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") IsAbstract: returning null");
				#endif
				return null;
			}

			if(type.IsUnityObject())
			{
				#if DEV_MODE && DEBUG_DEFAULT_VALUE
				Debug.Log("DefaultValue(" + StringUtils.ToString(type) + ") IsUnityObject: returning null");
				#endif
				return null;
			}

			if(type == Types.SystemObject)
			{
				return null;
			}

			if(Types.Type.IsAssignableFrom(type))
			{
				return typeof(void);
			}

			if(type == Types.Void || type == Types.DBNull)
			{
				return null;
			}

			if(type.IsEnum)
			{
				var underlyingType = Enum.GetUnderlyingType(type);
				return Enum.ToObject(type, underlyingType.DefaultValueInternal());
			}

			if(type.IsGenericType)
			{
				var baseType = type.GetGenericTypeDefinition();
				if(baseType == Types.KeyValuePair)
				{
					#if DEV_MODE && DEBUG_DEFAULT_VALUE
					Debug.Log("type: "+StringUtils.ToString(type));
					#endif
					var keyAndValueTypes = type.GetGenericArguments();
					return Activator.CreateInstance(type, keyAndValueTypes[0].DefaultValueInternal(), keyAndValueTypes[1].DefaultValueInternal());
				}

				var nullableType = Nullable.GetUnderlyingType(type);
				if(nullableType != null)
				{
					#if DEV_MODE && DEBUG_DEFAULT_VALUE
					Debug.Log("return null for nullable type: "+StringUtils.ToString(type));
					#endif
					return null;
				}
			}

			if(Types.MulticastDelegate.IsAssignableFrom(type))
			{
				return null;
			}

			// try to create instance using Activator.CreateInstance and FormatterServices.GetUninitializedObject
			return CreateInstanceInternal(type);
		}

		/// <summary>
		/// Tries to create an instance of type using Activator.CreateInstance and if that fails FormatterServices.GetUninitializedObject.
		/// </summary>
		/// <param name="type"> The type of the instance to create. </param>
		/// <returns> The created instance, or null if failed to create instance. </returns>
		[CanBeNull]
		public static object CreateInstance([NotNull]this Type type)
		{
			createInstanceRecursiveCallCount = 0;
			return CreateInstanceInternal(type);
		}

		/// <summary>
		/// Tries to create an instance of type using Activator.CreateInstance and if that fails FormatterServices.GetUninitializedObject.
		/// </summary>
		/// <param name="type"> The type of the instance to create. </param>
		/// <returns> The created instance, or null if failed to create instance. </returns>
		[CanBeNull]
		internal static object CreateInstanceInternal([NotNull]this Type type)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type != null);
			Debug.Assert(!type.IsComponent());
			Debug.Assert(!type.IsGameObject());
			Debug.Assert(!type.IsGenericTypeDefinition, "CreateInstance called for type "+StringUtils.ToString(type)+ " with IsGenericTypeDefinition "+StringUtils.True);
			Debug.Assert(!type.IsAbstract, "CreateInstance called for type " + StringUtils.ToString(type) + " with IsAbstract " + StringUtils.True);
			#endif

			if(createInstanceRecursiveCallCount > MaxRecursiveCallCount)
			{
				createInstanceRecursiveCallCount = 0;
				#if DEV_MODE
				Debug.Log("type.CreateInstance recursive call count was 25 for type " + StringUtils.ToString(type));
				#endif
				return FormatterServices.GetUninitializedObject(type);
			}

			if(type.IsScriptableObject())
			{
				#if DEV_MODE && DEBUG_DEFAULT_VALUE
				Debug.Log("ScriptableObject.CreateInstance(" + StringUtils.ToString(type)+")");
				#endif
				return ScriptableObject.CreateInstance(type);
			}

			if(type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(System.Collections.ObjectModel.ReadOnlyCollection<>))
			{
				return null;
			}

			// try to create instance using parameterless constructor
			// (this can fail if class has no parameterless constructor)
			try
			{
				#if DEV_MODE && DEBUG_DEFAULT_VALUE
				Debug.Log("Activator.CreateInstance(" + StringUtils.ToString(type)+")");
				#endif

				return Activator.CreateInstance(type);
			}
			#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
			catch(Exception e) { Debug.LogWarning("DefaultValue(" + StringUtils.ToString(type) + "): parameterless CreateInstance failed. Trying constructors with parameters.\n" + e); }
			#else
			catch { }
			#endif

			// manually fetch class constructors, and try and create an instance using those
			// (this can fail if constructor does not accept default parameter values and throws an exception)
			var constructors = type.GetConstructors();
			for(int c = constructors.Length - 1; c >= 0; c--)
			{
				var constructor = constructors[c];
				var parameters = constructor.GetParameters();
				int count = parameters.Length;

				// Avoid infinite loop issues with constructors where parameter type equals type of class that contains the constructor.
				// E.g. LinkedList(LinkedList previous, LinkedList next)
				bool infiniteRecursionDetected = false;
				for(int p = 0; p < count; p++)
				{
					if(parameters[p].ParameterType == type)
					{
						infiniteRecursionDetected = true;
						break;
					}
				}
				if(infiniteRecursionDetected)
				{
					continue;
				}				

				bool parameterGenerationFailed = false;

				//generate default values for all parameters that the constructor requires.
				var parameterValues = ArrayPool<object>.Create(count);
				for(int p = 0; p < count; p++)
				{
					try
					{
						parameterValues[p] = parameters[p].DefaultValueInternal();

						#if DEV_MODE && DEBUG_DEFAULT_VALUE
						Debug.Log(StringUtils.ToString(type)+" parameterValues["+p+"] = " + StringUtils.ToString(parameterValues[p]) +")");
						#endif
					}
					#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
					catch(Exception e)
					{
						Debug.LogWarning("DefaultValue(" + StringUtils.ToString(type) + "): generating default value for parameter " + p +"/+"+count+" of type "+parameters[p].ParameterType+" failed.\n"+e);
						parameterGenerationFailed = true;
					}
					#else
					catch
					{
						parameterGenerationFailed = true;
					}
					#endif

					if(parameterGenerationFailed)
					{
						break;
					}
				}

				if(!parameterGenerationFailed)
				{
					//try to create the instance
					try
					{
						return constructor.Invoke(parameterValues);
					}
					#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
					catch(Exception e) { Debug.LogWarning("DefaultValue(" + StringUtils.ToString(type) + "): CreateInstance with "+count+" parameters failed. Trying ("+(c == constructors.Length - 1 ? "GetUninitializedObject next" : "next constructor")+").\n"+e); }
					#else
					catch { }
					#endif
				}
			}

			// If all else fails, as a last resort, don't use a constructor at all, and
			// return an uninitialized instance of the class. This can be somewhat risky,
			// since important setup phases could get skipped because constructor was not called.
			try
			{
				return FormatterServices.GetUninitializedObject(type);
			}
			#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
			catch(Exception e) { Debug.LogWarning("DefaultValue: GetUninitializedObject failed. Returning null...\n"+ e); }
			#else
			catch { }
			#endif

			// if everything failed, return null
			return null;
		}

		/// <summary>
		/// Tries to create an instance of type using Activator.CreateInstance and if that fails FormatterServices.GetUninitializedObject.
		/// </summary>
		/// <param name="type"> The type of the instance to create. </param>
		/// <returns> The created instance, or null if failed to create instance. </returns>
		[CanBeNull]
		public static object CreateInstance([NotNull]this Type type, [NotNull]Type parameterType, object parameterValue)
		{
			createInstanceRecursiveCallCount = 0;
			return CreateInstanceInternal(type, parameterType, parameterValue);
		}

		/// <summary>
		/// Tries to create an instance of type using Activator.CreateInstance and if that fails FormatterServices.GetUninitializedObject.
		/// </summary>
		/// <param name="type"> The type of the instance to create. </param>
		/// <returns> The created instance, or null if failed to create instance. </returns>
		[CanBeNull]
		internal static object CreateInstanceInternal([NotNull]this Type type, [NotNull]Type parameterType, object parameterValue)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type != null);
			Debug.Assert(parameterType != null);
			Debug.Assert(!type.IsUnityObject());
			if(parameterValue == null)
			{
				Debug.Assert(!parameterType.IsValueType);
			}
			else
			{
				Debug.Assert(parameterType == parameterValue.GetType());
			}
			#endif
			
			if(createInstanceRecursiveCallCount > MaxRecursiveCallCount)
			{
				createInstanceRecursiveCallCount = 0;
				#if DEV_MODE
				Debug.Log("type.CreateInstance recursive call count was 25 for type " + StringUtils.ToString(type));
				#endif
				return FormatterServices.GetUninitializedObject(type);
			}

			if(parameterType == type)
			{
				// can not generate instance using provided parameter, but try to generate it without it.
				return type.DefaultValueInternal();
			}

			// Fetch class constructors. We will then try to find one that accepts parameterValue as a parameter.
			var constructors = type.GetConstructors();
			
			// Best constructor candidate is one that only has one parameter that matches provided parameter type.
			int constructorCount = constructors.Length;
			for(int c = constructorCount - 1; c >= 0; c--)
			{
				var constructor = constructors[c];
				var parameters = constructor.GetParameters();
				int parameterCount = parameters.Length;
				if(parameterCount != 1)
				{
					continue;
				}

				var parameter = parameters[0];
				
				if(!parameter.ParameterType.IsAssignableFrom(parameterType))
				{
					continue;
				}

				var parameterValues = ArrayExtensions.TempObjectArray(parameterValue);
				
				//try to create the instance
				try
				{
					return constructor.Invoke(parameterValues);
				}
				#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
				catch(Exception e) { Debug.LogWarning("DefaultValue(" + StringUtils.ToString(type) + "): CreateInstance with "+ parameterCount + " parameters failed. Trying ("+(c == constructors.Length - 1 ? "GetUninitializedObject next" : "next constructor")+").\n"+e); }
				#else
				catch { }
				#endif
			}

			bool atLeastOnConstructorWithEnoughParametersFound = true;
			// Second best candidate is one that has more than one parameter, but one of them matches provided parameter type.
			// The less parameters there are, the better.
			for(int wantedParameterCount = 2; atLeastOnConstructorWithEnoughParametersFound; wantedParameterCount++)
			{
				atLeastOnConstructorWithEnoughParametersFound = false;

				for(int c = constructorCount - 1; c >= 0; c--)
				{
					var constructor = constructors[c];
					var parameters = constructor.GetParameters();
					int parameterCount = parameters.Length;
					
					if(parameterCount < wantedParameterCount)
					{
						continue;
					}

					atLeastOnConstructorWithEnoughParametersFound = true;

					int assignableAtIndex = -1;

					for(int p = 0; p < parameterCount; p++)
					{
						var parameter = parameters[p];
				
						if(!parameter.ParameterType.IsAssignableFrom(parameterType))
						{
							continue;
						}

						assignableAtIndex = p;
						break;
					}

					if(assignableAtIndex == -1)
					{
						break;
					}

					var parameterValues = ArrayPool<object>.Create(parameterCount);
					parameterValues[assignableAtIndex] = parameterValue;

					// Generate default values for all the other parameters that the constructor requires.
					bool parameterGenerationFailed = false;
					for(int p = 0; p < parameterCount; p++)
					{
						if(assignableAtIndex == p)
						{
							continue;
						}

						try
						{
							parameterValues[p] = parameters[p].DefaultValueInternal();

							#if DEV_MODE && DEBUG_DEFAULT_VALUE
							Debug.Log(StringUtils.ToString(type)+" parameterValues["+p+"] = " + StringUtils.ToString(parameterValues[p]) +")");
							#endif
						}
						#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
						catch(Exception e)
						{
							Debug.LogWarning("DefaultValue(" + StringUtils.ToString(type) + "): generating default value for parameter " + (p + 1) +"/+"+ parameterCount + " of type "+parameters[p].ParameterType+" failed.\n"+e);
							parameterGenerationFailed = true;
						}
						#else
						catch
						{
							parameterGenerationFailed = true;
						}
						#endif

						if(parameterGenerationFailed)
						{
							break;
						}
					}

					if(!parameterGenerationFailed)
					{
						// Try to create the instance with the generated parameter values.
						try
						{
							return constructor.Invoke(parameterValues);
						}
						#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
						catch(Exception e) { Debug.LogWarning("DefaultValue(" + StringUtils.ToString(type) + "): CreateInstance with "+ parameterCount + " parameters failed. Trying ("+(c == constructors.Length - 1 ? "GetUninitializedObject next" : "next constructor")+").\n"+e); }
						#else
						catch { }
						#endif
					}
				}
			}

			// if failed to generate instance using provided parameter, try to generate it without it
			return type.DefaultValueInternal();
		}

		public static Type[] GetTypesAccessibleFromContext([NotNull]this Type type)
		{
			var internalTypes = type.GetOnlyInternallyAccessibleTypes();
			var result = AllVisibleTypes.Join(internalTypes);
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(result.Length == allVisibleTypes.Length + internalTypes.Length);
			#endif
			return result;
		}
		
		/// <summary>
		/// Gets all nested types inside type as well as all types in type's assembly
		/// that are only internally accessible
		/// </summary>
		/// <param name="type">
		/// The type to act on. </param>
		/// <returns>
		/// An array of type.
		/// </returns>
		public static Type[] GetOnlyInternallyAccessibleTypes(this Type type)
		{
			//add all nested types
			var nestedTypes = type.GetNestedTypes(BindingFlags.NonPublic);
			var assemblyTypes = type.Assembly.GetOnlyInternallyAccessibleTypes();

			int count1 = nestedTypes.Length;
			if(count1 == 0)
			{
				return assemblyTypes;
			}
			int count2 = assemblyTypes.Length;
			if(count2 == 0)
			{
				return nestedTypes;
			}

			return assemblyTypes.Join(nestedTypes);
		}

		public static Type[] GetOnlyInternallyAccessibleTypes(this Assembly assembly)
		{
			Type[] result;
			if(HiddenTypesByAssembly.TryGetValue(assembly, out result))
			{
				return result;
			}

			var allAssemblyTypes = assembly.GetLoadableTypes(false);
			
			for(int n = allAssemblyTypes.Length - 1; n >= 0; n--)
			{
				var t = allAssemblyTypes[n];

				//skip all types that are visible to external assemblies
				if(t.IsVisible)
				{
					continue;
				}

				if(t.IsNested && !t.IsNestedPublic && !t.IsNestedAssembly)
				{
					//we only want to add nested public or internal types (skipping private, protected and protected internal)
					continue;
				}

				//skip some internally generated types
				var nestedClass = t.DeclaringType;
				if(nestedClass != null && nestedClass != t)
				{
					var nestedClassName = nestedClass.Name;
					if(nestedClassName.Length == 0 || nestedClassName[0] == '<')
					{
						#if DEV_MODE
						Debug.Log("SKIPPING: "+t.FullName+ " with nestedClass=" + nestedClass.FullName+", Name="+t.Name+", Namespace="+t.Namespace+", Assembly="+t.Assembly.FullName);
						#endif
						continue;
					}
				}

				var name = t.Name;
				if(name.Length == 0 || name[0] == '<')
				{
					#if DEV_MODE
					Debug.Log("SKIPPING: " + t.FullName + " with nestedClass=" + (nestedClass == null ? "null" : nestedClass.FullName) + ", Name=" + t.Name + ", Namespace=" + t.Namespace + ", Assembly=" + t.Assembly.FullName);
					#endif
					continue;
				}

				//add internal class , nested public or nested internal type
				ReusableTypeList.Add(t);
			}

			result = ReusableTypeList.ToArray();
			ReusableTypeList.Clear();

			HiddenTypesByAssembly.Add(assembly, result);

			return result;
		}
		
		private static Type[] GetLoadableTypes([NotNull]this Assembly assembly, bool exportedOnly)
		{
			try
			{
				Type[] result;
				if(exportedOnly)
				{
					result = assembly.GetExportedTypes();
				}
				else
				{
					result = assembly.GetTypes();
				}
				return result;
			}
			catch(NotSupportedException) //thrown if GetExportedTypes is called for a dynamic assembly
			{
				#if DEV_MODE
				Debug.LogWarning(assembly.GetName().Name+ ".GetLoadableTypes() NotSupportedException\n"+assembly.FullName);
				#endif
				return ArrayPool<Type>.ZeroSizeArray;
			}
			catch(ReflectionTypeLoadException e)
			{
				var exceptionTypes = e.Types;
				int count = exceptionTypes.Length;
				for(int n = count - 1; n >= 0; n--)
				{
					var type = exceptionTypes[n];
					if(type != null)
					{
						ReusableTypeList.Add(type);
					}
				}

				#if DEV_MODE
				Debug.LogWarning(assembly.GetName().Name+ ".GetLoadableTypes() ReflectionTypeLoadException, salvaged: " + ReusableTypeList.Count+ "\n" + assembly.FullName);
				#endif

				var result = ArrayPool<Type>.Create(ReusableTypeList);
				ReusableTypeList.Clear();
				return result;
			}
		}

		/// <summary>
		/// Gets all non-abstract UnityEngine.Object types that extend given Type
		/// </summary>
		public static Type[] GetExtendingUnityObjectTypes(this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(Types.UnityObject.IsAssignableFrom(type), "GetExtendingUnityObjectTypes was called for type "+StringUtils.ToString(type)+" which wasn't assignable from UnityEngine.Object.");
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type.IsClass, "GetExtendingUnityObjectTypes was called for " + StringUtils.ToString(type)+" which wasn't a class type. Value types and interfaces don't support inheritance.");
			#endif

			if(!setupDone)
			{
				Setup();
			}
			
			for(int n = componentTypes.Length - 1; n >= 0; n--)
			{
				var t = componentTypes[n];
				if(t.IsSubclassOf(type))
				{
					ReusableTypeList.Add(t);
				}
			}

			for(int n = scriptableObjectTypes.Length - 1; n >= 0; n--)
			{
				var t = scriptableObjectTypes[n];
				if(t.IsSubclassOf(type))
				{
					ReusableTypeList.Add(t);
				}
			}

			#if UNITY_EDITOR
			for(int n = editorTypes.Length - 1; n >= 0; n--)
			{
				var t = editorTypes[n];
				if(t.IsSubclassOf(type))
				{
					ReusableTypeList.Add(t);
				}
			}
			#endif
			
			for(int n = otherUnityObjectTypes.Length - 1; n >= 0; n--)
			{
				var t = otherUnityObjectTypes[n];
				if(t.IsSubclassOf(type))
				{
					ReusableTypeList.Add(t);
				}
			}
			
			if(includeInvisible)
			{
				for(int n = allInvisibleTypes.Length - 1; n >= 0; n--)
				{
					var t = allInvisibleTypes[n];
					if(t.IsSubclassOf(type))
					{
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			return result;
		}
		
		/// <summary>
		/// Gets all non-abstract UnityEngine.Object types that implement given interface
		/// </summary>
		public static Type[] GetImplementingUnityObjectTypes(this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type.IsInterface, "GetImplementingUnityObjectTypes was called for type " + type.Name+ " which wasn't an interface. Use GetExtendingUnityObjectTypes instead.");
			#endif

			if(!setupDone)
			{
				Setup();
			}

			for(int n = componentTypes.Length - 1; n >= 0; n--)
			{
				var t = componentTypes[n];
				if(type.IsAssignableFrom(t))
				{
					ReusableTypeList.Add(t);
				}
			}

			for(int n = scriptableObjectTypes.Length - 1; n >= 0; n--)
			{
				var t = scriptableObjectTypes[n];
				if(type.IsAssignableFrom(t))
				{
					ReusableTypeList.Add(t);
				}
			}

			#if UNITY_EDITOR
			for(int n = editorTypes.Length - 1; n >= 0; n--)
			{
				var t = editorTypes[n];
				if(type.IsAssignableFrom(t))
				{
					ReusableTypeList.Add(t);
				}
			}
			#endif

			for(int n = otherUnityObjectTypes.Length - 1; n >= 0; n--)
			{
				var t = otherUnityObjectTypes[n];
				if(type.IsAssignableFrom(t))
				{
					ReusableTypeList.Add(t);
				}
			}

			if(includeInvisible)
			{
				for(int n = allInvisibleTypes.Length - 1; n >= 0; n--)
				{
					var t = allInvisibleTypes[n];
					if(type.IsAssignableFrom(t))
					{
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			return result;
		}

		/// <summary>
		/// Gets all non-abstract Component types that extend given Type
		/// </summary>
		public static Type[] GetExtendingComponentTypes(this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(Types.Component.IsAssignableFrom(type), "GetExtendingComponentTypes was called for type " + StringUtils.ToString(type) + " which wasn't assignable from Component.");
			#endif
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type.IsClass, "GetExtendingUnityObjectTypes was called for " + StringUtils.ToString(type)+" which wasn't a class type. Value types and interfaces don't support inheritance.");
			#endif
			
			if(!setupDone)
			{
				Setup();
			}

			if(type == Types.Component && !includeInvisible)
			{
				return componentTypes;
			}

			for(int n = componentTypes.Length - 1; n >= 0; n--)
			{
				var t = componentTypes[n];
				if(t.IsSubclassOf(type))
				{
					ReusableTypeList.Add(t);
				}
			}

			if(includeInvisible)
			{
				for(int n = allInvisibleTypes.Length - 1; n >= 0; n--)
				{
					var t = allInvisibleTypes[n];
					if(t.IsSubclassOf(type))
					{
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			return result;
		}

		/// <summary>
		/// Gets all non-abstract Component types that implement given interface
		/// </summary>
		public static Type[] GetImplementingComponentTypes(this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type.IsInterface, "GetImplementingComponentTypes was called for type "+type.Name+ " which wasn't an interface. Use GetExtendingComponentTypes instead.");
			#endif

			if(!setupDone)
			{
				Setup();
			}

			if(type == Types.Component && !includeInvisible)
			{
				return componentTypes;
			}

			for(int n = componentTypes.Length - 1; n >= 0; n--)
			{
				var t = componentTypes[n];
				if(type.IsAssignableFrom(t) && t != type)
				{
					ReusableTypeList.Add(t);
				}
			}

			if(includeInvisible)
			{
				for(int n = allInvisibleTypes.Length - 1; n >= 0; n--)
				{
					var t = allInvisibleTypes[n];
					if(type.IsAssignableFrom(t))
					{
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			return result;
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Gets all non-abstract ScriptableObject types that extend given Type
		/// </summary>
		public static Type[] GetExtendingEditorTypes(this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(Types.Editor.IsAssignableFrom(type), "GetExtendingEditorTypes was called for type " + StringUtils.ToString(type) + " which wasn't assignable from Editor.");
			Debug.Assert(type.IsClass, "GetExtendingUnityObjectTypes was called for " + StringUtils.ToString(type)+" which wasn't a class type. Value types and interfaces don't support inheritance.");
			#endif

			if(!setupDone)
			{
				Setup();
			}
			
			if(type == Types.Editor)
			{
				if(includeInvisible)
				{
					return editorTypes.Join(invisibleEditorTypes);
				}
				return editorTypes;
			}

			for(int n = editorTypes.Length - 1; n >= 0; n--)
			{
				var t = editorTypes[n];
				if(t.IsSubclassOf(type))
				{
					ReusableTypeList.Add(t);
				}
			}

			if(includeInvisible)
			{
				for(int n = invisibleEditorTypes.Length - 1; n >= 0; n--)
				{
					var t = invisibleEditorTypes[n];
					if(t.IsSubclassOf(type))
					{
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);

			#if DEV_MODE
			Debug.Assert(result.Length == ReusableTypeList.Count);
			Debug.Log("GetExtendingEditorTypes("+type.Name+") found "+result.Length+" results:\n"+StringUtils.ToString(result, "\n"));
			#endif

			ReusableTypeList.Clear();
			return result;
		}
		#endif
		
		/// <summary>
		/// Gets all non-abstract types that implement given interface
		/// </summary>
		/// <param name="baseType"> abstract, interface or other base class type to act on. </param>
		/// <param name="implementingUnityObjectTypes"> [out] List of types of the implementing unity objects. </param>
		/// <param name="implementingNonUnityObjectTypes"> [out] List of types of the implementing non unity objects. </param>
		public static void GetImplementingTypes(this Type baseType, out Type[] implementingUnityObjectTypes, out Type[] implementingNonUnityObjectTypes)
		{
			if(!setupDone)
			{
				Setup();
			}

			implementingUnityObjectTypes = GetImplementingUnityObjectTypes(baseType, false);
			implementingNonUnityObjectTypes = GetImplementingNonUnityObjectClassTypes(baseType, false);
		}
		
		/// <summary>
		/// Gets all non-abstract types that are not assignable from UnityEngine.Object and extend given Type
		/// </summary>
		public static Type[] GetExtendingNonUnityObjectClassTypes(this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!Types.UnityObject.IsAssignableFrom(type), "GetExtendingEditorTypes was called for type " + StringUtils.ToString(type) + " which WAS assignable from UnityEngine.Object.");
			Debug.Assert(type.IsClass, "GetExtendingUnityObjectTypes was called for " + StringUtils.ToString(type)+" which wasn't a class type. Value types and interfaces don't support inheritance.");
			#endif

			if(!setupDone)
			{
				Setup();
			}

			for(int n = nonUnityObjectClassTypes.Length - 1; n >= 0; n--)
			{
				var t = nonUnityObjectClassTypes[n];
				if(t.IsSubclassOf(type))
				{
					ReusableTypeList.Add(t);
				}
			}

			if(includeInvisible)
			{
				for(int n = invisibleNonUnityObjectClassTypes.Length - 1; n >= 0; n--)
				{
					var t = invisibleNonUnityObjectClassTypes[n];
					if(t.IsSubclassOf(type))
					{
						//e.g. UnityEngine.Events.UnityEventBase
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			return result;
		}
		
		/// <summary>
		/// Gets all non-abstract class types that are not assignable from UnityEngine.Object and implement given interface
		/// </summary>
		public static Type[] GetImplementingNonUnityObjectClassTypes(this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type.IsInterface, "GetImplementingNonUnityObjectClassTypes was called for type " + type.Name+ " which wasn't an interface. Use GetExtendingNonUnityObjectClassTypes instead.");
			#endif

			if(!setupDone)
			{
				Setup();
			}

			for(int n = nonUnityObjectClassTypes.Length - 1; n >= 0; n--)
			{
				var t = nonUnityObjectClassTypes[n];
				if(type.IsAssignableFrom(t))
				{
					ReusableTypeList.Add(t);
				}
			}

			if(includeInvisible)
			{
				for(int n = invisibleNonUnityObjectClassTypes.Length - 1; n >= 0; n--)
				{
					var t = invisibleNonUnityObjectClassTypes[n];
					if(type.IsAssignableFrom(t))
					{
						//e.g. UnityEditor.IPreviewable!
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			return result;
		}

		/// <summary>
		/// Gets all non-abstract class types that are not assignable from UnityEngine.Object and implement given interface
		/// </summary>
		public static Type[] GetImplementingValueTypes(this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type.IsInterface, "GetImplementingValueTypes was called for type " + type.Name+ " which wasn't an interface.");
			#endif

			if(!setupDone)
			{
				Setup();
			}

			for(int n = nonEnumValueTypes.Length - 1; n >= 0; n--)
			{
				var t = nonEnumValueTypes[n];
				if(type.IsAssignableFrom(t))
				{
					ReusableTypeList.Add(t);
				}
			}

			if(includeInvisible)
			{
				for(int n = invisibleNonEnumValueTypes.Length - 1; n >= 0; n--)
				{
					var t = invisibleNonEnumValueTypes[n];
					if(type.IsAssignableFrom(t))
					{
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			return result;
		}

		/// <summary>
		/// Gets all non-abstract types that extend the provided base class type
		/// </summary>
		/// <param name="type"> The type for which all extending types are returned. </param>
		/// <param name="includeInvisible"> (Optional) True to also return invisible (not publically accessible) classes. </param>
		/// <returns> An array of extending types. </returns>
		public static Type[] GetExtendingInterfaceTypes([NotNull]this Type type, bool includeInvisible)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type.IsInterface);
			#endif

			if(!setupDone)
			{
				Setup();
			}

			for(int n = abstractTypes.Length - 1; n >= 0; n--)
			{
				var t = abstractTypes[n];
				if(t.IsAssignableFrom(type))
				{
					ReusableTypeList.Add(t);
				}
			}

			if(includeInvisible)
			{
				for(int n = invisibleAbstractTypes.Length - 1; n >= 0; n--)
				{
					var t = invisibleAbstractTypes[n];
					if(t.IsAssignableFrom(type))
					{
						ReusableTypeList.Add(t);
					}
				}
			}

			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			return result;
		}

		/// <summary>
		/// Gets all non-abstract types that extend the provided base class type
		/// </summary>
		/// <param name="type"> The type for which all extending types are returned. </param>
		/// <param name="includeInvisible"> (Optional) True to also return invisible (not publically accessible) classes. </param>
		/// <returns> An array of extending types. </returns>
		public static Type[] GetExtendingTypes([NotNull]this Type type, bool includeInvisible)
		{
			if(!setupDone)
			{
				Setup();
			}
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(type.IsClass, "GetExtendingTypes was called for " + StringUtils.ToString(type)+" which wasn't a class type. Value types and interfaces don't support inheritance.");
			#endif
			
			if(type.IsUnityObject())
			{
				if(type.IsComponent())
				{
					return GetExtendingComponentTypes(type, includeInvisible);
				}
				
				#if UNITY_EDITOR
				if(Types.Editor.IsAssignableFrom(type))
				{
					return GetExtendingEditorTypes(type, includeInvisible);
				}
				#endif
				
				return GetExtendingUnityObjectTypes(type, includeInvisible);
			}

			return GetExtendingNonUnityObjectClassTypes(type, includeInvisible);
		}

		public static Type GetEnumType(string name, string fullName)
		{
			KeyValuePair<string, Type>[] keyValuePairs;
			if(enumTypesByName.TryGetValue(name, out keyValuePairs))
			{
				for(int n = keyValuePairs.Length - 1; n >= 0; n--)
				{
					var pair = keyValuePairs[n];
					if(string.Equals(pair.Key, fullName))
					{
						return pair.Value;
					}
				}
			}
			return null;
		}
		
		public static Type GetRandomEnumType()
		{
			int index = UnityEngine.Random.Range(0, GetEnumTypeCount());
			int n = 0;
			foreach(var keyValuePair in enumTypesByName)
			{
				if(n == index)
				{
					var fullNameAndtype = keyValuePair.Value;
					return fullNameAndtype[UnityEngine.Random.Range(0, fullNameAndtype.Length)].Value;
				}
				n++;
			}
			throw new IndexOutOfRangeException();
		}

		private static int GetEnumTypeCount()
		{
			return enumTypesByName.Count;
		}
		
		/// <summary>
		/// Tries to find visible enum type in any assemblies.
		/// Supports finding by assembly qualified name, full name and class name
		/// </summary>
		/// <param name="name"> The assembly qualified name, full name or class name of the enum. </param>
		/// <returns>
		/// enum type matching name or null if not found
		/// </returns>
		public static Type GetEnumType(string name)
		{
			//UPDATE: support getting by assembly qualified name
			try
			{
				var result = Type.GetType(name);
				if(result != null)
				{
					return result;
				}
			}
			catch{}

			string fullName = null;
			int i = name.LastIndexOf('.');
			if(i != -1)
			{
				fullName = name;
				name = name.Substring(i+1);
			}

			KeyValuePair<string, Type>[] keyValuePairs;
			if(enumTypesByName.TryGetValue(name, out keyValuePairs))
			{
				if(fullName != null)
				{
					for(int n = keyValuePairs.Length - 1; n >= 0; n--)
					{
						var pair = keyValuePairs[n];
						if(string.Equals(pair.Key, fullName))
						{
							return pair.Value;
						}
					}
					#if DEV_MODE
					Debug.LogWarning("GetEnumType("+name+"): found Type that matches name but not namespace: "+ keyValuePairs[0].Key);
					#endif
					return null;
				}
				return keyValuePairs[0].Value;
			}
			return null;
		}

		/// <summary>
		/// Tries to find visible type in any assemblies.
		/// Supports finding by assembly qualified name, full name and class name
		/// </summary>
		/// <param name="name"> The assembly qualified name, full name or class name of the type. </param>
		/// <returns>
		/// type matching name or null if not found
		/// </returns>
		[CanBeNull]
		public static Type GetType(string name)
		{
			string fullName = null;
			int i = name.LastIndexOf('.');
			if(i != -1)
			{
				// assembly qualified name
				if(name.IndexOf(',') != -1)
				{
					try
					{
						return Type.GetType(name);
					}
					catch
					{
						return null;
					}
					
				}

				fullName = name;
				name = name.Substring(i+1);
			}

			KeyValuePair<string, Type>[] keyValuePairs;
			if(TypesByName.TryGetValue(name, out keyValuePairs))
			{
				if(fullName != null)
				{
					for(int n = keyValuePairs.Length - 1; n >= 0; n--)
					{
						var pair = keyValuePairs[n];
						if(string.Equals(pair.Key, fullName))
						{
							return pair.Value;
						}
					}
					#if DEV_MODE && DEBUG_GET_TYPE
					Debug.Log("GetType(\"" + name + "\") returning null because no match found for fullName=\"" + fullName + "\" in keyValuePairs:\n" + StringUtils.ToString(keyValuePairs, "\n"));
					#endif
					return null;
				}
				return keyValuePairs[0].Value;
			}

			#if DEV_MODE && DEBUG_GET_TYPE
			Debug.Log("GetType(\""+name+"\") returning null. Entry not found in among " + TypesByName.Count + " TypesByName entries.");
			#endif

			return null;
		}

		/// <summary>
		/// Tries to find visible type in any assemblies.
		/// Supports finding by assembly qualified name, full name and class name
		/// </summary>
		/// <param name="name"> The assembly qualified name, full name or class name of the type. </param>
		/// <param name="baseType"> Base type or interface of returned type. </param>
		/// <returns> Type matching name or null if not found. </returns>
		[CanBeNull]
		public static Type GetType(string name, [NotNull]Type baseType)
		{
			string fullName = null;
			int i = name.LastIndexOf('.');
			if(i != -1)
			{
				// assembly qualified name
				if(name.IndexOf(',') != -1)
				{
					try
					{
						return Type.GetType(name);
					}
					catch
					{
						return null;
					}
					
				}

				fullName = name;
				name = name.Substring(i+1);
			}

			KeyValuePair<string, Type>[] keyValuePairs;
			if(TypesByName.TryGetValue(name, out keyValuePairs))
			{
				if(fullName != null)
				{
					for(int n = keyValuePairs.Length - 1; n >= 0; n--)
					{
						var pair = keyValuePairs[n];
						if(string.Equals(pair.Key, fullName))
						{
							if(baseType.IsAssignableFrom(pair.Value))
							{
								return pair.Value;
							}
						}
					}
					#if DEV_MODE && DEBUG_GET_TYPE
					Debug.Log("GetType(\""+name+"\") returning null because no match with fullName=\""+fullName+ "\" and assignable from " + baseType.FullName + " found in keyValuePairs:\n" + StringUtils.ToString(keyValuePairs, "\n"));
					#endif
					return null;
				}

				for(int n = keyValuePairs.Length - 1; n >= 0; n--)
				{
					var pair = keyValuePairs[n];
					if(baseType.IsAssignableFrom(pair.Value))
					{
						return pair.Value;
					}
				}

				#if DEV_MODE && DEBUG_GET_TYPE
				Debug.Log("GetType(\""+name+"\") returning null because no type assignable from "+ baseType.FullName + " found in keyValuePairs:\n" + StringUtils.ToString(keyValuePairs, "\n"));
				#endif
				return null;
			}

			#if DEV_MODE && DEBUG_GET_TYPE
			Debug.Log("GetType(\""+name+"\", "+ baseType.FullName+") returning null. Entry not found in among " + TypesByName.Count + " TypesByName entries.");
			#endif

			return null;
		}

		/// <summary>
		/// Gets all non-abstract types that implement the provided interface type.
		/// </summary>
		/// <param name="interfaceType">
		/// The interface for which all implementing types are returned.
		/// This can not be a generic type definition.
		/// E.g. IList{int} is legal, but IList{T} is not.
		/// </param>
		/// <param name="includeInvisible"> (Optional) True to also return invisible (not publically accessible) classes. </param>
		/// <returns> Array of implementing types. </returns>
		public static Type[] GetImplementingTypes([NotNull]this Type interfaceType, bool includeInvisible = false)
		{
			return interfaceType.IsGenericTypeDefinition ? GetImplementingTypesForInterfaceGenericTypeDefinition(interfaceType, includeInvisible) : GetImplementingTypesForInterfaceNotGenericTypeDefinition(interfaceType, includeInvisible);
		}

		/// <summary>
		/// Gets all non-abstract types that implement the provided interface type
		/// </summary>
		/// <param name="interfaceType"> The interfaceType target for which all implementing types are returned. </param>
		/// <param name="includeInvisible"> (Optional) True to also return invisible (not publically accessible) classes. </param>
		/// <returns> Array of implementing types. </returns>
		public static Type[] GetImplementingTypesForInterfaceNotGenericTypeDefinition([NotNull]this Type interfaceType, bool includeInvisible = false)
		{
			if(!setupDone)
			{
				Setup();
			}

			#if DEV_MODE && PI_ASSERTATIONS
			if(interfaceType.IsGenericTypeDefinition) { Debug.LogError("GetImplementingTypesForInterfaceNotGenericTypeDefinition called with generic type: "+StringUtils.ToStringSansNamespace(interfaceType)); }
			if(!interfaceType.IsInterface) { Debug.LogError("GetImplementingTypesForInterfaceNotGenericTypeDefinition called with type which was not an interface: "+StringUtils.ToStringSansNamespace(interfaceType)); }
			#endif

			for(int n = allVisibleTypes.Length - 1; n >= 0; n--)
			{
				var type = allVisibleTypes[n];
				if(interfaceType.IsAssignableFrom(type))
				{
					ReusableTypeList.Add(type);
				}
			}

			if(includeInvisible)
			{
				for(int n = allInvisibleTypes.Length - 1; n >= 0; n--)
				{
					var type = allInvisibleTypes[n];
					if(interfaceType.IsAssignableFrom(type))
					{
						ReusableTypeList.Add(type);
					}
				}
			}
			
			var result = ArrayPool<Type>.Create(ReusableTypeList);
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(result.Length == ReusableTypeList.Count);
			#endif

			ReusableTypeList.Clear();

			return result;
		}
		
		/// <summary>
		/// Gets all non-abstract types that implement the given generic type definition of an interface.
		/// 
		/// E.g. If 'interfaceGenericTypeDefinition' is of type IList{T}, then returns types that implement IList{int}, IList{string}, List{float} etc.
		/// </summary>
		/// <param name="interfaceGenericTypeDefinition"> The generic type definition of an interface for which all implementing types are returned. </param>
		/// <param name="includeInvisible"> (Optional) True to also return invisible (not publically accessible) classes. </param>
		/// <returns> Array of implementing types. </returns>
		public static Type[] GetImplementingTypesForInterfaceGenericTypeDefinition([NotNull]this Type interfaceGenericTypeDefinition, bool includeInvisible = false)
		{
			if(!setupDone)
			{
				Setup();
			}

			#if DEV_MODE && PI_ASSERTATIONS
			if(!interfaceGenericTypeDefinition.IsGenericTypeDefinition) { Debug.LogError("GetImplementingTypesForGenericInterface called with type which was not a generic type definition: "+StringUtils.ToStringSansNamespace(interfaceGenericTypeDefinition)); }
			if(!interfaceGenericTypeDefinition.IsInterface) { Debug.LogError("GetImplementingTypesForGenericInterface called with type which was not an interface: "+StringUtils.ToStringSansNamespace(interfaceGenericTypeDefinition)); }
			#endif

			// Optimizations to do:
			// could skip enums and delegates...

			for(int n = allVisibleTypes.Length - 1; n >= 0; n--)
			{
				var type = allVisibleTypes[n];
				var testInterfaces = type.GetInterfaces();
				for(int i = testInterfaces.Length - 1; i >= 0; i--)
				{
					var testType = testInterfaces[i];
					do
					{
						if(testType.IsGenericType)
						{
							if(testType.IsGenericTypeDefinition)
							{
								if(testType == interfaceGenericTypeDefinition)
								{
									#if DEV_MODE && PI_ASSERTATIONS
									Debug.Assert(!ReusableTypeList.Contains(type), "List already contained "+StringUtils.ToStringSansNamespace(type)+"!");
									#endif
									ReusableTypeList.Add(type);
								}
							}
							else if(testType.GetGenericTypeDefinition() == interfaceGenericTypeDefinition)
							{
								ReusableTypeList.Add(type);
							}
						}
						testType = testType.BaseType;
					}
					while(testType != null);
				}
			}

			if(includeInvisible)
			{
				for(int n = allInvisibleTypes.Length - 1; n >= 0; n--)
				{
					var type = allInvisibleTypes[n];
					var testInterfaces = type.GetInterfaces();
					for(int i = testInterfaces.Length - 1; i >= 0; i--)
					{
						var testType = testInterfaces[i];
						do
						{
							if(testType == interfaceGenericTypeDefinition)
							{
								ReusableTypeList.Add(type);
							}
							testType = testType.BaseType;
						}
						while(testType != null);
					}
				}
			}
			
			var result = ArrayPool<Type>.Create(ReusableTypeList);
			ReusableTypeList.Clear();
			
			return result;
		}
		
		public static bool IsAssetType(this Type type)
		{
			if(Types.ScriptableObject.IsAssignableFrom(type))
			{
				return true;
			}

			if(Types.Motion.IsAssignableFrom(type))
			{
				return true;
			}

			if(type == Types.AudioClip)
			{
				return true;
			}

			if(type == typeof(TextAsset))
			{
				return true;
			}
			
			return false;
		}

		public static string GetNamespaceParent(string namespaceString)
		{
			int i = namespaceString.LastIndexOf('.');
			if(i == -1)
			{
				return namespaceString;
			}
			return namespaceString.Substring(0, i);
		}

		public static string[] GetPopupMenuLabels(Type[] types)
		{
			int count = types.Length;
			var labels = new string[count];
			for(int n = count - 1; n >= 0; n--)
			{
				var type = types[n];
				var label = GetPopupMenuLabel(type);
				labels[n] = label;
			}
			return labels;
		}
		
		public static string GetPopupMenuLabel(Type type, string globalNamespaceTypePrefix)
		{
			if(type.Namespace == null)
			{
				return string.Concat(globalNamespaceTypePrefix, GetPopupMenuLabel(type));
			}
			return GetPopupMenuLabel(type);
		}
		
		public static string GetPopupMenuLabel([NotNull]Type type)
		{
			string result;
			if(TypePopupMenuPaths.TryGetValue(type, out result))
			{
				return result;
			}
			
			var menuPath = StringUtils.ToString(type, '/');
			TypePopupMenuPaths.Add(type, menuPath);
			TypeFullNames.Add(type, menuPath.Replace('/', '.'));
			TypeNames.Add(type, StringUtils.ToString(type, '\0'));

			#if DEV_MODE && DEBUG_CACHE_NEW_TYPE_NAME
			Debug.Log("Cached new type name: "+menuPath);
			#endif
			return menuPath;
		}

		public static string GetFullName([NotNull]Type type)
		{
			string result;
			if(TypeFullNames.TryGetValue(type, out result))
			{
				return result;
			}
			
			var menuPath = StringUtils.ToString(type, '/');
			TypePopupMenuPaths.Add(type, menuPath);
			string fullName = menuPath.Replace('/', '.');
			TypeFullNames.Add(type, fullName);
			TypeNames.Add(type, StringUtils.ToString(type, '\0'));

			#if DEV_MODE && DEBUG_CACHE_NEW_TYPE_NAME
			Debug.Log("Cached new type name: "+menuPath);
			#endif
			return fullName;
		}

		public static string GetShortName([NotNull]Type type)
		{
			string result;
			if(TypeNames.TryGetValue(type, out result))
			{
				return result;
			}
			
			var menuPath = StringUtils.ToString(type, '/');
			TypePopupMenuPaths.Add(type, menuPath);
			TypeFullNames.Add(type, menuPath.Replace('/', '.'));
			string shortName = StringUtils.ToString(type, '\0');
			TypeNames.Add(type, shortName);

			#if DEV_MODE && DEBUG_CACHE_NEW_TYPE_NAME
			Debug.Log("Cached new type name: "+menuPath);
			#endif

			return shortName;
		}

		public static bool ContainsSerializedObjectReferenceFields([NotNull]this Type type, ref bool depthLimitReached, int depthLimit = 7)
		{
			if(type.IsPrimitive)
			{
				#if DEV_MODE && DEBUG_CONTAINS_OBJECT_REFERENCES
				Debug.Log("ContainsSerializedObjectReferenceFields("+StringUtils.ToStringSansNamespace(type)+"): "+StringUtils.False + " (Primitive)");
				#endif
				return false;
			}

			if(type.IsUnityObject())
			{
				#if DEV_MODE && DEBUG_CONTAINS_OBJECT_REFERENCES
				Debug.Log("ContainsSerializedObjectReferenceFields("+StringUtils.ToStringSansNamespace(type)+"): "+StringUtils.True + " (this.IsUnityObject)");
				#endif
				return true;
			}
			
			if(depthLimit <= 0)
			{
				#if DEV_MODE
				Debug.LogWarning("ContainsSerializedObjectReferenceFields returning false for type "+type.Name+" depth limit was reached; possible recursive self-reference");
				#endif
				depthLimitReached = true;
				return false;
			}

			if(type.IsArray)
			{
				var elementTypeContains = type.GetElementType().ContainsSerializedObjectReferenceFields(ref depthLimitReached, depthLimit - 1);
				#if DEV_MODE && DEBUG_CONTAINS_OBJECT_REFERENCES
				Debug.Log("ContainsSerializedObjectReferenceFields("+StringUtils.ToStringSansNamespace(type)+"): "+StringUtils.ToColorizedString(elementTypeContains) + " (via array element type) with depthLimit="+StringUtils.ToColorizedString(depthLimit)+", depthLimitReached="+StringUtils.ToColorizedString(depthLimitReached));
				#endif
				return elementTypeContains;
			}

			if(type.IsGenericType)
			{
				var genericArguments = type.GetGenericArguments();
				for(int n = genericArguments.Length - 1; n >= 0; n--)
				{
					if(genericArguments[n].ContainsSerializedObjectReferenceFields(ref depthLimitReached, depthLimit - 1))
					{
						#if DEV_MODE && DEBUG_CONTAINS_OBJECT_REFERENCES
						Debug.Log("ContainsSerializedObjectReferenceFields("+StringUtils.ToStringSansNamespace(type)+"): "+StringUtils.False + " (via generic argument "+StringUtils.ToStringSansNamespace(genericArguments[n])+") with depthLimit="+StringUtils.ToColorizedString(depthLimit)+", depthLimitReached="+StringUtils.ToColorizedString(depthLimitReached));
						#endif
						return true;
					}
				}
			}

			var fields = type.GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			for(int n = fields.Length - 1; n >= 0; n--)
			{
				var field = fields[n];
				if(field.FieldType.ContainsSerializedObjectReferenceFields(ref depthLimitReached, depthLimit - 1))
				{
					#if DEV_MODE && DEBUG_CONTAINS_OBJECT_REFERENCES
					Debug.Log("ContainsSerializedObjectReferenceFields("+StringUtils.ToStringSansNamespace(type)+"): "+StringUtils.True + " (via field \""+field.Name+"\" of type "+StringUtils.ToStringSansNamespace(field.FieldType)+") with depthLimit="+StringUtils.ToColorizedString(depthLimit)+", depthLimitReached="+StringUtils.ToColorizedString(depthLimitReached));
					#endif
					return true;
				}
			}

			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			for(int n = properties.Length - 1; n >= 0; n--)
			{
				var property = properties[n];
				if(property.PropertyType.ContainsSerializedObjectReferenceFields(ref depthLimitReached, depthLimit - 1))
				{
					#if DEV_MODE && DEBUG_CONTAINS_OBJECT_REFERENCES
					Debug.Log("ContainsSerializedObjectReferenceFields("+StringUtils.ToStringSansNamespace(type)+"): "+StringUtils.True + " (via property \""+property.Name+"\" of type "+StringUtils.ToStringSansNamespace(property.PropertyType)+") with depthLimit="+StringUtils.ToColorizedString(depthLimit)+", depthLimitReached="+StringUtils.ToColorizedString(depthLimitReached));
					#endif
					return true;
				}
			}

			#if DEV_MODE && DEBUG_CONTAINS_OBJECT_REFERENCES
			Debug.Log("ContainsSerializedObjectReferenceFields("+StringUtils.ToStringSansNamespace(type)+"): "+StringUtils.False + " (tested "+fields.Length+" fields and "+properties.Length+" properties)\n\nfields:\n"+StringUtils.ToString(fields, "\n")+"\n\nproperties:\n"+StringUtils.ToString(properties, "\n"));
			#endif

			return false;
		}
		
		public static bool NestedMemberCountExceeds([NotNull]this Type type, ref int maxCount, int maxDepth = 2, bool includeProperties = true)
		{
			if(type.IsPrimitive)
			{
				return false;
			}
			
			if(maxDepth <= 0)
			{
				#if DEV_MODE
				Debug.LogWarning("NestedMemberCountExceeds returning false for type " + type.Name+" depth limit was reached; possible recursive self-reference");
				#endif
				return true;
			}

			if(type.IsArray)
			{
				return type.GetElementType().NestedMemberCountExceeds(ref maxCount, maxDepth);
			}
			
			var fields = type.GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			int fieldCount = fields.Length;
			maxCount -= fieldCount;
			if(maxCount <= 0)
			{
				return true;
			}

			if(includeProperties)
			{
				var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				int propertyCount = properties.Length;
				maxCount -= propertyCount;
				if(maxCount <= 0)
				{
					return true;
				}
				
				for(int n = propertyCount - 1; n >= 0; n--)
				{
					if(properties[n].PropertyType.NestedMemberCountExceeds(ref maxCount, maxDepth - 1))
					{
						return true;
					}
				}
			}

			for(int n = fieldCount - 1; n >= 0; n--)
			{
				if(fields[n].FieldType.NestedMemberCountExceeds(ref maxCount, maxDepth - 1))
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// Tests whether or not 'testType' is subclass of given undeclared generic type.
		/// </summary>
		/// <param name="testType"> The type to test. </param>
		/// <param name="undeclaredGenericType"> An undeclared generic type. </param>
		/// <returns> True if testType derives from undeclaredGenericType, false if not. </returns>
		/// <example>
		/// <code>
		/// public class SomeValue&lt;T>
		/// {
		///		public T value;
		/// }
		/// 
		/// public class IntValue : SomeValue&lt;int>
		/// {
		///		public void TestIsSubclassOfSomeValue()
		///		{
		///			Debug.Log(GetType().IsSubclassOfUndeclaredGeneric(typeof(SomeValue&lt;int>))); //prints "True"
		///		}
		/// }
		/// </code>
		/// </example>
		public static bool IsSubclassOfUndeclaredGeneric([NotNull]this Type testType, Type undeclaredGenericType)
		{
			do
			{
				var test = testType.IsGenericType ? testType.GetGenericTypeDefinition() : testType;
				if(test == undeclaredGenericType)
				{
					return true;
				}

				testType = testType.BaseType;
			}
			while(testType != Types.SystemObject && testType != null);
 
			return false;
		}

		/// <summary>
		/// If 'type' is generic type, returns its generic type definition.
		/// If any inherited type of 'type' is generic type, returns generic type definition of said inherited type.
		/// Otherwise returns null.
		/// </summary>
		/// <param name="type"> The type to test. </param>
		/// <returns> True if testType derives from undeclaredGenericType, false if not. </returns>
		[CanBeNull]
		public static Type GetGenericTypeDefinitionIncludingInherited([NotNull]this Type type)
		{
			do
			{
				if(type.IsGenericType)
				{
					return type.GetGenericTypeDefinition();
				}
				type = type.BaseType;
			}
			while(type != Types.SystemObject && type != null);
 
			return null;
		}

		/// <summary>
		/// If 'type' is generic type, returns its generic type definition.
		/// If any inherited type of 'type' is generic, returns generic argument of said type.
		/// If any implemented interface of type 'type' is generic, returns generic argument of said type.
		/// Otherwise returns null.
		/// </summary>
		/// <param name="type"> The type whose generic argument we are trying to get. </param>
		/// <returns> Generic argument of type, its inherited type or an interface that it implements. </returns>
		[CanBeNull]
		public static Type GetGenericArgumentOrInheritedGenericArgument([NotNull]this Type type)
		{
			var testType = type;
			do
			{
				if(testType.IsGenericType)
				{
					var genericArguments = testType.GetGenericArguments();
					if(genericArguments.Length == 1)
					{
						return genericArguments[0];
					}
				}
				testType = testType.BaseType;
			}
			while(testType != Types.SystemObject && testType != null);
 
			if(!type.IsInterface)
			{
				var interfaces = type.GetInterfaces();
				for(int n = interfaces.Length - 1; n >= 0; n--)
				{
					var genericArgument = GetGenericArgumentOrInheritedGenericArgument(interfaces[n]);
					if(genericArgument != null)
					{
						return genericArgument;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// If 'type' is generic type, returns its generic type definition.
		/// If any inherited type of 'type' is generic type, returns generic type definition of said inherited type.
		/// Otherwise returns null.
		/// </summary>
		/// <param name="type"> The type to test. </param>
		/// <returns> True if testType derives from undeclaredGenericType, false if not. </returns>
		[CanBeNull]
		public static Type GetGenericTypeDefinitionIncludingInheritedAndInterfaces([NotNull]this Type type)
		{
			do
			{
				if(type.IsGenericType)
				{
					return type.GetGenericTypeDefinition();
				}
				type = type.BaseType;
			}
			while(type != Types.SystemObject && type != null);
 
			return null;
		}

		/// <summary> Gets all Components in hierarchy that implement the given interface type. </summary>
		/// <param name="interfaceType"> Type of the interface. </param>
		/// <param name="addFoundToList"> List into which found Objects are added, in no particular order. </param>
		public static void FindObjectsImplementingInterface(this Type interfaceType, [NotNull]List<Object> addFoundToList)
		{
			var types = interfaceType.GetImplementingComponentTypes(false);
			for(int t = 0, count = types.Length; t < count; t++)
			{
				var type = types[t];
				var found = Object.FindObjectsOfType(type);
				if(found.Length > 0)
				{
					addFoundToList.AddRange(found);
				}
			}
		}

		/// <summary> Gets all Components in hierarchy that implement the given interface type. </summary>
		/// <param name="interfaceType"> Type of the interface. </param>
		/// <param name="foundComponents"> List into which found Components are added, in no particular order. </param>
		/// <param name="foundOther"> List into which found non-Component UnityEngine.Objects are added, in no particular order. </param>
		public static void FindObjectsImplementingInterface(this Type interfaceType, [NotNull]List<Component> foundComponents, [NotNull]List<Object> foundOther)
		{
			var types = interfaceType.GetImplementingUnityObjectTypes(false);
			for(int t = 0, typeCount = types.Length; t < typeCount; t++)
			{
				var type = types[t];
				if(type.IsComponent())
				{
					var foundObjects = Object.FindObjectsOfType(type);
					if(foundObjects.Length > 0)
					{
						foundComponents.AddRange(foundObjects as Component[]);
					}
					continue;
				}
				
				#if UNITY_EDITOR
				var assetGuids = AssetDatabase.FindAssets("t:" + type.Name);
				for(int a = 0, assetCount = assetGuids.Length; a < assetCount; a++)
				{
					var guid = assetGuids[a];
					var path = AssetDatabase.GUIDToAssetPath(guid);
					foundOther.Add(AssetDatabase.LoadAssetAtPath(path, type));
				}
				#endif
			}
		}

		public static int GetTypeId([NotNull]Type type)
		{
			if(type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				var genericTypeDefinition = type.GetGenericTypeDefinition();

				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(genericTypeDefinition != type, "type "+StringUtils.ToString(type)+" GetGenericTypeDefinition returned "+StringUtils.ToString(genericTypeDefinition));
				#endif

				var genericTypeDefinitionId = GetTypeId(genericTypeDefinition);
				
				int typeCount = allVisibleTypes.Length + allInvisibleTypes.Length;
				int idComponentMultiplier = typeCount;

				int result = genericTypeDefinitionId;

				var genericTypes = type.GetGenericArguments();
				int genericTypeCount = genericTypes.Length;
				for(int n = 0; n < genericTypeCount; n++)
				{
					var genericType = genericTypes[n];
					int genericTypeId = GetTypeId(genericType);

					result += genericTypeId * idComponentMultiplier;
					idComponentMultiplier *= typeCount;
				}

				#if DEV_MODE
				Debug.Log("GetTypeUniqueId("+type.Name+"): "+result+" with genericTypes="+StringUtils.ToString(genericTypes));
				#endif
				return result;
			}

			int id;
			if(TypeIds.TryGet(type, out id))
			{
				return id;
			}
			
			id = TypeIds.Count;
			TypeIds.Add(type, id);

			#if DEV_MODE
			Debug.LogWarning("GetTypeUniqueId("+type.Name+") did not exist yet: "+id);
			#endif

			return id;
		}

		[CanBeNull]
		public static Type GetTypeById(int id)
		{
			#if DEV_MODe && PI_ASSERTATIONS
			Debug.Assert(id >= 0);
			#endif

			Type type;
			if(TypeIds.TryGetBySecond(id, out type))
			{
				return type;
			}
			return null;
		}
		
		public static bool IsCollection(this Type type)
		{
			return typeof(ICollection).IsAssignableFrom(type);
		}

		[NotNull]
		public static Type[] GetGenericArgumentsFromInterface(this Type sourceType, Type interfaceTypeDefinition)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(interfaceTypeDefinition.IsInterface);
			#endif

			//handle special case where sourceType generic type definition equals interface type definition
			if(sourceType.IsInterface && sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == interfaceTypeDefinition)
			{
				return sourceType.GetGenericArguments();
			}
			
			// handle class types
			var allInterfaces = sourceType.GetInterfaces();
			for(int n = allInterfaces.Length - 1; n >= 0; n--)
			{
				var testInterface = allInterfaces[n];
				if(testInterface.IsGenericType && testInterface.GetGenericTypeDefinition() == interfaceTypeDefinition)
				{
					return testInterface.GetGenericArguments();
				}
			}

			return ArrayPool<Type>.ZeroSizeArray;
		}

		[NotNull]
		public static Type[] GetGenericArgumentsFromBaseClass(this Type sourceType, Type baseClassTypeDefinition)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!baseClassTypeDefinition.IsInterface);
			#endif

			for(var type = sourceType; type != null; type = type.BaseType)
			{
				if(type.IsGenericType && type.GetGenericTypeDefinition() == baseClassTypeDefinition)
				{
					return type.GetGenericArguments();
				}
			}
			
			return ArrayPool<Type>.ZeroSizeArray;
		}

		public static bool TryGetGenericArgumentsFromInterface([NotNull]this Type sourceType, [NotNull]Type interfaceTypeDefinition, [NotNull]out Type[] genericArguments)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(interfaceTypeDefinition.IsInterface);
			#endif

			//handle special case where sourceType generic type definition equals interface type definition
			if(sourceType.IsInterface && sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == interfaceTypeDefinition)
			{
				genericArguments = sourceType.GetGenericArguments();
				return true;
			}
			
			// handle class types
			var allInterfaces = sourceType.GetInterfaces();
			for(int n = allInterfaces.Length - 1; n >= 0; n--)
			{
				var testInterface = allInterfaces[n];
				if(testInterface.IsGenericType && testInterface.GetGenericTypeDefinition() == interfaceTypeDefinition)
				{
					genericArguments = testInterface.GetGenericArguments();
					return true;
				}
			}

			genericArguments = ArrayPool<Type>.ZeroSizeArray;
			return false;
		}

		public static bool TryGetGenericArgumentsFromBaseClass([NotNull]this Type sourceType, [NotNull]Type baseClassTypeDefinition, [NotNull]out Type[] genericArguments)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!baseClassTypeDefinition.IsInterface);
			#endif

			for(var type = sourceType; type != null; type = type.BaseType)
			{
				if(type.IsGenericType && type.GetGenericTypeDefinition() == baseClassTypeDefinition)
				{
					genericArguments = type.GetGenericArguments();
					return true;
				}
			}
			
			genericArguments = ArrayPool<Type>.ZeroSizeArray;
			return false;
		}

		public static bool IsBaseComponentType(this Type componentType)
		{
			return componentType == Types.Collider || componentType == Types.Collider2D || componentType == Types.Renderer;
		}

		public static void GetInspectorViewables(this Type type, ref LinkedMemberHierarchy hierarchy, [CanBeNull]LinkedMemberInfo parentMemberInfo, [NotNull]ref List<LinkedMemberInfo> results, BindingFlags bindingFlags = ParentDrawerUtility.BindingFlagsDeclaredInstance, bool includeHidden = false, FieldVisibility includeNonSerializedFields = FieldVisibility.SerializedOnly, PropertyVisibility propertyVisibility = PropertyVisibility.AttributeExposedOnly, MethodVisibility methodVisibility = MethodVisibility.AttributeExposedOnly, LinkedMemberParent parentType = LinkedMemberParent.UnityObject)
		{
			bool notUnitySerialized = parentType == LinkedMemberParent.Static || hierarchy.Target == null;

			var order = InspectorUtility.Preferences.MemberDisplayOrder;
			for(int t = 0; t < 3; t++)
			{
				switch(order[t])
				{
					case Member.Field:
						var fields = type.GetFields(bindingFlags);
						for(int n = 0, count = fields.Length; n < count; n++)
						{
							var field = fields[n];
							if(field.IsInspectorViewable(includeHidden, includeNonSerializedFields))
							{
								#if DEV_MODE && PI_ASSERTATIONS
								Debug.Assert(!field.IsStatic || parentType == LinkedMemberParent.Static);
								#endif

								var linkedMemberInfo = hierarchy.Get(parentMemberInfo, field, parentType, notUnitySerialized ? null : field.Name);

								if((linkedMemberInfo.HasAttribute<Attributes.InlineAttribute>(false, true) || field.FieldType.GetCustomAttributes(typeof(Attributes.InlineAttribute), false).Length > 0) && !field.FieldType.IsUnityObject())
								{
									field.FieldType.GetInspectorViewables(ref hierarchy, linkedMemberInfo, ref results, bindingFlags, includeHidden, includeNonSerializedFields, propertyVisibility, methodVisibility, LinkedMemberParent.LinkedMemberInfo);
								}
								else
								{
									results.Add(linkedMemberInfo);
								}
							}
						}
						break;
					case Member.Property:
						var properties = type.GetProperties(bindingFlags);
						for(int n = 0, count = properties.Length; n < count; n++)
						{
							var property = properties[n];
							if(property.IsInspectorViewable(propertyVisibility, includeHidden))
							{
								#if DEV_MODE && PI_ASSERTATIONS
								Debug.Assert(!property.IsStatic() || parentType == LinkedMemberParent.Static);
								#endif

								var linkedMemberInfo = hierarchy.Get(parentMemberInfo, property, parentType, notUnitySerialized ? null : property.Name);

								if((linkedMemberInfo.HasAttribute<Attributes.InlineAttribute>(false, true) || property.PropertyType.GetCustomAttributes(typeof(Attributes.InlineAttribute), false).Length > 0) && !property.PropertyType.IsUnityObject())
								{
									property.PropertyType.GetInspectorViewables(ref hierarchy, linkedMemberInfo, ref results, bindingFlags, includeHidden, includeNonSerializedFields, propertyVisibility, methodVisibility, LinkedMemberParent.LinkedMemberInfo);
								}
								else
								{
									results.Add(linkedMemberInfo);
								}
							}
						}
						break;
					case Member.Method:
						var methods = type.GetMethods(bindingFlags);
						for(int n = 0, count = methods.Length; n < count; n++)
						{
							var method = methods[n];
							if(method.IsInspectorViewable(methodVisibility, includeHidden))
							{
								#if DEV_MODE && PI_ASSERTATIONS
								Debug.Assert(!method.IsStatic() || parentType == LinkedMemberParent.Static);
								#endif
								results.Add(hierarchy.Get(parentMemberInfo, method, parentType));
							}
						}
						break;
				}
			}

			#if DEV_MODE && DEBUG_GET_INSPECTOR_VIEWABLES
			/*if(results.Count > 1)*/ { Debug.Log(type.Name+".GetInspectorViewables: returning "+results.Count+" MemberInfos:\n"+StringUtils.ToString(results, "\n")); }
			#endif
		}

		public static void GetInspectorViewableFields(this Type type, ref LinkedMemberHierarchy hierarchy, [CanBeNull]LinkedMemberInfo parentMemberInfo, [NotNull]ref List<LinkedMemberInfo> results, BindingFlags bindingFlags, bool includeHidden, FieldVisibility includeNonSerializedFields, LinkedMemberParent parentType)
		{
			bool notUnitySerialized = parentType == LinkedMemberParent.Static || hierarchy.Target == null;

			var fields = type.GetFields(bindingFlags);
			for(int n = 0, count = fields.Length; n < count; n++)
			{
				var field = fields[n];
				if(field.IsInspectorViewable(includeHidden, includeNonSerializedFields))
				{
					#if DEV_MODE && PI_ASSERTATIONS
					Debug.Assert(!field.IsStatic || parentType == LinkedMemberParent.Static);
					#endif

					results.Add(hierarchy.Get(parentMemberInfo, field, parentType, notUnitySerialized ? null : field.Name));
				}
			}
			
			#if DEV_MODE && DEBUG_GET_INSPECTOR_VIEWABLES
			if(results.Count > 4) { Debug.Log(type.Name+".GetInspectorViewables: returning "+results.Count+" MemberInfos:\n"+StringUtils.ToString(results, "\n")); }
			#endif
		}

		public static void GetInspectorViewableProperties(this Type type, ref LinkedMemberHierarchy hierarchy, [CanBeNull]LinkedMemberInfo parentMemberInfo, [NotNull]ref List<LinkedMemberInfo> results, BindingFlags bindingFlags, bool includeHidden, PropertyVisibility propertyVisibility, LinkedMemberParent parentType)
		{
			bool notUnitySerialized = parentType == LinkedMemberParent.Static || hierarchy.Target == null;

			var properties = type.GetProperties(bindingFlags);
			for(int n = 0, count = properties.Length; n < count; n++)
			{
				var property = properties[n];
				if(property.IsInspectorViewable(propertyVisibility, includeHidden))
				{
					#if DEV_MODE && PI_ASSERTATIONS
					Debug.Assert(!property.IsStatic() || parentType == LinkedMemberParent.Static);
					#endif
					results.Add(hierarchy.Get(parentMemberInfo, property, parentType, notUnitySerialized ? null : property.Name));
				}
			}

			#if DEV_MODE && DEBUG_GET_INSPECTOR_VIEWABLES
			if(results.Count > 4) { Debug.Log(type.Name+".GetInspectorViewables: returning "+results.Count+" MemberInfos:\n"+StringUtils.ToString(results, "\n")); }
			#endif
		}

		public static void GetInspectorViewableMethods(this Type type, ref LinkedMemberHierarchy hierarchy, [CanBeNull]LinkedMemberInfo parentMemberInfo, [NotNull]ref List<LinkedMemberInfo> results, BindingFlags bindingFlags, bool includeHidden, MethodVisibility methodVisibility, LinkedMemberParent parentType)
		{
			var methods = type.GetMethods(bindingFlags);
			for(int n = 0, count = methods.Length; n < count; n++)
			{
				var method = methods[n];
				if(method.IsInspectorViewable(methodVisibility, includeHidden))
				{
					#if DEV_MODE && PI_ASSERTATIONS
					Debug.Assert(!method.IsStatic() || parentType == LinkedMemberParent.Static);
					#endif
					results.Add(hierarchy.Get(parentMemberInfo, method, parentType));
				}
			}

			#if DEV_MODE && DEBUG_GET_INSPECTOR_VIEWABLES
			if(results.Count > 4) { Debug.Log(type.Name+".GetInspectorViewables: returning "+results.Count+" MemberInfos:\n"+StringUtils.ToString(results, "\n")); }
			#endif
		}

		public static void GetInspectorViewables(this Type type, ref LinkedMemberHierarchy hierarchy, [CanBeNull]LinkedMemberInfo parentMemberInfo, [NotNull]ref List<LinkedMemberInfo> results, BindingFlags bindingFlags, bool includeHidden, FieldVisibility includeNonSerializedFields, PropertyVisibility propertyVisibility, MethodVisibility methodVisibility)
		{
			if(bindingFlags.HasFlag(BindingFlags.Static))
			{
				if(!bindingFlags.HasFlag(BindingFlags.Instance))
				{
					type.GetInspectorViewables(ref hierarchy, parentMemberInfo, ref results, bindingFlags, includeHidden, includeNonSerializedFields, propertyVisibility, methodVisibility, LinkedMemberParent.Static);
					return;
				}
			}
			else if(bindingFlags.HasFlag(BindingFlags.Instance))
			{
				var parentType = parentMemberInfo != null ? LinkedMemberParent.LinkedMemberInfo : type.IsUnityObject() ? LinkedMemberParent.UnityObject : LinkedMemberParent.Missing;
				type.GetInspectorViewables(ref hierarchy, parentMemberInfo, ref results, bindingFlags, includeHidden, includeNonSerializedFields, propertyVisibility, methodVisibility, parentType);
				return;
			}
			
			var parentTypeIfNotStatic = parentMemberInfo != null ? LinkedMemberParent.LinkedMemberInfo : type.IsUnityObject() ? LinkedMemberParent.UnityObject : LinkedMemberParent.Missing;

			bool noSerializedProperty;
			if(hierarchy.Target == null)
			{
				noSerializedProperty = true;
			}
			else if(parentMemberInfo != null)
			{
				noSerializedProperty = parentMemberInfo.ParentChainIsBroken;
			}
			else
			{
				// if parentMemberInfo is null and parent is not UnityEngine.Object, parent chain is broken
				noSerializedProperty = !type.IsUnityObject();
			}

			var order = InspectorUtility.Preferences.MemberDisplayOrder;
			for(int t = 0; t < 3; t++)
			{
				switch(order[t])
				{
					case Member.Field:
						var fields = type.GetFields(bindingFlags);
						for(int n = 0, count = fields.Length; n < count; n++)
						{
							var field = fields[n];
							if(field.IsInspectorViewable(includeHidden, includeNonSerializedFields))
							{
								bool isStatic = field.IsStatic;

								var linkedMemberInfo = hierarchy.Get(parentMemberInfo, field, isStatic ? LinkedMemberParent.Static : parentTypeIfNotStatic, isStatic || noSerializedProperty ? null : field.Name);

								if(linkedMemberInfo.GetAttribute<Attributes.InlineAttribute>(false, true) != null || field.FieldType.GetCustomAttributes(typeof(Attributes.InlineAttribute), false).Length > 0)
								{
									field.FieldType.GetInspectorViewables(ref hierarchy, linkedMemberInfo, ref results, bindingFlags, includeHidden, includeNonSerializedFields, propertyVisibility, methodVisibility, LinkedMemberParent.LinkedMemberInfo);
								}
								else
								{
									results.Add(linkedMemberInfo);
								}
							}
						}
						break;
					case Member.Property:
						var properties = type.GetProperties(bindingFlags);
						for(int n = 0, count = properties.Length; n < count; n++)
						{
							var property = properties[n];
							if(property.IsInspectorViewable(propertyVisibility, includeHidden))
							{
								bool isStatic = property.IsStatic();

								var linkedMemberInfo = hierarchy.Get(parentMemberInfo, property, isStatic ? LinkedMemberParent.Static : parentTypeIfNotStatic, isStatic || noSerializedProperty ? null : property.Name);

								if(linkedMemberInfo.GetAttribute<Attributes.InlineAttribute>(false, true) != null || property.PropertyType.GetCustomAttributes(typeof(Attributes.InlineAttribute), false).Length > 0)
								{
									property.PropertyType.GetInspectorViewables(ref hierarchy, linkedMemberInfo, ref results, bindingFlags, includeHidden, includeNonSerializedFields, propertyVisibility, methodVisibility, LinkedMemberParent.LinkedMemberInfo);
								}
								else
								{
									results.Add(linkedMemberInfo);
								}
							}
						}
						break;
					case Member.Method:
						var methods = type.GetMethods(bindingFlags);
						for(int n = 0, count = methods.Length; n < count; n++)
						{
							var method = methods[n];
							if(method.IsInspectorViewable(methodVisibility, includeHidden))
							{
								results.Add(hierarchy.Get(parentMemberInfo, method, method.IsStatic ? LinkedMemberParent.Static : parentTypeIfNotStatic));
							}
						}
						break;
				}
			}

			#if DEV_MODE && DEBUG_GET_INSPECTOR_VIEWABLES
			if(results.Count > 4) { Debug.Log(type.Name+".GetInspectorViewables: returning "+results.Count+" MemberInfos:\n"+StringUtils.ToString(results, "\n")); }
			#endif
		}

		public static void GetStaticInspectorViewables(this Type type, ref LinkedMemberHierarchy hierarchy, [CanBeNull]LinkedMemberInfo parentMemberInfo, [NotNull]ref List<LinkedMemberInfo> results, bool includeHidden, PropertyVisibility propertyVisibility, MethodVisibility methodVisibility)
		{
			var bindingFlags = ParentDrawerUtility.BindingFlagsDeclaredStatic;
			var order = InspectorUtility.Preferences.MemberDisplayOrder;
			for(int t = 0; t < 3; t++)
			{
				switch(order[t])
				{
					case Member.Field:
						var fields = type.GetFields(bindingFlags);
						for(int n = 0, count = fields.Length; n < count; n++)
						{
							var field = fields[n];
							if(field.IsInspectorViewableInStaticMode(includeHidden))
							{
								results.Add(hierarchy.Get(parentMemberInfo, field, LinkedMemberParent.Static));
							}
						}
						break;
					case Member.Property:
						var properties = type.GetProperties(bindingFlags);
						for(int n = 0, count = properties.Length; n < count; n++)
						{
							var property = properties[n];
							if(property.IsInspectorViewableInStaticMode(propertyVisibility, includeHidden))
							{
								results.Add(hierarchy.Get(parentMemberInfo, property, LinkedMemberParent.Static));
							}
						}
						break;
					case Member.Method:
						var methods = type.GetMethods(bindingFlags);
						for(int n = 0, count = methods.Length; n < count; n++)
						{
							var method = methods[n];
							if(method.IsInspectorViewableInStaticMode(methodVisibility, includeHidden))
							{
								results.Add(hierarchy.Get(parentMemberInfo, method, LinkedMemberParent.Static));
							}
						}
						break;
				}
			}

			#if DEV_MODE && DEBUG_GET_INSPECTOR_VIEWABLES
			if(results.Count > 4) { Debug.Log(type.Name+".GetInspectorViewables: returning "+results.Count+" MemberInfos:\n"+StringUtils.ToString(results, "\n")); }
			#endif
		}
	}
}