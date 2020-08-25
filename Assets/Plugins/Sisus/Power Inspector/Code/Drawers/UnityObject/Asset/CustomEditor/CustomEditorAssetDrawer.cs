#define SAFE_MODE

//#define DEBUG_GET_EDITOR
//#define DEBUG_SETUP_EDITOR
//#define DEBUG_DETECT_MOUSEOVER
//#define DEBUG_ASSET_IMPORTER

#if UNITY_EDITOR
using System;
using System.IO;
using JetBrains.Annotations;
using Sisus.Attributes;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

#if UNITY_2017_2_OR_NEWER
using System.Reflection;
#endif

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Presets;
#endif

namespace Sisus
{
	/// <summary>
	/// Like ScriptableObjectDrawer, but might use an Editor for drawing at least some aspects of it.
	/// </summary>
	[Serializable, DrawerByExtension(".bytes", true)]
	public class CustomEditorAssetDrawer : CustomEditorBaseDrawer<CustomEditorAssetDrawer, Object>, ICustomEditorAssetDrawer, IOnProjectOrHierarchyChanged
	{
		/// <summary>
		/// Targets to use for the main Editor ("editor" field) used for drawing the body.
		/// Can be different from "targets" field values for targets that have asset importers,
		/// in which case editorTargets will refer to those importers.
		/// </summary>
		private Object[] editorTargets;

		/// <summary>
		/// The default Editor for the target assets. Even when editorTargets are AssetImporters,
		/// this will still be the Editor for the assets themselves and not the importers.
		/// 
		/// Usually used for drawing the header of the Editor, and when fetching Previewables
		/// for the Preview area.
		/// 
		/// When assets are not asset importers, this refers to the same target as the editor field.
		/// </summary>
		private Editor assetEditor;

		#if UNITY_2017_2_OR_NEWER
		/// <summary>
		/// True if editorTargets are is asset importers, and Editor is an AssetImporterEditor.
		/// </summary>
		private bool isAssetImporter;
		#endif

		private GUIContent headerSubtitle = new GUIContent();
		
		private string localPath;
		private bool isPackageAsset;

		private float headerHeight = EditorGUIDrawer.AssetTitlebarHeightWithOneButtonRow;
		private bool headerHeightDetermined;

		private GUIContent[] assetLabels;
		private GUIContent[] assetLabelsOnlyOnSomeTargets =  new GUIContent[0];

		/// <inheritdoc/>
		public virtual bool WantsSearchBoxDisabled
		{
			get
			{
				return false;
			}
		}

		/// <inheritdoc cref="IUnityObjectDrawer.Unfoldedness" />
		public override float Unfoldedness
		{
			get
			{
				return 1f;
			}
		}

		/// <inheritdoc cref="IUnityObjectDrawer.Unfolded" />
		public override bool Unfolded
		{
			get
			{
				return true;
			}
		}

		/// <inheritdoc cref="IUnityObjectDrawer.Expandable" />
		public override bool Foldable
		{
			get
			{
				return false;
			}
		}

		/// <inheritdoc />
		public override PrefixResizer PrefixResizer
		{
			get
			{
				// hide the prefix resizer if Editor body has no content.
				// Won't compare directly to EditorHeight, because the PrefixResizer control itself can add 6 units to height.
				return Height < HeaderHeight + DrawGUI.SingleLineHeight ? PrefixResizer.Disabled : PrefixResizer.TopOnly;
			}
		}

		/// <inheritdoc />
		public GUIContent[] AssetLabels
		{
			get
			{
				return assetLabels;
			}
		}

		/// <inheritdoc />
		public GUIContent[] AssetLabelsOnlyOnSomeTargets
		{
			get
			{
				return assetLabelsOnlyOnSomeTargets;
			}
		}

		/// <inheritdoc cref="IUnityObjectDrawer.RequiresConstantRepaint" />
		public override bool RequiresConstantRepaint
		{
			get
			{
				return editor == null ? false : editor.RequiresConstantRepaint();
			}
		}

