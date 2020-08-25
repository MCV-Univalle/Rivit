//#define DEBUG_IGNORED_COMMANDS
//#define DEBUG_EXECUTE_COMMAND
#define DEBUG_KEYBOARD_INPUT
//#define DEBUG_NEW_KEYBOARD_FOCUS

using UnityEngine;
using Component = UnityEngine.Component;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Sisus
{
	/// <summary>
	/// Contains several extension methods for ISplittableInspectorDrawer.
	/// These methods were done as extension methods, instead of being placed in an
	/// inheritable base class, so that different inspector drawers could extend different
	/// base classes, like EditorWindow or MonoBehaviour, based on their individual needs.
	/// </summary>
	public static class IInspectorDrawerExtensions
	{
		public static void ShowInSplitView(this ISplittableInspectorDrawer inspectorDrawer, Object target)
		{
			#if DEV_MODE
			Debug.Log("ShowInSplitView("+StringUtils.ToString(target)+")");
			#endif

			if(target != null)
			{
				var component = target as Component;
				if(component != null)
				{
					#if DEV_MODE
					Debug.Log("ShowInSplitView("+StringUtils.TypeToString(target)+") - was a Component");
					#endif

					var gameObject = component.gameObject;
					inspectorDrawer.ShowInSplitView(ArrayPool<Object>.CreateWithContent(gameObject));
					var splitBottom = inspectorDrawer.SplitView;

					// Wait one frame so that there's been time to cache all layout data during the next OnGUI call,
					// so that ScrollToShow can scroll to the correct position
					splitBottom.OnNextLayout(()=>
					{
						var show = splitBottom.State.drawers.FindDrawer(component);
						if(show != null)
						{
							splitBottom.Manager.Select(splitBottom, InspectorPart.Viewport, show, ReasonSelectionChanged.Peek);
							splitBottom.ScrollToShow(show);
							show.SetUnfolded(true, false, false);
						}
					});
				}
				else //GameObjects and Assets are okay to be shown as standalone
				{
					#if DEV_MODE
					Debug.Log("ShowInSplitView("+StringUtils.TypeToString(target)+") was not a Component");
					#endif

					inspectorDrawer.ShowInSplitView(ArrayPool<Object>.CreateWithContent(target));
				}
			}
		}

		public static IInspector SelectedOrDefaultView(this IInspectorDrawer inspectorDrawer)
		{
			var mainView = inspectorDrawer.MainView;
			var splittable = inspectorDrawer as ISplittableInspectorDrawer;
			if(splittable != null && splittable.ViewIsSplit)
			{
				var manager = splittable.Manager;
				var selected = manager.SelectedInspector;
				if(mainView == selected)
				{
					return mainView;
				}

				var splitView = splittable.SplitView;
				if(splitView == selected)
				{
					return splitView;
				}

				if(splitView == manager.ActiveInspector)
				{
					return splitView;
				}
			}
			return mainView;
		}

		public static void OnKeyDown(this IInspectorDrawer inspectorDrawer, Event e)
		{
			#if DEV_MODE && DEBUG_KEYBOARD_INPUT
			Debug.Log(StringUtils.ToColorizedString(inspectorDrawer.ToString(), ".OnKeyDown(", e.keyCode, ") with HasFocus=", inspectorDrawer.HasFocus, ", selectedControl=", StringUtils.ToString(inspectorDrawer.SelectedOrDefaultView().FocusedDrawer), ", SelectedPart=", inspectorDrawer.Manager.SelectedInspectorPart));
			#endif
			
			if(!inspectorDrawer.HasFocus)
			{
				return;
			}

			var view = inspectorDrawer.SelectedOrDefaultView();
			var keys = view.Preferences.keyConfigs;

			#if !POWER_INSPECTOR_LITE
			if(keys.stepBackInSelectionHistory.DetectAndUseInput(e))
			{
				view.StepBackInSelectionHistory();
				return;
			}
			if(keys.stepForwardInSelectionHistory.DetectAndUseInput(e))
			{
				view.StepForwardInSelectionHistory();
				return;
			}
			#endif
			
			inspectorDrawer.Repaint();

			DrawGUI.RegisterInputEvent(e);
			
			var activeView = inspectorDrawer.SelectedOrDefaultView();

			// give controls time to react to selection changes, editing text field changes etc.
			// before cached values are updated. (e.g. unapplied changes in delayed fields are
			// not discarded before they have time to get applied
			activeView.ResetNextUpdateCachedValues();
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!inspectorDrawer.HasFocus || activeView.Selected);
			#endif

			#if DEV_MODE && DEBUG_KEYBOARD_INPUT
			Debug.Log(StringUtils.ToColorizedString("OnKeyDown activeView.Selected=", activeView.Selected, ", activeView.FocusedDrawer=", activeView.FocusedDrawer+ ", Manager.SelectedInspector=", inspectorDrawer.Manager.SelectedInspector, ", inspectorDrawer.HasFocus=", inspectorDrawer.HasFocus));
			#endif

			if(activeView.Toolbar.OnKeyboardInputGivenWhenNotSelected(e, activeView.Preferences.keyConfigs))
			{
				if(e.type != EventType.Used)
				{
					DrawGUI.Use(e);
					ExitGUIUtility.ExitGUI();
				}
			}

			IDrawer selectedControl = null;
			if(activeView.Selected)
			{
				selectedControl = activeView.FocusedDrawer;
				if(selectedControl != null)
				{
					var onKeyboardInputBeingGiven = selectedControl.OnKeyboardInputBeingGiven;
					if(onKeyboardInputBeingGiven != null)
					{
						#if DEV_MODE && DEBUG_KEYBOARD_INPUT
						Debug.Log("onKeyboardInputBeingGiven(" + StringUtils.ToString(e) + "): " +StringUtils.ToString(onKeyboardInputBeingGiven));
						#endif
						if(onKeyboardInputBeingGiven(selectedControl, e, selectedControl.Inspector.Preferences.keyConfigs))
						{
							return;
						}
					}

					if(selectedControl.OnKeyboardInputGiven(e, selectedControl.Inspector.Preferences.keyConfigs))
					{
						return;
					}
				}
				else if(inspectorDrawer.Manager.SelectedInspectorPart == InspectorPart.Toolbar)
				{
					activeView.Toolbar.OnKeyboardInputGiven(e, activeView.Preferences.keyConfigs);
				}
				else if(inspectorDrawer.Manager.SelectedInspectorPart == InspectorPart.Viewport || inspectorDrawer.Manager.SelectedInspectorPart == InspectorPart.None)
				{
					bool fieldChangeInputGiven;
					if(keys.DetectNextField(e, true) || keys.DetectPreviousField(e, true)
					|| keys.nextFieldLeft.DetectAndUseInput(e) || keys.nextFieldRight.DetectAndUseInput(e)
					|| keys.nextFieldDown.DetectAndUseInput(e) || keys.nextFieldUp.DetectAndUseInput(e))
					{
						fieldChangeInputGiven = true;
					}
					else if(e.modifiers == EventModifiers.FunctionKey)
					{
						switch(e.keyCode)
						{
							case KeyCode.DownArrow:
							case KeyCode.UpArrow:
							case KeyCode.LeftArrow:
							case KeyCode.RightArrow:
								fieldChangeInputGiven = true;
								break;
							default:
								fieldChangeInputGiven = false;
								break;
						}
					}
					else
					{
						fieldChangeInputGiven = false;
					}

					if(fieldChangeInputGiven)
					{
						var drawers = activeView.State.drawers;
						if(drawers.Length == 0)
						{
							if(activeView.Toolbar != null)
							{
								activeView.Toolbar.OnFindCommandGiven();
							}
							else
							{
								KeyboardControlUtility.SetKeyboardControl(0, 3);
							}
						}
						else
						{
							var first = drawers[0];
							var select = first.GetNextSelectableDrawerRight(true, null);

							#if DEV_MODE && DEBUG_NEXT_FIELD
							Debug.Log(first + ".GetNextSelectableDrawerRight: "+StringUtils.ToString(select));
							#endif

							if(select != null)
							{
								activeView.Select(select, ReasonSelectionChanged.SelectNextControl);
							}
						}
					}
				}
			}

			if(DrawGUI.EditingTextField && keys.DetectTextFieldReservedInput(e, TextFieldType.TextRow))
			{
				#if DEV_MODE && DEBUG_KEYBOARD_INPUT
				Debug.Log(StringUtils.ToColorizedString("OnKeyboardInputGiven( ", StringUtils.ToString(e), ") DetectTextFieldReservedInput: ", true, " with selectedControl=", selectedControl));
				#endif
				return;
			}

			if(keys.addComponent.DetectInput(e))
			{
				#if DEV_MODE
				Debug.Log("AddComponent shortcut detected");
				#endif

				if(AddComponentButtonDrawer.OpenSelectedOrFirstFoundInstance(activeView))
				{
					DrawGUI.Use(e);
				}
			}

			if(keys.toggleSplitView.DetectInput(e))
			{
				var splittable = inspectorDrawer as ISplittableInspectorDrawer;
				if(splittable != null && splittable.CanSplitView)
				{
					DrawGUI.Use(e);
					bool setSplitView = !splittable.ViewIsSplit;
					splittable.MainView.OnNextLayout(() => splittable.SetSplitView(setSplitView));
				}
			}

			if(keys.refresh.DetectAndUseInput(e))
			{
				var selectedInspector = inspectorDrawer.Manager.SelectedInspector;
				if(selectedInspector != null && selectedInspector.InspectorDrawer == inspectorDrawer)
				{
					selectedInspector.ForceRebuildDrawers();
				}
				else
				{
					var mainView = inspectorDrawer.MainView;
					mainView.ForceRebuildDrawers();
					var splittable = inspectorDrawer as ISplittableInspectorDrawer;
					if(splittable != null && splittable.ViewIsSplit)
					{
						splittable.SplitView.ForceRebuildDrawers();
					}
				}
			}

			var keyCode = e.keyCode;
			switch(keyCode)
			{
				case KeyCode.Menu:
					if(selectedControl != null)
					{
						selectedControl.OpenContextMenu(e, selectedControl.RightClickArea, false, selectedControl.SelectedPart);
					}
					break;
				case KeyCode.Space:
					inspectorDrawer.Manager.RegisterKeyHeldDown(keyCode, "space");
					break;
				case KeyCode.F2:
					inspectorDrawer.Repaint();
					if(!DrawGUI.EditingTextField)
					{
						DrawGUI.Use(e);
						DrawGUI.EditingTextField = true;
					}
					break;
				case KeyCode.Escape:

					#if DEV_MODE
					Debug.Log("!!! ESCAPE !!!");
					#endif

					//when dragging a control, allow aborting using the escape key
					if(inspectorDrawer.Manager.MouseDownInfo.MouseDownOverDrawer != null)
					{
						inspectorDrawer.Manager.MouseDownInfo.Clear();
					}

					if(DrawGUI.EditingTextField)
					{
						DrawGUI.Use(e);
						DrawGUI.EditingTextField = false;
					}
					break;
				case KeyCode.AltGr:
				case KeyCode.RightAlt:
					KeyConfig.OnAltGrDown();
					break;
				#if DEV_MODE
				case KeyCode.I:
					if(e.control && e.alt)
					{
						Debug.Log("INFO: FocusedDrawer="+StringUtils.ToString(inspectorDrawer.SelectedOrDefaultView().FocusedDrawer)+ ", EditingTextField=" + DrawGUI.EditingTextField);
					}
				break;
				#endif
			}
		}

		public static void OnKeyUp(this IInspectorDrawer inspectorDrawer, Event e)
		{
			inspectorDrawer.Manager.OnKeyUp(e);
		}

		public static void OnValidateCommand(this IInspectorDrawer inspectorDrawer, Event e)
		{
			if(DrawGUI.ExecutingCustomMenuCommand || !inspectorDrawer.HasFocus)
			{
				#if DEV_MODE
				Debug.Log("Ignoring ValidateCommand with name: " + e.commandName);
				#endif
				return;
			}

			#if DEV_MODE
			Debug.Log("Detected ValidateCommand with name: " + e.commandName);
			#endif

			var selectedView = inspectorDrawer.Manager.SelectedInspector;
			if(selectedView != null && selectedView.InspectorDrawer == inspectorDrawer && inspectorDrawer.HasFocus)
			{
				selectedView.OnValidateCommand(e);
			}
		}

		public static void OnExecuteCommand(this IInspectorDrawer inspectorDrawer, Event e)
		{
			if(DrawGUI.ExecutingCustomMenuCommand || !inspectorDrawer.HasFocus)
			{
				#if DEV_MODE && DEBUG_IGNORED_COMMANDS
				Debug.LogWarning(StringUtils.ToColorizedString("Ignoring ExecuteCommand with name \"", e.commandName + "\"\nDrawer.HasFocus=", inspectorDrawer.HasFocus, ", ExecutingCustomMenuCommand=", DrawGUI.ExecutingCustomMenuCommand, ", FocusedDrawer=" + inspectorDrawer.Manager.FocusedControl));
				#endif
				return;
			}

			#if DEV_MODE && DEBUG_EXECUTE_COMMAND
			if(!string.Equals(e.commandName, "NewKeyboardFocus", System.StringComparison.Ordinal))
			{
				Debug.Log("Detected ExecuteCommand with name: " + e.commandName+ "\nFocusedControl=" + StringUtils.ToString(inspectorDrawer.Manager.FocusedControl)+", keyCode="+e.keyCode);
			}
			#endif
			
			#if DEV_MODE && DEBUG_NEW_KEYBOARD_FOCUS
			if(string.Equals(e.commandName, "NewKeyboardFocus", System.StringComparison.Ordinal))
			{
				var m = inspectorDrawer.Manager;
				Debug.Log(StringUtils.ToColorizedString("NewKeyboardFocus part=", m.SelectedInspectorPart, ", control=", m.FocusedControl, ", KeyboardControl=", KeyboardControlUtility.KeyboardControl));
			}
			#endif

			var selectedView = inspectorDrawer.SelectedOrDefaultView();
			if(selectedView != null && inspectorDrawer.HasFocus)
			{
				selectedView.OnExecuteCommand(e);
			}
		}
	}
}