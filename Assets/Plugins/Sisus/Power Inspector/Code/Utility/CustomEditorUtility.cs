//#define DEBUG_CUSTOM_EDITORS
//#define DEBUG_PROPERTY_DRAWERS
//#define DEBUG_SET_EDITING_TEXT_FIELD

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Sisus.Compatibility;

#if DEV_MODE && DEBUG_CUSTOM_EDITORS
using System.Linq;
#endif

namespace Sisus
{
	public static class CustomEditorUtility
	{
		private static Dictionary<Type, Type> customEditorsByType;
		private static Dictionary<Type, Type> propertyDrawersByType;
		private static Dictionary<Type, Type> decoratorDrawersByType;

		public static Dictionary<Type, Type> CustomEditorsByType
		{
			get
			{
				if(customEditorsByType == null)
				{
					customEditorsByType = new Dictionary<Type, Type>();

					var editorType = typeof(CustomEditor);
					var inspectedTypeField = editorType.GetField("m_InspectedType", BindingFlags.NonPublic | BindingFlags.Instance);
					var useForChildrenField = editorType.GetField("m_EditorForChildClasses", BindingFlags.NonPublic | BindingFlags.Instance);

					//NOTE: important to also get invisible types, so that internal Editors such as RectTransformEditor are also returned
					var editorTypes = Types.Editor.GetExtendingEditorTypes(true);
					
					var ignored = PluginAttributeConverterProvider.ignoredEditors;
					for(int n = ignored.Length - 1; n >= 0; n--)
					{
						int index = Array.IndexOf(editorTypes, ignored[n]);
						if(index != -1)
						{
							editorTypes = editorTypes.RemoveAt(index);
						}
					}

					#if DEV_MODE
					Debug.Assert(Array.IndexOf(editorTypes, Types.GetInternalEditorType("UnityEditor.RectTransformEditor")) != -1, "RectTransformEditor was not among "+ editorTypes.Length+" found editors!");
					#endif

					GetDrawersByInspectedTypeFromAttributes<CustomEditor>(editorTypes, inspectedTypeField, ref customEditorsByType);
					
					//second pass: also apply for inheriting types if they don't already have more specific overrides
					GetDrawersByInheritedInspectedTypesFromAttributes<CustomEditor>(editorTypes, inspectedTypeField, useForChildrenField, ref customEditorsByType);
					
					#if DEV_MODE && DEBUG_CUSTOM_EDITORS
					var log = customEditorsByType.Where(pair => !Types.Component.IsAssignableFrom(pair.Key));
					Debug.Log("Non-Components with custom editors:\n"+StringUtils.ToString(log, "\n"));
					#endif
				}

				return customEditorsByType;
			}
		}

		public static Dictionary<Type, Type> PropertyDrawersByType
		{
			get
			{
				if(propertyDrawersByType == null)
				{
					propertyDrawersByType = new Dictionary<Type, Type>();

					var propertyDrawerType = typeof(CustomPropertyDrawer);
					var typeField = propertyDrawerType.GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
					var useForChildrenField = propertyDrawerType.GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);
					var propertyDrawerTypes = typeof(UnityEditor.PropertyDrawer).GetExtendingNonUnityObjectClassTypes(true);

					GetDrawersByInspectedTypeFromAttributes<CustomPropertyDrawer>(propertyDrawerTypes, typeField, ref propertyDrawersByType);
					//second pass: also apply for inheriting types if they don't already have more specific overrides
					GetDrawersByInheritedInspectedTypesFromAttributes<CustomPropertyDrawer>(propertyDrawerTypes, typeField, useForChildrenField, ref propertyDrawersByType);

					#if DEV_MODE && PI_ASSERTATIONS
					Debug.Assert(Array.IndexOf(propertyDrawerTypes, typeof(UnityEditorInternal.UnityEventDrawer)) != -1);
					#endif

					#if DEV_MODE && DEBUG_PROPERTY_DRAWERS
					Debug.Log("propertyDrawersByType:\r\n"+StringUtils.ToString(propertyDrawersByType, "\r\n"));
					#endif
				}
				
				return propertyDrawersByType;
			}
		}