		/// <inheritdoc cref="IUnityObjectDrawer.HeaderHeight" />
		public override float HeaderHeight
		{
			get
			{
				return headerHeight;
			}
		}

		/// <inheritdoc/>
		protected override float ToolbarIconsTopOffset
		{
			get
			{
				return AssetToolbarIconsTopOffset;
			}
		}

		/// <inheritdoc/>
		protected sealed override float HeaderToolbarIconWidth
		{
			get
			{
				return AssetHeaderToolbarIconWidth;
			}
		}

		/// <inheritdoc/>
		protected sealed override float HeaderToolbarIconHeight
		{
			get
			{
				return AssetHeaderToolbarIconHeight;
			}
		}

		/// <inheritdoc />
		protected sealed override float HeaderToolbarIconsRightOffset
		{
			get
			{
				return AssetHeaderToolbarIconsRightOffset;
			}
		}

		/// <inheritdoc />
		protected sealed override float HeaderToolbarIconsOffset
		{
			get
			{
				return AssetHeaderToolbarIconsOffset;
			}
		}

		#if UNITY_2018_1_OR_NEWER //  // Presets were added in Unity 2018.1
		/// <inheritdoc/>
		protected override bool HasPresetIcon
		{
			get
			{
				return isAssetImporter && Editable;
			}
		}
		#endif

		/// <inheritdoc />
		protected override MonoScript MonoScript
		{
			get
			{
				var target = Target as ScriptableObject;
				return target == null ? null : MonoScript.FromScriptableObject(target);
			}
		}

		/// <inheritdoc />
		protected override bool IsAsset
		{
			get
			{
				return true;
			}
		}

		/// <summary> Gets a value indicating whether target asset is contained inside a Unity Package. </summary>
		/// <value> True if this object is inside a Unity Package, false if not. </value>
		protected bool IsPackageAsset
		{
			get
			{
				return isPackageAsset;
			}
		}

		/// <summary>
		/// Gets the local path of the target asset.
		/// This is either relative to the Assets
		/// directory or relative to the Unity Package
		/// directory.
		/// </summary>
		/// <value> The local path of the target asset. </value>
		protected string LocalPath
		{
			get
			{
				return localPath;
			}
		}

		/// <summary>
		/// Gets the full file path of the target asset.
		/// </summary>
		/// <value> The full file path of the target asset. </value>
		protected string FullPath
		{
			get
			{
				return localPath.Length == 0 ? "" : FileUtility.LocalToFullPath(localPath);
			}
		}

		/// <inheritdoc />
		protected override Color PrefixBackgroundColor
		{
			get
			{
				return InspectorUtility.Preferences.theme.AssetHeaderBackground;
			}
		}
		
		/// <summary> Gets subtitle text to display below the main ehader text. </summary>
		/// <returns> The header subtitle. </returns>
		protected GUIContent HeaderSubtitle
		{
			get
			{
				return headerSubtitle;
			}
		}

		/// <summary>
		/// Separation between header Editor and normal editor was needed to make the preview icon and label get displayed correctly.
		/// Also, if targets are of mismatching types, an Editor can't be created for them, but since the Editor for the header will only
		/// use the first of the targets, it will still work.
		/// </summary>
		/// <value>
		/// The Editor used for drawing the header
		/// </value>
		[NotNull]
		protected virtual Editor HeaderEditor
		{
			get
			{
				if(assetEditor == null)
				{
					FetchAssetEditor();
					
					#if DEV_MODE
					if(assetEditor != null)
					{
						Debug.LogError("The null check was needed"); //this has been needed during DrawHeaderBase
					}
					else
					{
						Debug.LogError("headerEditor null after GetEditor with allSameType=" + StringUtils.ToColorizedString(allSameType)+", targets="+StringUtils.ToString(targets)+", editorTargets="+ StringUtils.ToString(editorTargets));
					}
					#endif

					#if DEV_MODE
					Debug.Assert(assetEditor != null, ToString()+".HeaderEditor returning null.");
					#endif
				}
				else if(Editors.DisposeIfInvalid(ref assetEditor))
				{
					FetchAssetEditor();

					#if DEV_MODE
					Debug.Assert(assetEditor != null, ToString()+".HeaderEditor returning null.");
					#endif
				}
				
				return assetEditor;
			}
		}
		
