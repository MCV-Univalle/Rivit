using UnityEngine;

using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

public class SampleStartLocal : MonoBehaviour
{
	void Awake()
	{
		SPlugins.SRemoteConsole.StartLocal();
		SPlugins.SRemoteConsole.RegisterUnityDebugLogCallback();
	}

	void OnGUI()
	{
		GUILayout.Label("SampleStartLocal");
		GUILayout.Space(20f);
		if (true == GUILayout.Button("Write Debug.Log"))
		{
			SPlugins.SRemoteConsole.Log("Write SRemoteConsole.Log", this);
		}

		GUILayout.Space(20f);
		if (true == GUILayout.Button("Write Debug.LogWarning"))
		{
			SPlugins.SRemoteConsole.LogWarning("Write SRemoteConsole.LogWarning", this);
		}

		GUILayout.Space(20f);
		if (true == GUILayout.Button("Write Debug.LogError"))
		{
			SPlugins.SRemoteConsole.LogError("Write SRemoteConsole.LogError", this);
		}

		GUILayout.Space(20f);
		_log = GUILayout.TextField(_log);
		if (true == GUILayout.Button("Send Log"))
		{
			SPlugins.SRemoteConsole.Log(_log);
		}
	}

	void OnDestroy()
	{
		SPlugins.SRemoteConsole.ShoutDown();
	}

	private string _log = string.Empty;
}