		public static Dictionary<Type, Type> DecoratorDrawersByType
		{
			get
			{
				if(decoratorDrawersByType == null)
				{
					decoratorDrawersByType = new Dictionary<Type, Type>();

					var propertyDrawerType = typeof(CustomPropertyDrawer);
					var typeField = propertyDrawerType.GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
					// UPDATE: Apparently this field is not really respected for decorator drawers
					//var useForChildrenField = propertyDrawerType.GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);

					var decoratorDrawerTypes = typeof(DecoratorDrawer).GetExtendingNonUnityObjectClassTypes(true);

					GetDrawersByInspectedTypeFromAttributes<CustomPropertyDrawer>(decoratorDrawerTypes, typeField, ref decoratorDrawersByType);
					GetDrawersByInheritedInspectedTypesFromAttributes<CustomPropertyDrawer>(decoratorDrawerTypes, typeField, null, ref decoratorDrawersByType);
				}
				
				return decoratorDrawersByType;
			}
		}
		
		/// <summary>
		/// Attempts to get PropertyDrawer Type for given class or from attributes on the field
		/// </summary>
		/// <param name="classMemberType"> Type of the class for which we are trying to find the PropertyDrawer. </param>
		/// <param name="memberInfo"> LinkedMemberInfo of the property for which we are trying to find the PropertyDrawer. </param>
		/// <param name="propertyAttribute"> [out] PropertyAttribute found on the property. </param>
		/// <param name="drawerType"> [out] Type of the PropertyDrawer for the PropertyAttribute. </param>
		/// <returns>
		/// True if target has a PropertyDrawer, false if not.
		/// </returns>
		public static bool TryGetPropertyDrawerType([NotNull]Type classMemberType, [NotNull]LinkedMemberInfo memberInfo, out PropertyAttribute propertyAttribute, out Type drawerType)
		{
			var attributes = memberInfo.GetAttributes(Types.PropertyAttribute);
			for(int n = attributes.Length - 1; n >= 0; n--)
			{
				var attribute = attributes[n];
				if(TryGetPropertyDrawerType(attribute.GetType(), out drawerType))
				{
					propertyAttribute = attribute as PropertyAttribute;
					return true;
				}
			}
			propertyAttribute = null;
			return TryGetPropertyDrawerType(classMemberType, out drawerType);
		}

		/// <summary>
		/// Attempts to get PropertyDrawer Type for given class.
		/// </summary>
		/// <param name="classMemberOrAttributeType">
		/// Type of the class for which we are trying to find the PropertyDrawer. </param>
		/// <param name="drawerType">
		/// [out] Type of the PropertyDrawer. </param>
		/// <returns>
		/// True if target has a PropertyDrawer, false if not.
		/// </returns>
		public static bool TryGetPropertyDrawerType([NotNull]Type classMemberOrAttributeType, out Type drawerType)
		{
			return PropertyDrawersByType.TryGetValue(classMemberOrAttributeType, out drawerType);
		}

		public static bool TryGetDecoratorDrawerTypes([NotNull]LinkedMemberInfo memberInfo, out object[] decoratorAttributes, out Type[] drawerTypes)
		{
			//TO DO: Add support for PropertyAttribute.order
			
			drawerTypes = null;
			decoratorAttributes = null;
			
			var attributes = memberInfo.GetAttributes(Types.PropertyAttribute);
			for(int n = attributes.Length - 1; n >= 0; n--)
			{
				var attribute = attributes[n];
				Type drawerType;
				if(TryGetDecoratorDrawerType(attribute.GetType(), out drawerType))
				{
					if(drawerTypes == null)
					{
						decoratorAttributes = new[] { attribute };
						drawerTypes = new[]{drawerType};
					}
					else
					{
						decoratorAttributes = decoratorAttributes.Add(attribute);
						drawerTypes = drawerTypes.Add(drawerType);
					}
				}
			}
			
			return drawerTypes != null;
		}

