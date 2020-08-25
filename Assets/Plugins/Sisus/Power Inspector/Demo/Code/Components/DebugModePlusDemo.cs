#pragma warning disable CS0414 //we are interested in seeing how the fields look in the inspector, so this warning isn't valid here

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sisus.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sisus.Demo
{
	#if UNITY_2018_3_OR_NEWER
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class DebugModePlusDemo : MonoBehaviour
	{
		[PHeader("Let's take a look at how\n<em>Debug Mode+</em> can be\nuseful during debugging.\n",
			"Try invoking the <em>Print List Items</em> method and see what happens.\n",
			"Next try entering <em>Debug Mode+</em> using the header toolbar button and see if you can find the source of the issue.")]
		[ShowInInspector, ReadOnly, Tooltip("Results of the last \"Print List Items\" run.")]
		private string lastRunResult = "";

		/// <summary>
		/// A list of Objects.
		/// 
		/// Should not contain any null Objects.
		/// </summary>
		[NotNullMembers, HideInInspector]
		private List<Object> list = new List<Object>();

		/// <summary> Lists names of all items in the list. </summary>
		/// <exception cref="NullReferenceException"> Thrown when a value in list was unexpectedly null. </exception>
		[ShowInInspector]
		public void PrintListItems()
		{
			lastRunResult = "";

			foreach(var item in list)
			{
				if(item == null)
				{
					lastRunResult = "NullReferenceException";
					throw new NullReferenceException();
				}
				Debug.Log("\"" + item.name + "\" (" + item.GetType().Name + ")");
			}
			
			// if the foreach loop finished without any exceptions
			lastRunResult = "Success! :)";
		}

		[UsedImplicitly]
		private void Reset()
		{
			BuildList();
		}

		[UsedImplicitly]
		private void OnEnable()
		{
			BuildList();
		}

		private void BuildList()
		{
			list.Clear();
			list.Add(gameObject);
			list.Add(this);
			list.Add(GetComponent<Camera>()); // What could go wrong?
		}

		[PSpace(5f)]
		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowFeature("debug-mode");
		}
	}
}