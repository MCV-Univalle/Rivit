using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sisus
{
	/// <summary>
	/// Class for invoking coroutines without a MonoBehaviour instance.
	/// Also supports invoking coroutines in edit mode.
	/// </summary>
	[ExecuteInEditMode, AddComponentMenu("")]
	public class StaticCoroutine : MonoBehaviour
	{
		private static StaticCoroutine instance;
		private static MonoBehaviour monoBehaviour;
	 
		#if UNITY_EDITOR
		private static List<CoroutineInfo> editModeCoroutines = new List<CoroutineInfo>();
		#endif

		private static List<CoroutineInfo> runtimeCoroutines = new List<CoroutineInfo>();

		public struct CoroutineInfo
		{
			public readonly string name;
			public readonly object owner;
			public readonly IEnumerator coroutine;

			public CoroutineInfo(IEnumerator methodCoroutine, string methodName)
			{
				coroutine = methodCoroutine;
				name = methodName;
				owner = null;
			}

			public CoroutineInfo(IEnumerator methodCoroutine, string methodName, object methodOwner)
			{
				coroutine = methodCoroutine;
				name = methodName;
				owner = methodOwner;
			}

			public CoroutineInfo(IEnumerator methodCoroutine)
			{
				coroutine = methodCoroutine;
				name = "";
				owner = null;
			}

			public bool Equals(string methodName)
			{
				return string.Equals(name, methodName);
			}

			public bool Equals(string methodName, object methodOwner)
			{
				return ReferenceEquals(owner, methodOwner) && string.Equals(name, methodName);
			}

			public bool Equals(IEnumerator methodCoroutine)
			{
				return coroutine == methodCoroutine;
			}
		}

		private static StaticCoroutine Instance
		{
			get
			{
				if(instance == null)
				{
					instance = FindObjectOfType<StaticCoroutine>();

					if(instance == null)
					{
						var go = new GameObject("StaticCoroutine");
						go.hideFlags = HideFlags.HideAndDontSave; //hide in hierarchy
						if(Application.isPlaying)
						{
							DontDestroyOnLoad(go); //have the object persist from one scene to the next
						}
						instance = go.AddComponent<StaticCoroutine>();
					}
					
					monoBehaviour = instance;
				}
				
				return instance;
			}
		}
		
		private static MonoBehaviour MonoBehavior
		{
			get
			{
				if(monoBehaviour == null)
				{
					instance = FindObjectOfType<StaticCoroutine>();

					if(instance == null)
					{
						var go = new GameObject("StaticCoroutine");
						go.hideFlags = HideFlags.HideAndDontSave; //hide in hierarchy
						if(Application.isPlaying)
						{
							DontDestroyOnLoad(go);//have the object persist from one scene to the next
						}
						instance = go.AddComponent<StaticCoroutine>();
					}
					
					monoBehaviour = instance;
				}

				return monoBehaviour;
			}
		}

		/// <summary>
		/// Generates an invisible temporary MonoBehaviour in the background and starts coroutine using it.
		/// 
		/// This method can only be used in play mode. For starting coroutines in edit mode use
		/// StartCoroutine(IEnumerator, bool) or StartCoroutineInEditMode.
		/// </summary>
		/// <param name="coroutine"> Coroutine to start. </param>
		[NotNull]
		public static new Coroutine StartCoroutine([NotNull]IEnumerator coroutine)
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				throw new NotSupportedException("StartCoroutine(IEnumerator) cannot be called in edit mode. Please use use StartCoroutine(IEnumerator, bool) or StartCoroutineInEditMode.");
			}
			#endif

			return MonoBehavior.StartCoroutine(Instance.DoCoroutine(coroutine));
		}
		
		/// <summary>
		/// In play mode generates an invisible temporary MonoBehaviour in the background and starts coroutine using it.
		/// 
		/// In edit mode handles manually invoking the coroutine.
		/// Note that many of the built-in yield instructions in Unity won't work in edit mode,
		/// however you can use yield return null just fine.
		/// </summary>
		/// <param name="coroutine"> Coroutine to start. </param>
		/// <param name="allowInEditMode"> Determines what happens if called in edit mode: if true starts the coroutine in edit mode, if false throws an exception. </param>
		public static IEnumerator StartCoroutine([NotNull]IEnumerator coroutine, bool allowInEditMode)
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				if(allowInEditMode)
				{
					yield return StartCoroutineInEditMode(coroutine);
				}
				else
				{
					throw new NotSupportedException("StartCoroutine was called in edit mode with allowInEditMode false.");
				}
			}
			else
			#endif
			{
				yield return MonoBehavior.StartCoroutine(Instance.DoCoroutine(coroutine));
			}
		}

		/// <summary>
		/// Generates an invisible temporary MonoBehaviour in the background and starts coroutine using it after the specified delay in seconds.
		/// 
		/// This method can only be used in play mode. For starting delayed coroutines in edit mode use
		/// StartCoroutine(float, IEnumerator, bool) or StartCoroutineInEditMode.
		/// </summary>
		/// <param name="delay"> Delay in seconds before coroutine is started. </param>
		/// <param name="coroutine"> Coroutine to start. </param>
		[NotNull]
		public static Coroutine StartCoroutine(float delay, [NotNull]IEnumerator coroutine)
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				throw new NotSupportedException("StartCoroutine(float,IEnumerator) cannot be called in edit mode. Please use use StartCoroutine(float, IEnumerator, bool) or StartCoroutineInEditMode.");
			}
			#endif

			return MonoBehavior.StartCoroutine(Instance.DoCoroutineDelayed(delay, coroutine));
		}

		/// <summary>
		/// Generates an invisible temporary MonoBehaviour in the background and starts coroutine using it after the specified delay in seconds.
		/// 
		/// Coroutines can also be started in edit mode, however the return value will be null.
		/// Note that many of the built-in yield instructions in Unity won't work in edit mode,
		/// however you can use yield return null just fine.
		/// </summary>
		/// <param name="delay"> Delay in seconds before coroutine is started. </param>
		/// <param name="coroutine"> Coroutine to start. </param>
		[CanBeNull]
		public static IEnumerator StartCoroutine(float delay, [NotNull]IEnumerator coroutine, bool allowInEditMode)
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				if(allowInEditMode)
				{
					yield return new WaitForSecondsRealtime(delay);
					yield return StartCoroutineInEditMode(coroutine);
				}
				else
				{
					throw new NotSupportedException("StartCoroutine was called in edit mode with allowInEditMode false.");
				}
			}
			else
			#endif
			{
				yield return MonoBehavior.StartCoroutine(Instance.DoCoroutineDelayed(delay, coroutine));
			}
		}
		
		/// <summary>
		/// Stops static coroutine that was previously started using StartCoroutine.
		/// 
		/// Supports edit mode.
		/// </summary>
		/// <param name="delay"> Delay in seconds before coroutine is started. </param>
		/// <param name="coroutine"> Coroutine to start. </param>
		public static new void StopCoroutine(IEnumerator coroutine)
		{
			MonoBehavior.StopCoroutine(coroutine); //this will launch the coroutine on our instance
		}

		/// <summary>
		/// Determines whether or not coroutine started using StartCoroutine is currently invoking.
		/// 
		/// If methodName is null or empty and this is called in play mode returns false.
		/// 
		/// If coroutine is null and this is called in edit mode returns false.
		/// </summary>
		/// <param name="coroutine"> The coroutine to check. This is used in edit mode only. </param>
		/// <param name="methodName"> The name of the coroutine method. This is used in play mode only. </param>
		/// <returns> True if method is still invoking, false if not. </returns>
		public static bool IsInvoking([CanBeNull]IEnumerator coroutine, [CanBeNull]string methodName)
		{
			if(coroutine != null)
			{
				return IsInvoking(coroutine);
			}

			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				for(int n = editModeCoroutines.Count - 1; n >= 0; n--)
				{
					if(editModeCoroutines[n].Equals(methodName))
					{
						return true;
					}
				}
				return false;
			}
			#endif

			if(string.IsNullOrEmpty(methodName))
			{
				return false;
			}
			return monoBehaviour != null && monoBehaviour.IsInvoking(methodName);
		}

		/// <summary>
		/// Determines whether or not coroutine started using StartCoroutine is currently invoking.
		/// 
		/// If methodName is null or empty and this is called in play mode returns false.
		/// 
		/// If coroutine is null and this is called in edit mode returns false.
		/// </summary>
		/// <param name="coroutine"> The coroutine to check. This is used in edit mode only. </param>
		/// <param name="methodName"> The name of the coroutine method. This is used in play mode only. </param>
		/// <returns> True if method is still invoking, false if not. </returns>
		public static bool IsInvoking([NotNull]IEnumerator coroutine)
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				if(coroutine == null)
				{
					return false;
				}

				for(int n = editModeCoroutines.Count - 1; n >= 0; n--)
				{
					if(editModeCoroutines[n].Equals(coroutine))
					{
						return true;
					}
				}
				//return editModeCoroutines.Contains(coroutine);
				return false;
			}
			#endif

			for(int n = runtimeCoroutines.Count - 1; n >= 0; n--)
			{
				if(runtimeCoroutines[n].Equals(coroutine))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether or not coroutine started using StartCoroutine is currently invoking.
		/// 
		/// If methodName is null or empty and this is called in play mode returns false.
		/// 
		/// If coroutine is null and this is called in edit mode returns false.
		/// </summary>
		/// <param name="coroutine"> The coroutine to check. This is used in edit mode only. </param>
		/// <param name="methodName"> The name of the coroutine method. This is used in play mode only. </param>
		/// <returns> True if method is still invoking, false if not. </returns>
		public static new bool IsInvoking(string methodName)
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				for(int n = editModeCoroutines.Count - 1; n >= 0; n--)
				{
					if(editModeCoroutines[n].Equals(methodName))
					{
						return true;
					}
				}
				return false;
			}
			#endif

			for(int n = runtimeCoroutines.Count - 1; n >= 0; n--)
			{
				if(runtimeCoroutines[n].Equals(methodName))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether or not any coroutines started using StartCoroutine are currently invoking.
		/// </summary>
		/// <returns> True if one or more coroutines are still invoking, false if none are. </returns>
		public static new bool IsInvoking()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				return editModeCoroutines.Count > 0;
			}
			#endif

			return monoBehaviour.IsInvoking();
		}
	 
		[UsedImplicitly]
		private void Awake()
		{
			if(instance == null)
			{
				instance = this;
			}

			#if UNITY_EDITOR
			HandleMoveNextForExistingCoroutinesInEditMode();
			#endif
		}
		
		#if UNITY_EDITOR
		private static void HandleMoveNextForExistingCoroutinesInEditMode()
		{
			if(Application.isPlaying || editModeCoroutines.Count == 0)
			{
				return;
			}
			UnityEditor.EditorApplication.update -= CallMoveNextForEditModeCoroutines;
			UnityEditor.EditorApplication.update += CallMoveNextForEditModeCoroutines;
		}
		#endif

		#if UNITY_EDITOR
		/// <summary>
		/// Starts coroutine If edit mode after specified delay in seconds.
		/// </summary>
		/// <param name="delay"> Delay in seconds before coroutine is started. </param>
		/// <param name="coroutine"> The coroutine to handle if edit mode is active. </param>
		/// <returns> True if handles starting coroutine, false if not. </returns>
		public static IEnumerator StartCoroutineInEditMode(float delay, IEnumerator coroutine)
		{
			if(Application.isPlaying)
			{
				throw new NotSupportedException("StartCoroutineInEditMode cannot be called in play mode. Please use use StartCoroutine(float, IEnumerator) instead.");
			}

			yield return new WaitForSecondsRealtime(delay);
			yield return StartCoroutineInEditMode(coroutine);
		}
		#endif

		#if UNITY_EDITOR
		/// <summary>
		/// Starts coroutine If edit mode.
		/// If not in edit mode throws a NotSupportedException exception.
		/// </summary>
		/// <param name="coroutine"> The coroutine to start. </param>
		/// <returns> Yield instructions that determine when coroutine has finished. </returns>
		public static WaitForStaticCoroutine StartCoroutineInEditMode(IEnumerator coroutine)
		{
			return StartCoroutineInEditMode(new CoroutineInfo(coroutine));
		}
		#endif

		#if UNITY_EDITOR
		/// <summary>
		/// Starts coroutine If edit mode.
		/// If not in edit mode throws a NotSupportedException exception.
		/// </summary>
		/// <param name="coroutine"> The coroutine to start. </param>
		/// <returns> Yield instructions that determine when coroutine has finished. </returns>
		public static WaitForStaticCoroutine StartCoroutineInEditMode(IEnumerator coroutine, string methodName, object owner = null)
		{
			return StartCoroutineInEditMode(new CoroutineInfo(coroutine, methodName, owner));
		}
		#endif

		#if UNITY_EDITOR
		/// <summary>
		/// Starts coroutine If edit mode.
		/// If not in edit mode throws a NotSupportedException exception.
		/// </summary>
		/// <param name="coroutine"> The coroutine to start. </param>
		/// <returns> Yield instructions that determine when coroutine has finished. </returns>
		public static WaitForStaticCoroutine StartCoroutineInEditMode(CoroutineInfo coroutine)
		{
			if(Application.isPlaying)
			{
				throw new NotSupportedException("StartCoroutineInEditMode cannot be called in play mode. Please use use StartCoroutine(IEnumerator) instead.");
			}
			
			editModeCoroutines.Add(coroutine);
			HandleMoveNextForExistingCoroutinesInEditMode();
			
			return new WaitForStaticCoroutine(coroutine.coroutine, null);
		}
		#endif

		#if UNITY_EDITOR
		private static void CallMoveNextForEditModeCoroutines()
		{
			int count = editModeCoroutines.Count;
			if(count == 0)
			{
				UnityEditor.EditorApplication.update -= CallMoveNextForEditModeCoroutines;
				return;
			}

			for(int n = count - 1; n >= 0; n--)
			{
				var editorCoroutine = editModeCoroutines[n];
				bool coroutineFinished = !editorCoroutine.coroutine.MoveNext();

				if(coroutineFinished)
				{
					editModeCoroutines.RemoveAt(n);
				}
			}
		}
		#endif

		private IEnumerator DoCoroutine([NotNull]IEnumerator coroutine)
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				yield return StartCoroutineInEditMode(coroutine);
			}
			else
			#endif
			{
				yield return MonoBehavior.StartCoroutine(coroutine);
			}
		}
		
		public static void LaunchDelayed(float delay, [NotNull]Action action, bool allowInEditMode)
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				if(allowInEditMode)
				{
					LaunchDelayedInEditMode(delay, action);
				}
				else
				{
					throw new NotSupportedException("LaunchDelayed was called in edit mode with allowInEditMode false.");
				}
			}
			else
			#endif
			{
				LaunchDelayed(delay, action);
			}
		}

		/// <summary> Executes given action after the specified delayed in seconds. </summary>
		/// <param name="delay"> The delay in seconds before action is executed. </param>
		/// <param name="action"> The action to execute. </param>
		public static void LaunchDelayed(float delay, [NotNull]Action action)
		{
			MonoBehavior.StartCoroutine(Instance.DoLaunchDelayed(delay, action)); //this will launch the coroutine on our instance
		}

		public static void LaunchDelayedTimeScaleIndependent(float delay, [NotNull]Action action)
		{
			MonoBehavior.StartCoroutine(Instance.DoLaunchDelayedTimeScaleIndependent(delay, action)); //this will launch the coroutine on our instance
		}
		
		private IEnumerator DoCoroutineDelayed(float delay, [NotNull]IEnumerator coroutine)
		{
			yield return new WaitForSeconds(delay);
			yield return MonoBehavior.StartCoroutine(coroutine);
		}

		private IEnumerator DoLaunchDelayed(float delay, [NotNull]Action action)
		{
			yield return new WaitForSeconds(delay);
			action();
		}

		private IEnumerator DoLaunchDelayedTimeScaleIndependent(float delay, [NotNull]Action action)
		{
			yield return new WaitForSecondsRealtime(delay);
			action();
		}

		[UsedImplicitly]
		private void OnApplicationQuit()
		{
			instance = null;
		}

		#if UNITY_EDITOR
		public static void LaunchDelayedInEditMode(int framesToDelay, [NotNull]Action action)
		{
			if(framesToDelay >= 1)
			{
				UnityEditor.EditorApplication.delayCall += ()=>LaunchDelayedInEditMode(framesToDelay - 1, action);
			}
			else
			{
				action();
			}
		}

		public static void LaunchDelayedInEditMode(float secondsToDelay, [NotNull]Action action)
		{
			DoLaunchDelayedInEditMode(UnityEditor.EditorApplication.timeSinceStartup + secondsToDelay, action);
		}

		private static void DoLaunchDelayedInEditMode(double waitDoneTime, [NotNull]Action action)
		{
			if(UnityEditor.EditorApplication.timeSinceStartup < waitDoneTime)
			{
				DrawGUI.OnNextBeginOnGUI(()=>DoLaunchDelayedInEditMode(waitDoneTime, action), true);
			}
			else
			{
				action();
			}
		}
		#endif
	}
}