		public static bool TryGetDecoratorDrawerTypes([NotNull]MemberInfo memberInfo, out object[] decoratorAttributes, out Type[] drawerTypes)
		{
			//TO DO: Add support for PropertyAttribute.order
			// 
			drawerTypes = null;
			decoratorAttributes = null;
			
			var attributes = memberInfo.GetCustomAttributes(Types.PropertyAttribute, true);
			for(int n = attributes.Length - 1; n >= 0; n--)
			{
				var attribute = attributes[n];
				Type drawerType;
				if(TryGetDecoratorDrawerType(attribute.GetType(), out drawerType))
				{
					if(drawerTypes == null)
					{
						decoratorAttributes = new[] { attribute };
						drawerTypes = new[]{drawerType};
					}
					else
					{
						decoratorAttributes = decoratorAttributes.Add(attribute);
						drawerTypes = drawerTypes.Add(drawerType);
					}
				}
			}
			
			return drawerTypes != null;
		}
		
		public static bool AttributeHasDecoratorDrawer(Type propertyAttributeType)
		{
			return DecoratorDrawersByType.ContainsKey(propertyAttributeType);
		}

		public static bool TryGetDecoratorDrawerType(Type propertyAttributeType, out Type drawerType)
		{
			return DecoratorDrawersByType.TryGetValue(propertyAttributeType, out drawerType);
		}
		
		public static bool TryGetCustomEditorType(Type targetType, out Type editorType)
		{
			return CustomEditorsByType.TryGetValue(targetType, out editorType);
		}

		/// <summary>
		/// Given an array of PropertyDrawer, DecoratorDrawers or Editors, gets their inspected types and adds them to drawersByInspectedType.
		/// </summary>
		/// <typeparam name="TAttribute"> Type of the attribute. </typeparam>
		/// <param name="drawerOrEditorTypes"> List of PropertyDrawer, DecoratorDrawer or Editor types. </param>
		/// <param name="targetTypeField"> FieldInfo for getting the inspected type. </param>
		/// <param name="drawersByInspectedType">
		/// [in,out] dictionary where drawer types will be added with their inspected types as the keys. </param>
		private static void GetDrawersByInspectedTypeFromAttributes<TAttribute>([NotNull]Type[] drawerOrEditorTypes, [NotNull]FieldInfo targetTypeField, [NotNull]ref Dictionary<Type,Type> drawersByInspectedType) where TAttribute : Attribute
		{
			var attType = typeof(TAttribute);
			
			for(int n = drawerOrEditorTypes.Length - 1; n >= 0; n--)
			{
				var drawerType = drawerOrEditorTypes[n];
				if(!drawerType.IsAbstract)
				{
					var attributes = drawerType.GetCustomAttributes(attType, true);
					for(int a = attributes.Length - 1; a >= 0; a--)
					{
						var attribute = attributes[a];
						var inspectedType = targetTypeField.GetValue(attribute) as Type;
						if(!inspectedType.IsAbstract)
						{
							drawersByInspectedType[inspectedType] = drawerType;
						}
					}
				}
			}
		}

		private static void GetDrawersByInheritedInspectedTypesFromAttributes<TAttribute>(Type[] drawerOrEditorTypes, FieldInfo targetTypeField, [CanBeNull]FieldInfo useForChildrenField, ref Dictionary<Type,Type> addEditorsByType) where TAttribute : Attribute
		{
			var attType = typeof(TAttribute);

			for(int n = drawerOrEditorTypes.Length - 1; n >= 0; n--)
			{
				var drawerType = drawerOrEditorTypes[n];

				if(!drawerType.IsAbstract)
				{
					var attributes = drawerType.GetCustomAttributes(attType, true);
					for(int a = attributes.Length - 1; a >= 0; a--)
					{
						var attribute = attributes[a];
						bool useForChildren = useForChildrenField == null ? true : (bool)useForChildrenField.GetValue(attribute);
						if(useForChildren)
						{
							var targetType = targetTypeField.GetValue(attribute) as Type;
							if(!targetType.IsClass)
							{
								//value types don't support inheritance
								continue;
							}

							var extendingTypes = targetType.GetExtendingTypes(true);

							for(int t = extendingTypes.Length - 1; t >= 0; t--)
							{
								var extendingType = extendingTypes[t];
								if(!extendingType.IsAbstract)
								{
									if(!addEditorsByType.ContainsKey(extendingType))
									{
										addEditorsByType.Add(extendingType, drawerType);
									}
								}
							}
						}
						#if DEV_MODE
						else if(typeof(DecoratorDrawer).IsAssignableFrom(drawerType)) { Debug.LogWarning(drawerType.Name+ ".useForChildren was "+StringUtils.False); }
						#endif
					}
				}
			}
		}