		/// <inheritdoc />
		protected override bool HasExecuteMethodIcon
		{
			get
			{
				return false;
			}
		}
		
		#if !POWER_INSPECTOR_LITE
		/// <inheritdoc/>
		protected override string PasteContextMenuText
		{
			get
			{
				#if UNITY_2017_2_OR_NEWER
				return isAssetImporter ? "Paste Settings" : "Paste Values";
				#else
				return "Paste Values";
				#endif
			}
		}
		#endif

		/// <inheritdoc/>
		protected sealed override Object[] EditorTargets
		{
			get
			{
				return editorTargets;
			}
		}

		/// <summary> Creates a new instance of the drawer or returns a reusable instance from the pool. </summary>
		/// <param name="targets"> The targets that the drawers represent. </param>
		/// <param name="parent"> The parent drawers of the created drawers. Can be null. </param>
		/// <param name="inspector"> The inspector in which the IDrawer are contained. Can not be null. </param>
		/// <returns> The drawer instance, ready to be used. </returns>
		public static CustomEditorAssetDrawer Create(Object[] targets, [CanBeNull]IParentDrawer parent, [NotNull]IInspector inspector)
		{
			return Create(targets, null, null, parent, inspector);
		}

		/// <summary> Creates a new instance of the drawer or returns a reusable instance from the pool. </summary>
		/// <param name="targets"> The targets that the drawers represent. </param>
		/// <param name="editorTargets"> Targets for use when creating the custom editor. E.g. AssetImporters for targets. </param>
		/// <param name="editorType"> The type to use for the custom editor. </param>
		/// <param name="parent"> The parent drawers of the created drawers. Can be null. </param>
		/// <param name="inspector"> The inspector in which the IDrawer are contained. Can not be null. </param>
		/// <returns> The drawer instance, ready to be used. </returns>
		public static CustomEditorAssetDrawer Create(Object[] targets, [CanBeNull]Object[] editorTargets, [CanBeNull]Type editorType, [CanBeNull]IParentDrawer parent, [NotNull]IInspector inspector)
		{
			CustomEditorAssetDrawer result;
			if(!DrawerPool.TryGet(out result))
			{
				result = new CustomEditorAssetDrawer();
			}
			result.Setup(targets, editorTargets, editorType, parent, inspector);
			result.LateSetup();
			return result;
		}

		/// <inheritdoc/>
		protected override void UpdateEditor()
		{
			if(canHaveEditor)
			{
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(editorTargets.Length > 0, ToString());
				Debug.Assert(editorTargets[0] != null, ToString());
				#endif

				#if DEV_MODE && DEBUG_GET_EDITOR
				Debug.Log(Msg(ToString(), ".GetEditor(ref ", StringUtils.TypeToString(editor), ", editorTargets=", StringUtils.TypesToString(editorTargets), ", editorType=", editorType, ", allSameType=", allSameType));
				#endif
				
				Editors.GetEditor(ref editor, editorTargets, editorType, allSameType);
				
				#if DEV_MODE && DEBUG_GET_EDITOR
				Debug.Log("editor.GetType(): "+editor.GetType().FullName +", editor.targets: "+StringUtils.ToString(editor.targets) + ", editor.targets.Types: "+StringUtils.TypesToString(editor.targets));
				#endif

				#if UNITY_2017_2_OR_NEWER
				if(assetEditor != null)
				{
					UpdateAssetImporterData();
				}
				#endif
			}
		}

