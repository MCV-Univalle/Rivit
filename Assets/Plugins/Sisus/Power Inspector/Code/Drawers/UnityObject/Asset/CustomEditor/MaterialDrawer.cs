#if UNITY_EDITOR
using System;
using JetBrains.Annotations;
using Sisus.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sisus
{
	[Serializable, DrawerForAsset(typeof(Material), false, true)]
	public class MaterialDrawer : CustomEditorAssetDrawer
	{
		#if UNITY_2019_2_OR_NEWER // MaterialEditor.customShaderGUI was added in Unity 2019.2
		MaterialProperty[] materialProperties;
		#endif

		/// <inheritdoc cref="IUnityObjectDrawer.Unfolded" />
		public override bool Unfolded
		{
			get
			{
				if(Foldable)
				{
					return unfoldedness;
				}

				#if DEV_MODE && PI_ASSERTATIONS
				if(parent == null) { Debug.LogWarning(ToString()+".Unfolded called with parent null. Was this called during Setup before parent was assigned?"); }
				#endif

				return true;
			}
		}
		
		/// <inheritdoc />
		public override float Unfoldedness
		{
			get
			{
				return Foldable ? unfoldedness : base.Unfoldedness;
			}
		}

		/// <inheritdoc cref="IUnityObjectDrawer.Expandable" />
		public override bool Foldable
		{
			get
			{
				#if DEV_MODE && PI_ASSERTATIONS
				if(parent == null) { Debug.LogWarning(ToString()+".Unfolded called with parent null. Was this called during Setup before parent was assigned?"); }
				#endif

				return parent != null && parent.Members.Length > 1; //parent is IGameObjectDrawer;
			}
		}

		/// <inheritdoc />
		protected override Editor HeaderEditor
		{
			get
			{
				//this is needed to make the forceVisible unfolding to work
				return Editor;
			}
		}

		/// <inheritdoc />
		public override PrefixResizer PrefixResizer
		{
			get
			{
				//prefix width in MaterialInspector is static so the prefix resizer should not be shown
				return DebugMode ? PrefixResizer.TopOnly : PrefixResizer.Disabled;
			}
		}

		#if UNITY_2018_1_OR_NEWER // Presets were added in Unity 2018.1
		/// <inheritdoc />
		protected override bool HasPresetIcon
		{
			get
			{
				return Editable;
			}
		}
		#endif

		/// <summary> Creates a new instance of the drawer or returns a reusable instance from the pool. </summary>
		/// <param name="targets"> The targets that the drawer represent. </param>
		/// <param name="parent"> The parent drawer of the created drawer. Can be null. </param>
		/// <param name="inspector"> The inspector in which the IDrawer are contained. Can not be null. </param>
		/// <returns> The instance, ready to be used. </returns>
		[NotNull]
		public static new MaterialDrawer Create(Object[] targets, [CanBeNull]IParentDrawer parent, [NotNull]IInspector inspector)
		{
			MaterialDrawer result;
			if(!DrawerPool.TryGet(out result))
			{
				result = new MaterialDrawer();
			}
			result.Setup(targets, targets, typeof(MaterialEditor), parent, inspector);
			result.LateSetup();
			return result;
		}

		/// <inheritdoc />
		protected override void Setup(Object[] setTargets, Object[] setEditorTargets, Type setEditorType, IParentDrawer setParent, IInspector setInspector)
		{
			base.Setup(setTargets, setEditorTargets, setEditorType, setParent, setInspector);

			// we have to set IsVisible
			var materialEditor = Editor as MaterialEditor;
			if(materialEditor != null)
			{
				#if DEV_MODE
				Debug.Log("materialEditor not null with setEditorType="+ StringUtils.ToString(setEditorType)+ ", Editor="+StringUtils.TypeToString(Editor) + ", HeaderEditor=" + StringUtils.TypeToString(HeaderEditor));
				#endif

				#if UNITY_2019_2_OR_NEWER

				// Was unable to figure out how the isVisible property value can be set using reflection, and as a result
				// calling OnInspectorGUI for the MaterialEditor draws nothing.
				// However, was able to get around the issue by getting the ShaderGUI from the MaterialEditor via MaterialEditor.customShaderGUI
				// (which was added in Unity 2019.2) and drawing the GUI using that instead.
				// The material properties are needed for the OnGUI method of the ShaderGUI.
				materialProperties = MaterialEditor.GetMaterialProperties(targets);
				

				#elif UNITY_2018_3_OR_NEWER

				var isVisible = typeof(MaterialEditor).GetField("m_IsVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if(isVisible != null)
				{
					isVisible.SetValue(Editor, true);
				}
				#if DEV_MODE
				else { Debug.LogWarning("MaterialEditor field \"m_IsVisible\" not found"); }
				#endif

				#else
				
				var forceVisible = typeof(MaterialEditor).GetProperty("forceVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if(forceVisible != null)
				{
					forceVisible.SetValue(Editor, true, null);
				}
				#if DEV_MODE
				else { Debug.LogWarning("MaterialEditor field \"forceVisible\" not found"); }
				#endif

				#endif
			}
			#if DEV_MODE && PI_ASSERTATIONS
			else
			{
				#if DEV_MODE
				Debug.Log("materialEditor null with setEditorType="+ StringUtils.ToString(setEditorType)+ ", Editor="+StringUtils.TypeToString(Editor) + ", HeaderEditor=" + StringUtils.TypeToString(HeaderEditor));
				#endif

				Debug.Assert(setEditorType != typeof(MaterialEditor));
			}
			#endif
		}

		#if UNITY_2019_2_OR_NEWER // MaterialEditor.customShaderGUI was added in Unity 2019.2
		/// <inheritdoc />
		protected override void DrawOnInspectorGUI([NotNull]Editor bodyEditor)
		{
			var materialEditor = (MaterialEditor)bodyEditor;

			EditorGUI.BeginChangeCheck();
			{
				// Was unable to figure out how to set the isVisible property value of the MaterialEditor in Unity 2019.3 onwards,
				// and thus calling OnInspectorGUI for the Editor has no effect in those versions.
				// However, it is possible to get the ShaderGUI from the MaterialEditor, and draw the GUI manually using that instead.
				//materialEditor.customShaderGUI.OnGUI(materialEditor, materialProperties);
				materialEditor.PropertiesGUI();
			}
			if(EditorGUI.EndChangeCheck())
			{
				materialEditor.PropertiesChanged();
				// changing the shader can cause the MaterialEditor to change, so refetch it after each change made.
				UpdateEditor();
			}
		}
		#endif

		/// <inheritdoc />
		protected override void GetHeaderSubtitle(ref GUIContent subtitle)
		{
			// no subtitle text to prevent clipping with the shader dropdown element
		}
	}
}
#endif