		public static void BeginEditor(out bool editingTextFieldWas, out EventType eventType, out KeyCode keyCode)
		{
			BeginEditorOrPropertyDrawer(out editingTextFieldWas, out eventType, out keyCode);
		}

		public static void EndEditor(bool editingTextFieldWas, EventType eventType, KeyCode keyCode)
		{
			EndEditorOrPropertyDrawer(editingTextFieldWas, eventType, keyCode);
		}

		public static void BeginPropertyDrawer(out bool editingTextFieldWas, out EventType eventType, out KeyCode keyCode)
		{
			BeginEditorOrPropertyDrawer(out editingTextFieldWas, out eventType, out keyCode);
		}
		

		public static void EndPropertyDrawer(bool editingTextFieldWas, EventType eventType, KeyCode keyCode)
		{
			EndEditorOrPropertyDrawer(editingTextFieldWas, eventType, keyCode);
		}

		private static void BeginEditorOrPropertyDrawer(out bool editingTextFieldWas, out EventType eventType, out KeyCode keyCode)
		{
			editingTextFieldWas = EditorGUIUtility.editingTextField;
			eventType = DrawGUI.LastInputEventType;
			var lastInputEvent = DrawGUI.LastInputEvent();
			keyCode = lastInputEvent == null ? KeyCode.None : lastInputEvent.keyCode;
		}

		private static void EndEditorOrPropertyDrawer(bool editingTextFieldWas, EventType eventType, KeyCode keyCode)
		{
			if(EditorGUIUtility.editingTextField != editingTextFieldWas)
			{
				if(eventType != EventType.KeyDown && eventType != EventType.KeyUp)
				{
					#if DEV_MODE && DEBUG_SET_EDITING_TEXT_FIELD
					Debug.Log("DrawGUI.EditingTextField = "+StringUtils.ToColorizedString(EditorGUIUtility.editingTextField)+" with eventType="+StringUtils.ToString(eventType)+", keyCode="+keyCode+", lastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()));
					#endif
					DrawGUI.EditingTextField = EditorGUIUtility.editingTextField;
				}
				else
				{
					switch(keyCode)
					{
						case KeyCode.UpArrow:
						case KeyCode.DownArrow:
						case KeyCode.LeftArrow:
						case KeyCode.RightArrow:
							if(!EditorGUIUtility.editingTextField)
							{
								#if DEV_MODE
								Debug.Log("DrawGUI.EditingTextField = "+StringUtils.ToColorizedString(false)+" with eventType="+StringUtils.ToString(eventType)+", keyCode="+keyCode+", lastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()));
								#endif
								DrawGUI.EditingTextField = false;
							}
							else // prevent Unity automatically starting field editing when field focus is changed to a text field, as that is not how Power Inspector functions
							{
								#if DEV_MODE
								Debug.LogWarning("EditorGUIUtility.editingTextField = "+StringUtils.ToColorizedString(false)+" with eventType="+StringUtils.ToString(eventType)+", keyCode="+keyCode+", lastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()));
								#endif
								EditorGUIUtility.editingTextField = false;
							}
							return;
						default:
							#if DEV_MODE
							Debug.Log("DrawGUI.EditingTextField = "+StringUtils.ToColorizedString(false)+" with eventType="+StringUtils.ToString(eventType)+", keyCode="+keyCode+", lastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()));
							#endif
							DrawGUI.EditingTextField = EditorGUIUtility.editingTextField;
							return;
					}
				}
			}
		}
	}
}
#endif