		#if UNITY_2017_2_OR_NEWER
		private void UpdateAssetImporterData()
		{
			#if DEV_MODE
			if(editor == null)
			{
				Debug.LogWarning("UpdateAssetImporterData called with Editor still " + StringUtils.Null);
			}
			#endif

			var importerEditor = editor as UnityEditor.Experimental.AssetImporters.AssetImporterEditor;
			if(importerEditor == null)
			{
				isAssetImporter = false;
				return;
			}
			
			isAssetImporter = true;

			if(assetEditor == null)
			{
				#if DEV_MODE
				Debug.LogWarning("Asset Importer Editor " + importerEditor.GetType().Name + " Asset Editor was still "+StringUtils.Null);
				#endif
				return;
			}

			if(assetEditor == importerEditor)
			{
				#if DEV_MODE
				Debug.LogError("Asset Importer Editor " + importerEditor.GetType().Name + " == Asset Editor "+assetEditor.GetType().Name);
				#endif
				return;
			}
			
			var setTargetEditorMethod = typeof(UnityEditor.Experimental.AssetImporters.AssetImporterEditor).GetMethod("InternalSetAssetImporterTargetEditor", BindingFlags.Instance | BindingFlags.NonPublic);
			if(setTargetEditorMethod == null)
			{
				#if DEV_MODE
				Debug.LogError("Could not find method InternalSetAssetImporterTargetEditor in AssetImporterEditor!");
				#endif
				return;
			}

			#if DEV_MODE && DEBUG_ASSET_IMPORTER
			Debug.Log("Setting Asset Importer Editor "+importerEditor.GetType().Name+" target Editor to "+assetEditor.GetType().Name);
			#endif

			setTargetEditorMethod.InvokeWithParameter(editor, assetEditor);
		}
		#endif

		/// <inheritdoc/>
		public virtual void SetupInterface(Object[] setTargets, Object[] setEditorTargets, Type setEditorType, IParentDrawer setParent, IInspector setInspector)
		{
			Setup(setTargets, setEditorTargets, setEditorType, setParent, setInspector);
		}

		/// <inheritdoc/>
		protected sealed override void Setup(Object[] setTargets, IParentDrawer setParent, IInspector setInspector, Type setEditorType)
		{
			throw new NotSupportedException("Please use the other Setup method of CustomEditorAssetDrawer.");
		}

		/// <summary> Sets up an instance of the drawers for usage. </summary>
		/// <param name="setTargets"> The targets that the drawers represent. Can not be null. </param>
		/// <param name="setEditorTargets"> The targets for which the main Editor is created. E.g. the asset importers for targets. Can be null. </param>
		/// <param name="setParent"> The parent drawers of the created drawers. Can be null. </param>
		/// <param name="setInspector"> The inspector in which the IDrawer are contained. Can not be null. </param>
		/// <param name="setEditorType"> The type of the custom editor. Can be null. </param>
		protected virtual void Setup([NotNull]Object[] setTargets, [CanBeNull]Object[] setEditorTargets, [CanBeNull]Type setEditorType, [CanBeNull]IParentDrawer setParent, [NotNull]IInspector setInspector)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(setTargets.Length > 0);
			#endif

			headerHeight = DrawGUI.Active.AssetTitlebarHeight(false);
			headerHeightDetermined = false;

			allSameType = setTargets.AllSameType();

			if(setEditorTargets == null)
			{
				if(!allSameType)
				{
					setEditorTargets = setTargets;
				}
				else
				{
					if(AssetImporters.TryGet(setTargets, ref setEditorTargets))
					{
						var assetImporterType = setEditorTargets[0].GetType();
						CustomEditorUtility.TryGetCustomEditorType(assetImporterType, out setEditorType);
					}
					else
					{
						setEditorTargets = setTargets;
					}
				}
			}
			else if(setEditorTargets[0] == null)
			{
				ArrayPool<Object>.Dispose(ref setEditorTargets);
				setEditorTargets = setTargets;
			}
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(setEditorTargets != null || !allSameType);
			Debug.Assert(!allSameType || setEditorTargets.Length > 0);
			#endif
			
			editorTargets = setEditorTargets;
			
			Sisus.AssetLabels.Get(setTargets, ref assetLabels, ref assetLabelsOnlyOnSomeTargets);
			Sisus.AssetLabels.OnAssetLabelsChanged += OnAssetLabelsChanged;

			var target = setTargets[0];
			localPath = setTargets.Length == 1 ? AssetDatabase.GetAssetPath(target) : "";
			isPackageAsset = FileUtility.IsPackageAsset(localPath);
			
			// base handles calling UpdateEditor
			base.Setup(setTargets, setParent, setInspector, setEditorType);

			FetchAssetEditor();

			#if DEV_MODE && PI_ASSERTATIONS && UNITY_2017_2_OR_NEWER
			if(!isAssetImporter && editor != HeaderEditor)
			{
				#if DEV_MODE && DEBUG_SETUP_EDITOR
				Debug.LogError(Msg(ToString(), " non-asset importer editor did not match HeaderEditor! - targets=", StringUtils.TypesToString(setTargets), ", editorTargets=", StringUtils.TypesToString(setEditorTargets), ", editorType=", setEditorType, ", editor=", StringUtils.TypeToString(editor), ", HeaderEditor=", HeaderEditor));
				#endif
			}
			#endif

			#if DEV_MODE && DEBUG_SETUP_EDITOR
			Debug.Log(Msg(ToString(), " Setup with targets=", StringUtils.TypesToString(setTargets), ", editorTargets=", StringUtils.TypesToString(setEditorTargets), ", editorType=", setEditorType, ", editor=", StringUtils.TypeToString(editor)));
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			if(editor == null)
			{
				Debug.LogWarning(Msg(ToString(), " editor was null after Setup with canHaveEditor=", canHaveEditor, " and targets: ", StringUtils.TypesToString(targets)));
			}
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			if(assetEditor == null)
			{
				Debug.LogWarning(Msg(ToString(), " headerEditor was null after Setup with canHaveEditor=", canHaveEditor, " and targets: ", StringUtils.TypesToString(targets)));
			}
			#endif
		}

		private void OnAssetLabelsChanged(Object[] labelsChangedforTargets)
		{
			if(labelsChangedforTargets.ContentsMatch(targets))
			{
				Sisus.AssetLabels.Get(targets, ref assetLabels, ref assetLabelsOnlyOnSomeTargets);
			}
		}
		
		/// <inheritdoc />
		protected override Type TypeToCheckForExcludeFromPresetAttribute()
		{
			return editorTargets[0].GetType();
		}

		/// <inheritdoc cref="IDrawer.LateSetup" />
		public override void LateSetup()
		{
			base.LateSetup();
			GetHeaderSubtitle(ref headerSubtitle);
		}

		/// <inheritdoc cref="IDrawer.OnMouseoverDuringDrag" />
		public override void OnMouseoverDuringDrag(MouseDownInfo mouseDownInfo, Object[] dragAndDropObjectReferences)
		{
			// Don't reject drag n drop if cursor is over editor body, because it could accept drag n drops (e.g. if it contains Object reference fields).
			if(HeaderMouseovered)
			{
				#if DEV_MODE
				Debug.LogWarning("OnMouseoverDuringDrag - Rejecting!");
				#endif
				DrawGUI.Active.DragAndDropVisualMode = DragAndDropVisualMode.Rejected;
			}
		}

		/// <inheritdoc />
		protected override void DrawHeaderBase(Rect position)
		{
			HandlePrefixHighlightingForFilter(position, 55f, 4f);

			var headerEditor = HeaderEditor;

			#if SAFE_MODE // I think that Editor was once null when creating a build
			if(headerEditor == null)
			{
				return;
			}
			#endif

			var drawnRect = EditorGUIDrawer.AssetHeader(position, headerEditor);

			if(!headerHeightDetermined && Event.current.type == EventType.Repaint)
			{
				if(headerHeight != drawnRect.height)
				{
					if(drawnRect.height >= DrawGUI.SingleLineHeight)
					{
						headerHeight = drawnRect.height;
					}
					else
					{
						headerHeight = DrawGUI.Active.AssetTitlebarHeight(false);
					}
				}
				headerHeightDetermined = true;
			}
		}
		
		/// <inheritdoc />
		protected override void NameByType()
		{
			var target = targets[0];
			if(target != null)
			{
				#if UNITY_EDITOR
				Undo.RecordObjects(targets, "Auto-Name");
				#endif

				var typeName = target.GetType().Name;
				for(int n = targets.Length - 1; n >= 0; n--)
				{
					target = targets[n];
					target.name = typeName;
					#if UNITY_EDITOR
					EditorUtility.SetDirty(target);
					#endif
				}
			}
		}

		/// <inheritdoc/>
		protected override void FindReferencesInScene()
		{
			DrawGUI.ExecuteMenuItem("Assets/Find References In Scene");
		}
		
		/// <inheritdoc />
		protected override Object[] FindObjectsOfType()
		{
			return Resources.FindObjectsOfTypeAll(Type);
		}

		/// <inheritdoc cref="IDrawer.SelectPreviousOfType" />
		public override void SelectPreviousOfType()
		{
			AssetDrawerUtility.SelectPreviousOfType(this);
		}

		/// <inheritdoc cref="IDrawer.SelectNextOfType" />
		public override void SelectNextOfType()
		{
			AssetDrawerUtility.SelectNextOfType(this);
		}
		
		/// <inheritdoc />
		public override bool DrawPrefix(Rect position)
		{
			// UPDATE:
			// Don't grey out header of uneditable assets, just the body.
			// This matches better how the default inspector does it.
			var guiColorWas = GUI.color;
			if(DrawGreyedOut)
			{
				var color = GUI.color;
				color.a = 1f;
				GUI.color = color;
			}

			bool dirty = base.DrawPrefix(position);
			
			DrawSubtitle(position);

			GUI.color = guiColorWas;

			return dirty;
		}

		/// <inheritdoc />
		public override bool DrawBody(Rect position)
		{
			if(isPackageAsset)
			{
				#if DEV_MODE
				Debug.Assert(GUI.enabled);
				#endif
				GUI.enabled = false;
			}

			bool dirty = base.DrawBody(position);

			if(isPackageAsset)
			{
				GUI.enabled = true;
			}

			return dirty;
		}

		/// <summary> Draw subtitle text below the main header text. </summary>
		/// <param name="headerRect"> The draw rect of the whole header. </param>
		private void DrawSubtitle(Rect headerRect)
		{
			var subtitleRect = headerRect;
			subtitleRect.height = DrawGUI.SingleLineHeight;
			subtitleRect.y += 22f;
			float iconOffset = 43f;
			subtitleRect.x += iconOffset;
			subtitleRect.width -= iconOffset;
			float removeFromRight = HeaderButtonsWidth + HeaderButtonsPadding;
			if(removeFromRight < InternalOpenButtonWidth)
			{
				removeFromRight = InternalOpenButtonWidth;
			}
			subtitleRect.width -= removeFromRight;

			if(subtitleRect.width > 0f)
			{
				GUI.Label(subtitleRect, HeaderSubtitle, InspectorPreferences.Styles.SubHeader);
			}

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(HeaderHeight >= subtitleRect.yMax - headerRect.y);
			Debug.Assert(headerRect.xMax >= subtitleRect.xMax);
			Debug.Assert(!HeadlessMode);
			#endif
		}

		/// <inheritdoc cref="IDrawer.Dispose" />
		public override void Dispose()
		{
			if(!ReferenceEquals(assetEditor, null))
			{
				Editors.Dispose(ref assetEditor);
			}

			editorTargets = null;

			// Make sure that asset Editor is updated properly
			if(ReferenceEquals(editor, assetEditor))
			{
				assetEditor = null;
			}

			GUIContentArrayPool.Dispose(ref assetLabels);
			Sisus.AssetLabels.OnAssetLabelsChanged -= OnAssetLabelsChanged;

			base.Dispose();
		}

		/// <summary> Opens target asset in appropriate editor. </summary>
		protected virtual void Open()
		{
			AssetDatabase.OpenAsset(Target);
		}

		/// <summary> Opens directory containing target in file explorer. </summary>
		protected virtual void ShowInExplorer()
		{
			EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(Target));
		}

		/// <inheritdoc />
		protected override bool CanPasteFromClipboard()
		{
			#if UNITY_2018_1_OR_NEWER
			return Clipboard.CanPasteAs(Types.Preset);
			#else
			return false;
			#endif
		}

		/// <inheritdoc />
		public override void CopyToClipboard(int index)
		{
			var target = editorTargets[index];
			#if UNITY_2018_1_OR_NEWER
			var preset = new Preset(target);
			Clipboard.TryCopy(preset, Types.Preset);
			#endif
			Clipboard.ObjectReference = target;

			SendCopyToClipboardMessage();
		}

		/// <inheritdoc />
		protected override void DoPasteFromClipboard()
		{
			for(int n = editorTargets.Length - 1; n >= 0; n--)
			{
				var target = editorTargets[n];
				#if UNITY_2018_1_OR_NEWER
				var preset = new Preset(target);
				Clipboard.TryPasteUnityObject(preset);
				preset.ApplyTo(target);
				#endif
				EditorUtility.SetDirty(target);
				var importer = (target as AssetImporter);
				if(importer != null)
				{
					importer.SaveAndReimport();
				}
			}
			editor.serializedObject.Update();
		}

		/// <inheritdoc cref="IDrawer.Duplicate" />
		public override void Duplicate()
		{
			#if UNITY_EDITOR
			AssetDrawerUtility.Duplicate(targets);
			#endif
		}

		/// <summary> Sets subtitle text and tooltip for display below the main header text. </summary>
		/// <param name="subtitle"> [in,out] The subtitle GUContent to set. This cannot be null. </param>
		protected virtual void GetHeaderSubtitle([NotNull]ref GUIContent subtitle)
		{
			subtitle.text = FileUtility.GetHumanReadableNameForAsset(Type, LocalPath);
		}
		
		#if DEV_MODE && DEBUG_DETECT_MOUSEOVER
		bool tempDebugMouseoverLastValue;
		public override bool DetectMouseover()
		{
			var result = base.DetectMouseover();
			if(result != tempDebugMouseoverLastValue)
			{
				tempDebugMouseoverLastValue = result;
				Debug.Log(ToString()+" DetectMouseover: "+StringUtils.ToColorizedString(result) + "\nClickToSelectArea.MouseIsOver: "+ ClickToSelectArea.MouseIsOver()+"\nIsOutsideViewport: "+InspectorUtility.ActiveInspector.IsOutsideViewport(Bounds)+ "\nBounds: "+ Bounds+"\nInspectorViewRect: " + DrawGUI.InspectorViewRect+ "\nIgnoreMouseInputs: " + InspectorUtility.ActiveInspector.IgnoreViewportMouseInputs());
			}
			return result;
		}
		#endif

		/// <inheritdoc />
		protected override void BuildContextMenu(ref Menu menu, bool extendedMenu)
		{
			if(BuildContextMenuItemsStartingFromBaseClass)
			{
				base.BuildContextMenu(ref menu, extendedMenu);
			}
			
			#if UNITY_EDITOR
			if(!IsDisabledAsset())
			{
				menu.AddSeparatorIfNotRedundant();
				menu.Add("Disable", "Disables the target assets by appending a supernumerary extension (like \".disabled\" or \".tmp\") to their filenames.", Disable);
			}
			#endif

			if(!BuildContextMenuItemsStartingFromBaseClass)
			{
				base.BuildContextMenu(ref menu, extendedMenu);
			}
		}

		protected bool IsDisabledAsset()
		{
			string filename = Path.GetFileName(LocalPath);
			if(string.IsNullOrEmpty(filename))
			{
				return false;
			}

			int dotInFilename = filename.LastIndexOf('.');
			if(dotInFilename == -1)
			{
				return false;
			}

			int secondDotInFilename = filename.LastIndexOf('.', dotInFilename - 1);
			return secondDotInFilename != -1;
		}

		#if UNITY_EDITOR
		/// <summary> Disables the target assets by appending a supernumerary extension (like ".disabled" or ".tmp") to their filenames. </summary>
		private void Disable()
		{
			var cacheInspector = inspector;

			FileUtility.Disable(targets, inspector.Preferences.disabledScriptExtension);

			cacheInspector.RebuildDrawers(ArrayPool<Object>.ZeroSizeArray, true);
			cacheInspector.OnNextLayout(()=>cacheInspector.RebuildDrawers(false));
		}
		#endif

		private void FetchAssetEditor()
		{
			// New test: use the same Editor for header and body unless targets differ from editor targets.
			if(ReferenceEquals(editorTargets, targets))
			{
				assetEditor = Editor;
			}
			else
			{
				Editors.GetEditor(ref assetEditor, targets, null, allSameType);
			}
			
			#if UNITY_2017_2_OR_NEWER
			if(editor != null)
			{
				UpdateAssetImporterData();
			}
			#endif
		}

		/// <inheritdoc/>
		public void OnProjectOrHierarchyChanged(OnChangedEventSubject changed, ref bool hasNullReferences)
		{
			if(changed != OnChangedEventSubject.Project && changed != OnChangedEventSubject.Undefined)
			{
				return;
			}

			for(int n = targets.Length - 1; n >= 0; n--)
			{
				var target = targets[n];
				if(target == null)
				{
					if(UnityObjectExtensions.TryToFixNull(ref target))
					{
						#if DEV_MODE
						Debug.LogWarning(ToString()+".OnHierarchyChanged fixed targets["+n+"] (\""+target.name+"\") being null.");
						#endif
						continue;
					}
					
					#if DEV_MODE
					Debug.Log(ToString()+".OnHierarchyChanged targets["+n+"] was null and could not be fixed.");
					#endif

					hasNullReferences = true;
				}
			}
			
			#if UNITY_EDITOR
			if(editor != null && Editors.DisposeIfInvalid(ref editor))
			{
				hasNullReferences = true; // set to true so that drawers get rebuilt, in case e.g. asset paths have changed.
			}

			if(assetEditor != null && Editors.DisposeIfInvalid(ref assetEditor))
			{
				hasNullReferences = true; // set to true so that drawers get rebuilt, in case e.g. asset paths have changed.
			}
			#endif

			FetchAssetEditor();
			UpdateEditor();
		}

		#if DEV_MODE
		/// <inheritdoc/>
		protected override object[] GetDevInfo()
		{
			#if UNITY_2017_2_OR_NEWER
			return base.GetDevInfo().Add(", HeaderEditor=", HeaderEditor, ", isAssetImporter=", isAssetImporter, ", targets=", StringUtils.TypesToString(targets), ", editorTargets=", StringUtils.TypesToString(editorTargets));
			#else
			return base.GetDevInfo().Add(", HeaderEditor=", HeaderEditor, ", targets=", StringUtils.TypesToString(targets), ", editorTargets=", StringUtils.TypesToString(editorTargets));
			#endif
		}
		#endif
	}
}
#endif